namespace SurveySystem.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;

    using AutoMapper.Internal;

    using Glimpse.Core.Extensibility;
    using Glimpse.Core.Extensions;

    using SurveySystem.Data;
    using SurveySystem.Data.Models;
    using SurveySystem.Web.Models.Survey;

    using WebGrease.Css.Extensions;

    public class SurveyController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

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

            // TODO: validate this!
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
                }

                survey.Questions.Add(question);
            }

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

            var dbCheckboxQuestions = questionsMap[QuestionType.Checkbox].OrderBy(x => x.SequenceNumber).ToList();
            for (var i = 0; i < userSubmission.CheckBoxQuestions.Count; i++)
            {
                var dbQuestion = dbCheckboxQuestions[i];
                var answers = dbQuestion.QuestionAnswers.OrderBy(x => x.Id).Select(x => x.Text).ToList();

                for (var j = 0; j < userSubmission.CheckBoxQuestions[i].Answered.Count; j++)
                {
                    if (userSubmission.CheckBoxQuestions[i].Answered[j])
                    {
                        dbQuestion.RespondentAnswers.Add(new RespondentAnswer { Text = answers[j] });
                    }
                }
            }

            var dbRadioButtonQuestions = questionsMap[QuestionType.RadioButton].OrderBy(x => x.SequenceNumber).ToList();
            for (int i = 0; i < userSubmission.RadioButtonQuestions.Count; i++)
            {
                var dbQuestion = dbRadioButtonQuestions[i];
                var answeredQuestion = userSubmission.RadioButtonQuestions[i];

                dbQuestion.RespondentAnswers.Add(new RespondentAnswer { Text = answeredQuestion.Answer });
            }

            var freeTextQuestions = questionsMap[QuestionType.FreeText].OrderBy(x => x.SequenceNumber).ToList();
            for (int i = 0; i < userSubmission.FreeTextQuestions.Count; i++)
            {
                var dbQuestion = freeTextQuestions[i];
                var answeredQuestion = userSubmission.FreeTextQuestions[i];

                dbQuestion.RespondentAnswers.Add(new RespondentAnswer { Text = answeredQuestion.Answer });
            }

            this.db.SaveChanges();

            return this.View(userSubmission);
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
                        Answers = x.QuestionAnswers.Select(y => y.Text).ToList(),
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