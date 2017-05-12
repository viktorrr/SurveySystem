namespace SurveySystem.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Web.Mvc;

    using AutoMapper.Internal;

    using SurveySystem.Data;
    using SurveySystem.Data.Models;
    using SurveySystem.Web.Models.Survey;

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
            var submission = this.GetSubmission(id);
            return this.View(submission);
        }

        [HttpPost]
        public ViewResult Submit(int id, SurveySubmission submission)
        {
            // TODO: validate user input (id + answers)!
            return this.View(submission);
        }

        private List<QuestionAnswer> CreateQuestionAnswers(Question question, string answers)
        {
            var questionAnswers = answers.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            return questionAnswers.Select(x => new QuestionAnswer { Question = question, Text = x }).ToList();
        }

        private SurveySubmission GetSubmission(int id)
        {
            var survey = this.db.Surveys.Include(x => x.Questions).First(x => x.Id == id);

            var result = survey.Questions
                .GroupBy(x => x.QuestionType, (type, values) => new { Type = type, Questions = values })
                .ToDictionary(x => x.Type, x => x.Questions);

            var freeTextQuestions = new List<FreeTextQuestion>();
            var radioButtonQuestions = new List<RadioButtonQuestion>();
            var checkBoxQuestions = new List<CheckBoxQuestion>();

            if (result.ContainsKey(QuestionType.FreeText))
            {
                freeTextQuestions = result[QuestionType.FreeText]
                    .Select(x => new FreeTextQuestion { SequentialNumber = x.SequenceNumber, Text = x.Text }).ToList();
            }

            if (result.ContainsKey(QuestionType.Checkbox))
            {
                checkBoxQuestions = result[QuestionType.Checkbox].Select(
                    x => new CheckBoxQuestion
                    {
                        Text = x.Text,
                        SequentialNumber = x.SequenceNumber,
                        Answers = x.QuestionAnswers.Select(y => y.Text).ToList(),
                    }).ToList();
            }

            if (result.ContainsKey(QuestionType.RadioButton))
            {
                radioButtonQuestions = result[QuestionType.RadioButton].Select(
                    x => new RadioButtonQuestion
                    {
                        Text = x.Text,
                        AvailableAnswers = x.QuestionAnswers.Select(y => y.Text).ToList(),
                        SequentialNumber = x.SequenceNumber
                    }).ToList();
            }

            return new SurveySubmission(id, freeTextQuestions, radioButtonQuestions, checkBoxQuestions);
        }
    }
}