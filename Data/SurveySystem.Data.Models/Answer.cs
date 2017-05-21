namespace SurveySystem.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using SurveySystem.Data.Common.Models;

    public class Answer : BaseModel<int>
    {
        public int QuestionId { get; set; }

        public virtual Question Question { get; set; }

        [Required]
        public string Text { get; set; }
    }
}
