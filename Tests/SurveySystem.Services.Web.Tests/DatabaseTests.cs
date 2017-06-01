namespace SurveySystem.Services.Web.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    using SurveySystem.Data;
    using SurveySystem.Data.Models;
    using SurveySystem.Web.Models.Survey;

    [TestFixture]
    public class DatabaseTests
    {
        [Test]
        public void SanityTest()
        {
            var survey = new Survey
            {
                Title = "Tittle",
                IsAnonymous = true,
                BeginsOn = DateTime.Now
            };

            var freeTextQuestion = new Question
            {
                Survey = survey,
                QuestionType = QuestionType.FreeText,
                Text = "free text question",
                SequenceNumber = 0
            };

            var checkBoxQuestion = new Question
            {
                Survey = survey,
                QuestionType = QuestionType.Checkbox,
                Text = "checkbox question",
                SequenceNumber = 1
            };
            checkBoxQuestion.QuestionAnswers.Add(new QuestionAnswer { Text = "true" });
            checkBoxQuestion.QuestionAnswers.Add(new QuestionAnswer { Text = "false" });

            var radioButtonQuestion = new Question
            {
                Survey = survey,
                QuestionType = QuestionType.RadioButton,
                Text = "radio button"
            };
            radioButtonQuestion.QuestionAnswers.Add(new QuestionAnswer { Text = "a" });
            radioButtonQuestion.QuestionAnswers.Add(new QuestionAnswer { Text = "b" });
            radioButtonQuestion.QuestionAnswers.Add(new QuestionAnswer { Text = "c" });

            var db = new ApplicationDbContext();

            db.Surveys.Add(survey);
            db.SaveChanges();

            Assert.True(db.Surveys.Count() == 1);
            Assert.True(checkBoxQuestion.QuestionAnswers.Count() == 2);
            Assert.True(radioButtonQuestion.QuestionAnswers.Count() == 3);
        }
    }
}
