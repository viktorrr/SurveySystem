namespace SurveySystem.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Data.Entity;

    using SurveySystem.Data.Common.Models;

    public class Question : BaseModel<int>
    {
        public Question()
        {
            this.QuestionAnswers = new HashSet<QuestionAnswer>();
            this.RespondentAnswers = new HashSet<RespondentAnswer>();
        }

        [Required]
        public int SequenceNumber { get; set; }

        [Required]
        public string Text { get; set; }

        [Required]
        public QuestionType QuestionType { get; set; }

        public virtual Survey Survey { get; set; }

        public int SurveyId { get; set; }

        public virtual ICollection<QuestionAnswer> QuestionAnswers { get; set; }

        public virtual ICollection<RespondentAnswer> RespondentAnswers { get; set; }
    }
}
