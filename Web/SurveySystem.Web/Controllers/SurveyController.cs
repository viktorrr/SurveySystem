namespace SurveySystem.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

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
            var i = -1;
            var freeTextQuestions = new List<FreeTextQuestion>
            {
                new FreeTextQuestion { SequentialNumber = ++i, Answer = "42", Text = "what's the meaning of life?" },
                new FreeTextQuestion { SequentialNumber = ++i }
            };

            var radioButtonQuestions = new List<RadioButtonQuestion>
            {
                new RadioButtonQuestion { SequentialNumber = ++i, AvailableAnswers = new List<string> { "R1", "R1" } },
                new RadioButtonQuestion { SequentialNumber = ++i, AvailableAnswers = new List<string> { "R2", "R22" } },
            };

            var checkBoxQuestions = new List<CheckBoxQuestion>
            {
                new CheckBoxQuestion { SequentialNumber = ++i, Answers = new List<string> { "C1", "C11", "C111" } },
                new CheckBoxQuestion { SequentialNumber = ++i, Answers = new List<string> { "C222" } }
            };

            var surveyQuestions = new List<BaseSurveyQuestion>();
            surveyQuestions.AddRange(freeTextQuestions);
            surveyQuestions.AddRange(radioButtonQuestions);
            surveyQuestions.AddRange(checkBoxQuestions);

            var questionTypes = surveyQuestions.Select(x => x.QuestionType).ToList();

            var submission = new SurveySubmission(id, questionTypes, freeTextQuestions, radioButtonQuestions, checkBoxQuestions);
            return this.View(submission);
        }

        [HttpPost]
        public ViewResult Submit(SurveySubmission submission)
        {
            return this.View(submission);
        }

        private List<QuestionAnswer> CreateQuestionAnswers(Question question, string answers)
        {
            var questionAnswers = answers.Split(
                new[] { Environment.NewLine },
                StringSplitOptions.RemoveEmptyEntries);

            return questionAnswers.Select(x => new QuestionAnswer { Question = question, Text = x }).ToList();
        }

        private IList<BaseSurveyQuestion> GetQuestions()
        {
            int i = -1;
            var freeTextQuestions = new List<FreeTextQuestion>
            {
                new FreeTextQuestion { SequentialNumber = i++ },
                new FreeTextQuestion { SequentialNumber = i++ }
            };

            var radioButtonQuestions = new List<RadioButtonQuestion>
            {
                new RadioButtonQuestion { SequentialNumber = i++, AvailableAnswers = new List<string> { "R1", "R1" } },
                new RadioButtonQuestion { SequentialNumber = i++, AvailableAnswers = new List<string> { "R2", "R22" } },
            };

            var checkBoxQuestions = new List<CheckBoxQuestion>
            {
                new CheckBoxQuestion { SequentialNumber = i++, Answers = new List<string> { "C1", "C11", "C111" } },
                new CheckBoxQuestion { SequentialNumber = i++, Answers = new List<string> { "C222" } }
            };

            var surveyQuestions = new List<BaseSurveyQuestion>();
            surveyQuestions.AddRange(freeTextQuestions);
            surveyQuestions.AddRange(radioButtonQuestions);
            surveyQuestions.AddRange(checkBoxQuestions);

            return surveyQuestions;
        }
    }
}