namespace SurveySystem.Web.Models.Survey
{
    using System.Collections.Generic;
    using System.Linq;

    using SurveySystem.Data.Models;

    public class SurveySubmission
    {
        private readonly Dictionary<int, FreeTextQuestion> freeTextQuestionsCache;
        private readonly Dictionary<int, RadioButtonQuestion> radioButtonQuestionsCache;
        private readonly Dictionary<int, CheckBoxQuestion> checkBoxQuestionsCache;

        public SurveySubmission()
        {
            // default ctor, used by the framework to initialize the model
            this.CheckBoxQuestions = new List<CheckBoxQuestion>();
            this.FreeTextQuestions = new List<FreeTextQuestion>();
            this.RadioButtonQuestions = new List<RadioButtonQuestion>();
        }

        public SurveySubmission(
            int surveyId,
            string beganOn,
            IList<FreeTextQuestion> freeTextQuestions,
            IList<RadioButtonQuestion> radioButtonQuestions,
            IList<CheckBoxQuestion> checkBoxQuestions)
        {
            this.SurveyId = surveyId;
            this.BeganOn = beganOn;

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

        public int SurveyId { get; set; }

        public SurveyRespondent Respondent { get; set; }

        public IList<FreeTextQuestion> FreeTextQuestions { get; set; }

        public IList<RadioButtonQuestion> RadioButtonQuestions { get; set; }

        public IList<CheckBoxQuestion> CheckBoxQuestions { get; set; }

        public IList<QuestionType> QuestionTypes { get; set; }

        public string BeganOn { get; set; }

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