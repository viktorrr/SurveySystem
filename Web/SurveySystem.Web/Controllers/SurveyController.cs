namespace SurveySystem.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;

    using SurveySystem.Data;
    using SurveySystem.Data.Models;
    using SurveySystem.Services.Web;
    using SurveySystem.Web.Models.Survey;

    public class SurveyController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private IEmailService emailService = new EmailService();

        [HttpGet]
        public ViewResult New()
        {
            return this.View();
        }

        [HttpPost]
        public ViewResult New(NewSurveyRequest request)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(request);
            }

            var survey = new Survey
            {
                IsAnonymous = request.IsAnonymous,
                BeginsOn = request.BeginsOn,
                Title = request.Title
            };

            for (int i = 0; i < request.Questions.Count; i++)
            {
                var questionDetails = request.Questions[i];
                var question = new Question
                {
                    Text = questionDetails.Text,
                    QuestionType = questionDetails.Type,
                    SequenceNumber = i
                };

                if (question.QuestionType != QuestionType.FreeText)
                {
                    question.QuestionAnswers = this.CreateQuestionAnswers(question, questionDetails.Answer);
                    if (!question.QuestionAnswers.Any())
                    {
                        this.ModelState.AddModelError($"Questions[{i}].QuestionAnswer", "Въпросът трябва да има поне един отговор.");
                    }
                }

                survey.Questions.Add(question);
            }

            if (!this.ModelState.IsValid)
            {
                return this.View(request);
            }

            // TODO: redirect to another page!
            this.db.Surveys.Add(survey);
            this.db.SaveChanges();

            return this.View();
        }

        [HttpGet]
        public ViewResult Submit(int id = 1)
        {
            // TODO: validate date!
            var submission = this.CreateRawSubmission(id);
            return this.View(submission);
        }

        [HttpPost]
        public ViewResult Submit(int id, SurveySubmission userSubmission)
        {
            if (!this.ModelState.IsValid)
            {
                var rawSubmission = this.CreateSubmissionWithAnswers(id, userSubmission);
                return this.View(rawSubmission);
            }

            var survey = this.GetSurvey(id);
            var questionsMap = this.BuildQuestionMap(survey);

            var dbSubmission = new Submission { Survey = survey };

            var dbCheckboxQuestions = questionsMap[QuestionType.Checkbox].OrderBy(x => x.SequenceNumber).ToList();
            for (var i = 0; i < userSubmission.CheckBoxQuestions.Count; i++)
            {
                var dbQuestion = dbCheckboxQuestions[i];
                var answers = dbQuestion.QuestionAnswers.OrderBy(x => x.Id).ToList();

                for (var j = 0; j < userSubmission.CheckBoxQuestions[i].Answered.Count; j++)
                {
                    if (userSubmission.CheckBoxQuestions[i].Answered[j])
                    {
                        dbSubmission.Answers.Add(new RespondentAnswer { QuestionAnswer = answers[j] });
                    }
                }
            }

            var dbRadioButtonQuestions = questionsMap[QuestionType.RadioButton].OrderBy(x => x.SequenceNumber).ToList();
            for (int i = 0; i < userSubmission.RadioButtonQuestions.Count; i++)
            {
                var dbQuestion = dbRadioButtonQuestions[i];
                var answeredQuestion = userSubmission.RadioButtonQuestions[i];
                var dbAnswer = dbQuestion.QuestionAnswers.First(x => x.Text == answeredQuestion.Answer);

                dbQuestion.RespondentAnswers.Add(new RespondentAnswer { QuestionAnswer = dbAnswer });
            }

            var freeTextQuestions = questionsMap[QuestionType.FreeText].OrderBy(x => x.SequenceNumber).ToList();
            for (int i = 0; i < userSubmission.FreeTextQuestions.Count; i++)
            {
                var dbQuestion = freeTextQuestions[i];
                var answeredQuestion = userSubmission.FreeTextQuestions[i];

                dbQuestion.RespondentAnswers.Add(new RespondentAnswer { Text = answeredQuestion.Answer });
            }

            this.db.Submission.Add(dbSubmission);
            this.db.SaveChanges();

            // TODO: redirect to another page!
            return this.View(userSubmission);
        }

        [HttpGet]
        public ViewResult Details(int id)
        {
            return this.View(id);
        }

        [HttpGet]
        public ViewResult Validate(int id)
        {
            ValidateCodeRequest request = null;

            if (this.db.Surveys.FirstOrDefault(x => x.Id == id) != null)
            {
                request = new ValidateCodeRequest { SurveyId = id };
            }

            return this.View(request);
        }

        [HttpPost]
        public ViewResult Validate(ValidateCodeRequest request)
        {
            // TODO: validate
            return this.View("ValidateResult", true);
        }

        [HttpGet]
        public ActionResult Invite(int id)
        {
            EmailInvitation invitation = null;

            if (this.db.Surveys.FirstOrDefault(x => x.Id == id) != null)
            {
                invitation = new EmailInvitation { SurveyId = id };
            }

            return this.View(invitation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Invite(EmailInvitation request)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View("InviteResult", false);
            }

            var emails = request.EmailList
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries );

            if (emails.Length == 0)
            {
                return this.View("InviteResult", false);
            }

            var survey = this.db.Surveys.First(x => x.Id == request.SurveyId);
            var url = this.FormatSurveyUrl(request.SurveyId);

            if (survey == null)
            {
                return this.View("InviteResult", false);
            }

            this.emailService.SendNewReservationEmail(request.EmailList, survey.Title, url);
            return this.View("InviteResult", true);
        }

        private string FormatSurveyUrl(int id)
        {
            var scheme = this.Request.Url.Scheme;
            var authority = this.Request.Url.Authority;
            var modifyUrl = this.Url.Action("Submit", new { id = id });

            return $"{scheme}://{authority}{modifyUrl}";
        }

        private SurveySubmission CreateSubmissionWithAnswers(int id, SurveySubmission userSubmission)
        {
            var result = this.CreateRawSubmission(id);
            for (int i = 0; i < result.CheckBoxQuestions.Count; i++)
            {
                result.CheckBoxQuestions[i].Answered =
                    userSubmission.CheckBoxQuestions[i].Answered;
            }

            for (var i = 0; i < result.FreeTextQuestions.Count; i++)
            {
                result.FreeTextQuestions[i] = userSubmission.FreeTextQuestions[i];
            }

            for (var i = 0; i < result.RadioButtonQuestions.Count; i++)
            {
                result.RadioButtonQuestions[i] = userSubmission.RadioButtonQuestions[i];
            }

            return result;
        }

        private List<QuestionAnswer> CreateQuestionAnswers(Question question, string answers)
        {
            if (string.IsNullOrEmpty(answers))
            {
                return new List<QuestionAnswer>();
            }

            var questionAnswers = answers.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            return questionAnswers.Select(x => new QuestionAnswer { Question = question, Text = x }).ToList();
        }

        private SurveySubmission CreateRawSubmission(int id)
        {
            var survey = this.GetSurvey(id);
            var questionsMap = this.BuildQuestionMap(survey);

            var freeTextQuestions = this.BuildFreeTextQuestions(questionsMap);
            var radioButtonQuestions = this.RadioButtonQuestions(questionsMap);
            var checkBoxQuestions = this.BuildCheckBoxQuestions(questionsMap);

            return new SurveySubmission(id, freeTextQuestions, radioButtonQuestions, checkBoxQuestions);
        }

        private Survey GetSurvey(int id)
        {
            return this.db.Surveys.Include(x => x.Questions).First(x => x.Id == id);
        }

        private IDictionary<QuestionType, IEnumerable<Question>> BuildQuestionMap(Survey survey)
        {
            return survey.Questions
                .GroupBy(x => x.QuestionType, (type, values) => new { Type = type, Questions = values })
                .ToDictionary(x => x.Type, x => x.Questions);
        }

        private List<CheckBoxQuestion> BuildCheckBoxQuestions(
            IDictionary<QuestionType, IEnumerable<Question>> questionsMap)
        {
            var result = new List<CheckBoxQuestion>();
            if (questionsMap.ContainsKey(QuestionType.Checkbox))
            {
                result = questionsMap[QuestionType.Checkbox].Select(
                    x => new CheckBoxQuestion
                    {
                        Text = x.Text,
                        SequentialNumber = x.SequenceNumber,
                        Answers = x.QuestionAnswers.OrderBy(y => y.Id).Select(y => y.Text).ToList()
                    }).ToList();
            }

            return result;
        }

        private List<FreeTextQuestion> BuildFreeTextQuestions(
            IDictionary<QuestionType, IEnumerable<Question>> questionsMap)
        {
            var result = new List<FreeTextQuestion>();
            if (questionsMap.ContainsKey(QuestionType.FreeText))
            {
                result = questionsMap[QuestionType.FreeText]
                    .Select(x => new FreeTextQuestion { SequentialNumber = x.SequenceNumber, Text = x.Text }).ToList();
            }

            return result;
        }

        private List<RadioButtonQuestion> RadioButtonQuestions(
            IDictionary<QuestionType, IEnumerable<Question>> questionsMap)
        {
            var result = new List<RadioButtonQuestion>();
            if (questionsMap.ContainsKey(QuestionType.RadioButton))
            {
                result = questionsMap[QuestionType.RadioButton].Select(
                    x => new RadioButtonQuestion
                    {
                        Text = x.Text,
                        AvailableAnswers = x.QuestionAnswers.Select(y => y.Text).ToList(),
                        SequentialNumber = x.SequenceNumber
                    }).ToList();
            }

            return result;
        }
    }
}