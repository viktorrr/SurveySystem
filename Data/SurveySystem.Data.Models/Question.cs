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
        public string Text { get; set; }

        [Required]
        public QuestionType QuestionType { get; set; }

        public virtual ICollection<QuestionAnswer> QuestionAnswers { get; set; }

        public virtual ICollection<RespondentAnswer> RespondentAnswers { get; set; }
    }

    public class Answer : BaseModel<int>
    {
        public int QuestionId { get; set; }

        public virtual Question Question { get; set; }

        [Required]
        public string Text { get; set; }
    }

    public class QuestionAnswer : Answer
    {
    }

    public class RespondentAnswer : Answer
    {
        // TODO: respondent info!
    }
}
