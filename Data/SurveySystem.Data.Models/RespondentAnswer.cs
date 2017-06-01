namespace SurveySystem.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using SurveySystem.Data.Common.Models;

    public class RespondentAnswer : BaseModel<int>
    {
        [Required]
        public virtual Question Question { get; set; }

        public virtual QuestionAnswer QuestionAnswer { get; set; }

        public virtual Respondent Respondent { get; set; }

        [Required]
        public virtual Submission Submission { get; set; }

        public string Text { get; set; }
    }
}
