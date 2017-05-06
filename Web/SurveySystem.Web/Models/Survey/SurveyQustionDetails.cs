namespace SurveySystem.Web.Models.Survey
{
    using System.ComponentModel.DataAnnotations;

    public class SurveyQustionDetails
    {
        public SurveyQustionDetails()
        {
            this.Type = SurveyQuestionType.Text;
        }

        [Required]
        public string Text { get; set; }

        public string Answer { get; set; }

        public SurveyQuestionType Type { get; set; }
    }
}