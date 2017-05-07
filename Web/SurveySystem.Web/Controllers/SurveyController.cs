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

            foreach (var questionDetails in request.Questions)
            {
                var question = new Question { Text = questionDetails.Text, QuestionType = questionDetails.Type };
                if (question.QuestionType != QuestionType.FreeText)
                {
                    var questionAnswers = questionDetails.Answer.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
                    var answers = questionAnswers.Select(x => new QuestionAnswer { Question = question, Text = x }).ToList();

                    question.QuestionAnswers = answers;
                }

                survey.Questions.Add(question);
            }

            this.db.Surveys.Add(survey);
            this.db.SaveChanges();

            return this.View();
        }

        [HttpGet]
        public ViewResult Fill(int id = 1)
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
        public ViewResult Fill(SurveySubmission submission)
        {
            return this.View(submission);
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