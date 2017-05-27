namespace SurveySystem.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using SurveySystem.Data.Common.Models;

    public class Submission : BaseModel<int>
    {
        public Submission()
        {
            this.Answers = new HashSet<RespondentAnswer>();;
        }

        public virtual Survey Survey { get; set; }

        public int SurveyId { get; set; }

        public virtual Respondent Respondent { get; set; }

        public int? RespondentId { get; set; }

        public DateTime BeginsOn { get; set; }

        public ICollection<RespondentAnswer> Answers { get; set; }
    }
}
