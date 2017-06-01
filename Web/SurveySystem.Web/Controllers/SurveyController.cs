namespace SurveySystem.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Web.Mvc;

    using Newtonsoft.Json;

    using SurveySystem.Common;
    using SurveySystem.Data;
    using SurveySystem.Data.Models;
    using SurveySystem.Services.Web;
    using SurveySystem.Web.Models.Survey;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    public class SurveyController : BaseController
    {
        private const int RandomStringLength = 15; // 15 because reasons...
        private static readonly char[] Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890~!@#$".ToCharArray();

        private ApplicationDbContext db = new ApplicationDbContext();
        private IEmailService emailService = new EmailService();

        [HttpGet]
        public ViewResult New()
        {
            return this.View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult New(NewSurveyRequest request)
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

            this.db.Surveys.Add(survey);
            this.db.SaveChanges();

            return this.RedirectToAction("Details", new { id = survey.Id });
        }

        [HttpGet]
        [AllowAnonymous]
        public ViewResult Submit(int id = 1)
        {
            var survey = this.GetSurvey(id);
            if (survey == null || survey.BeginsOn > DateTime.Now)
            {
                return this.View((SurveySubmission)null);
            }

            var submission = this.BuildSurveySubmission(survey);
            if (!survey.IsAnonymous)
            {
                submission.Respondent = new SurveyRespondent();
            }

            return this.View(submission);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ViewResult Submit(int id, SurveySubmission userSubmission)
        {
            var survey = this.GetSurvey(id);

            if (!this.ModelState.IsValid)
            {
                var rawSubmission = this.CreateSubmissionWithAnswers(survey, userSubmission);
                return this.View(rawSubmission);
            }

            var questionsMap = this.BuildQuestionMap(survey);
            var dbSubmission = new Submission
            {
                Survey = survey,
                BeginsOn = this.GetTimestamp(userSubmission.BeganOn)
            };
            var code = new SubmissionCode { Code = this.GenerateCode() };

            Respondent respondent = null;

            if (!survey.IsAnonymous)
            {
                respondent = new Respondent
                {
                    FirstName = userSubmission.Respondent.FirstName,
                    LastName = userSubmission.Respondent.LastName,
                    FacultyNumber = userSubmission.Respondent.FacultyNumber,
                    Email = userSubmission.Respondent.Email,
                    IP = this.HttpContext.Request.UserHostAddress,
                    Submission = dbSubmission
                };
            }

            dbSubmission.Respondent = respondent;

            var dbCheckboxQuestions = this.GetQuestions(questionsMap, QuestionType.Checkbox);
            for (var i = 0; i < userSubmission.CheckBoxQuestions.Count; i++)
            {
                var dbQuestion = dbCheckboxQuestions[i];
                var answers = dbQuestion.QuestionAnswers.OrderBy(x => x.Id).ToList();

                for (var j = 0; j < userSubmission.CheckBoxQuestions[i].Answered.Count; j++)
                {
                    if (userSubmission.CheckBoxQuestions[i].Answered[j])
                    {
                        var answer = new RespondentAnswer
                        {
                            QuestionAnswer = answers[j],
                            Question = dbQuestion,
                            Submission = dbSubmission,
                            Respondent = respondent
                        };

                        dbQuestion.RespondentAnswers.Add(answer);
                        dbSubmission.Answers.Add(answer);
                    }
                }
            }

            var dbRadioButtonQuestions = this.GetQuestions(questionsMap, QuestionType.RadioButton);
            for (int i = 0; i < userSubmission.RadioButtonQuestions.Count; i++)
            {
                var dbQuestion = dbRadioButtonQuestions[i];
                var answeredQuestion = userSubmission.RadioButtonQuestions[i];
                var dbAnswer = dbQuestion.QuestionAnswers.First(x => x.Text == answeredQuestion.Answer);

                var answer = new RespondentAnswer
                {
                    QuestionAnswer = dbAnswer,
                    Question = dbQuestion,
                    Submission = dbSubmission,
                    Respondent = respondent
                };
                dbQuestion.RespondentAnswers.Add(answer);
            }

            var freeTextQuestions = this.GetQuestions(questionsMap, QuestionType.FreeText);
            for (int i = 0; i < userSubmission.FreeTextQuestions.Count; i++)
            {
                var dbQuestion = freeTextQuestions[i];
                var answeredQuestion = userSubmission.FreeTextQuestions[i];

                var answer = new RespondentAnswer
                {
                    Text = answeredQuestion.Answer,
                    Submission = dbSubmission,
                    Respondent = respondent,
                    Question = dbQuestion
                };
                dbQuestion.RespondentAnswers.Add(answer);
            }

            if (respondent != null)
            {
                this.db.Respondents.Add(respondent);
            }

            this.db.Submissions.Add(dbSubmission);
            this.db.SubmissionCodes.Add(code);

            this.db.SaveChanges();

            if (!survey.IsAnonymous)
            {
                code.SubmissionId = dbSubmission.Id;
                dbSubmission.RespondentId = respondent.Id;
            }

            this.db.SaveChanges();

            return this.View("SubmitResult", (object)code.Code);
        }

        [HttpGet]
        public ActionResult All()
        {
            var surveys = this.db.Surveys
                .OrderBy(x => x.CreatedOn)
                .Select(x => new BasicSurveyDetails
                {
                    Id = x.Id,
                    IsAnonymous = x.IsAnonymous,
                    Tittle = x.Title,
                    CreatedOn = x.CreatedOn
                })
                .ToList();

            return this.View(surveys);
        }

        [HttpGet]
        public ViewResult Details(int id)
        {
            var survey = this.GetSurvey(id);
            if (survey == null)
            {
                return this.View((object)null);
            }

            var submissions = survey.Submissions.OrderBy(x => x.Id).Select(this.CreateSubmissionDetails).ToList();

            var surveyDetails = new BasicSurveyDetails
            {
                Id = survey.Id,
                Tittle = survey.Title,
                IsAnonymous = survey.IsAnonymous,
                Submissions = submissions
            };

            return this.View(surveyDetails);
        }

        [HttpGet]
        public JsonResult GetQuestionMap(int id)
        {
            var survey = this.GetSurvey(id);
            if (survey == null)
            {
                return new JsonResult();
            }

            var questionMap = this.CreateQuestionMap(survey);

            return this.Json(questionMap, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DownloadSubmission(int id)
        {
            var dbSubmission = this.db.Submissions.FirstOrDefault(x => x.Id == id);
            if (dbSubmission == null)
            {
                return new JsonResult();
            }

            var details = this.CreateSubmissionDetails(id);
            var json = JsonConvert.SerializeObject(details);

            return this.File(Encoding.UTF8.GetBytes(json), "application/json", $"submission-{id}.json");
        }

        [HttpGet]
        public ActionResult Serialize(int id)
        {
            var survey = this.GetSurvey(id);
            if (survey == null)
            {
                return new JsonResult();
            }

            var questionMap = this.CreateQuestionMap(survey);
            var json = JsonConvert.SerializeObject(questionMap);

            return this.File(Encoding.UTF8.GetBytes(json), "application/json", $"survey-{id}.json");
        }

        [HttpGet]
        public ViewResult Validate()
        {
            return this.View();
        }

        [HttpPost]
        public ViewResult Validate(ValidateCodeRequest request)
        {
            var exists = this.db.SubmissionCodes.Any(x => x.Code == request.Code);
            return this.View("ValidateResult", exists);
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
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

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

        [HttpGet]
        public ActionResult Submission(int id)
        {
            var submission = this.db.Submissions.FirstOrDefault(x => x.Id == id);
            if (submission == null)
            {
                return this.View((object)null);
            }

            var details = this.CreateSubmissionDetails(submission);
            return this.View(details);
        }

        private BasicSubmissionDetails CreateSubmissionDetails(int id)
        {
            var freeTextQuestions = this.db.RespondentAnswers
                .Where(x => x.Submission.Id == id && x.Text != null)
                .Select(x => new FreeTextQuestion
                {
                    Text = x.Question.Text,
                    Answer = x.Text,
                    SequentialNumber = x.Question.SequenceNumber
                })
                .ToList();

            var checkboxQuestions = this.db.RespondentAnswers
                .Where(x => x.Submission.Id == id && x.Question.QuestionType == QuestionType.Checkbox)
                .Include(x => x.QuestionAnswer)
                .GroupBy(x => x.Question, x => x)
                .ToDictionary(x => x.Key, x => x.ToList())
                .Select(x => new CheckBoxQuestion
                {
                    Text = x.Key.Text,
                    SequentialNumber = x.Key.SequenceNumber,
                    Answers = x.Value.Select(q => q.QuestionAnswer.Text).ToList()
                })
                .ToList();

            var radioButtonQuestions = this.db.RespondentAnswers
                .Where(x => x.Submission.Id == id && x.Question.QuestionType == QuestionType.RadioButton)
                .Include(x => x.QuestionAnswer)
                .Select(x => new RadioButtonQuestion
                {
                    SequentialNumber = x.Question.SequenceNumber,
                    Answer = x.QuestionAnswer.Text,
                    Text = x.Question.Text
                })
                .ToList();

            var submission = this.db.Submissions.First(x => x.Id == id);
            var details = this.CreateSubmissionDetails(submission);

            details.CheckBoxQuestions = checkboxQuestions;
            details.RadioButtonQuestions = radioButtonQuestions;
            details.FreeTextQuestions = freeTextQuestions;

            return details;
        }

        private Dictionary<string, Dictionary<string, int>> CreateQuestionMap(Survey survey)
        {
            return survey.Questions.Where(x => x.QuestionType != QuestionType.FreeText)
                    .ToDictionary(
                        question => question.Text,
                        question =>
                            question.QuestionAnswers.ToDictionary(
                                answer => answer.Text,
                                // TODO: fix this..
                                answer => this.db.RespondentAnswers.Count(respondentAnswer => respondentAnswer.QuestionAnswer.Id == answer.Id)));
        }

        private BasicSubmissionDetails CreateSubmissionDetails(Submission submission)
        {
            BasicRespondentDetails respondent = null;
            if (submission.Respondent != null)
            {
                respondent = new BasicRespondentDetails(
                    submission.Respondent.FirstName,
                    submission.Respondent.LastName,
                    submission.Respondent.Email,
                    submission.Respondent.FacultyNumber,
                    submission.Respondent.IP);
            }

            return new BasicSubmissionDetails(
                 submission.BeginsOn, submission.CreatedOn, respondent, submission.Id);
        }

        private string GetTimestamp()
        {
            return DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        }

        private DateTime GetTimestamp(string timestamp)
        {
            return DateTime.ParseExact(timestamp, "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        }

        private string FormatSurveyUrl(int id)
        {
            var scheme = this.Request.Url.Scheme;
            var authority = this.Request.Url.Authority;
            var modifyUrl = this.Url.Action("Submit", new { id = id });

            return $"{scheme}://{authority}{modifyUrl}";
        }

        private IList<Question> GetQuestions(
            IDictionary<QuestionType, IEnumerable<Question>> questionMap,
            QuestionType questionType)
        {
            var result = new List<Question>();

            if (questionMap.ContainsKey(questionType))
            {
                result = questionMap[questionType].OrderBy(x => x.SequenceNumber).ToList();
            }

            return result;
        }

        private SurveySubmission CreateSubmissionWithAnswers(Survey survey, SurveySubmission userSubmission)
        {
            var result = this.BuildSurveySubmission(survey);
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

        private SurveySubmission BuildSurveySubmission(Survey survey)
        {
            var questionsMap = this.BuildQuestionMap(survey);

            var id = survey.Id;
            var beginsOn = this.GetTimestamp();

            var freeTextQuestions = this.BuildFreeTextQuestions(questionsMap);
            var radioButtonQuestions = this.RadioButtonQuestions(questionsMap);
            var checkBoxQuestions = this.BuildCheckBoxQuestions(questionsMap);

            return new SurveySubmission(id, beginsOn, freeTextQuestions, radioButtonQuestions, checkBoxQuestions);
        }

        private Survey GetSurvey(int id)
        {
            return this.db.Surveys.Include(x => x.Questions).FirstOrDefault(x => x.Id == id);
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

        private string GenerateCode()
        {
            var data = new byte[1];
            using (var crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[RandomStringLength];
                crypto.GetNonZeroBytes(data);
            }

            var result = new StringBuilder(RandomStringLength);
            foreach (var b in data)
            {
                result.Append(Chars[b % Chars.Length]);
            }

            return result.ToString();
        }
    }
}