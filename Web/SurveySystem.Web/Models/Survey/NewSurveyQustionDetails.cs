namespace SurveySystem.Web.Models.Survey
{
    using System.ComponentModel.DataAnnotations;

    using SurveySystem.Data.Models;

    public class NewSurveyQustionDetails
    {
        public NewSurveyQustionDetails()
        {
            this.Type = QuestionType.FreeText;
        }

        [Required]
        public string Text { get; set; }

        public string Answer { get; set; }

        public QuestionType Type { get; set; }
    }
}