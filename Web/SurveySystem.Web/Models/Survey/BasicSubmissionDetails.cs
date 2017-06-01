namespace SurveySystem.Web.Models.Survey
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    using SurveySystem.Data.Models;

    public class BasicSubmissionDetails
    {
        private readonly Dictionary<int, FreeTextQuestion> freeTextQuestionsCache;
        private readonly Dictionary<int, RadioButtonQuestion> radioButtonQuestionsCache;
        private readonly Dictionary<int, CheckBoxQuestion> checkBoxQuestionsCache;

        public BasicSubmissionDetails(
            DateTime beganOn,
            DateTime completedOn,
            BasicRespondentDetails respondent,
            int id)
        {
            this.BeganOn = beganOn;
            this.CompletedOn = completedOn;
            this.Respondent = respondent;
            this.Id = id;
        }

        public BasicSubmissionDetails(
            IList<FreeTextQuestion> freeTextQuestions,
            IList<RadioButtonQuestion> radioButtonQuestions,
            IList<CheckBoxQuestion> checkBoxQuestions)
        {
            this.FreeTextQuestions = freeTextQuestions;
            this.RadioButtonQuestions = radioButtonQuestions;
            this.CheckBoxQuestions = checkBoxQuestions;

            this.freeTextQuestionsCache = this.FreeTextQuestions.ToDictionary(x => x.SequentialNumber, x => x);
            this.radioButtonQuestionsCache = this.RadioButtonQuestions.ToDictionary(x => x.SequentialNumber, x => x);
            this.checkBoxQuestionsCache = this.CheckBoxQuestions.ToDictionary(x => x.SequentialNumber, x => x);

            var surveyQuestions = new List<BaseSurveyQuestion>();
            surveyQuestions.AddRange(freeTextQuestions);
            surveyQuestions.AddRange(radioButtonQuestions);
            surveyQuestions.AddRange(checkBoxQuestions);

            this.QuestionTypes = surveyQuestions.OrderBy(x => x.SequentialNumber).Select(x => x.QuestionType).ToList();
        }

        public int Id { get; set; }

        public DateTime BeganOn { get; set; }

        public DateTime CompletedOn { get; set; }

        public BasicRespondentDetails Respondent { get; set; }

        public IList<FreeTextQuestion> FreeTextQuestions { get; set; }

        public IList<RadioButtonQuestion> RadioButtonQuestions { get; set; }

        public IList<CheckBoxQuestion> CheckBoxQuestions { get; set; }

        [JsonIgnore]
        public IList<QuestionType> QuestionTypes { get; set; }

        public FreeTextQuestion GetFreeTextQuestion(int number)
        {
            return this.freeTextQuestionsCache[number];
        }

        public RadioButtonQuestion GetRadioButtonQuestion(int number)
        {
            return this.radioButtonQuestionsCache[number];
        }

        public CheckBoxQuestion GetCheckBoxQuestion(int number)
        {
            return this.checkBoxQuestionsCache[number];
        }
    }
}