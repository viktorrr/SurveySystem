namespace SurveySystem.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using SurveySystem.Data.Common.Models;

    public class Respondent : BaseModel<int>
    {
        public Respondent()
        {
            this.Answers = new HashSet<RespondentAnswer>();
        }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string FacultyNumber { get; set; }

        [Required]
        public virtual Submission Submission { get; set; }

        public int SubmissionId { get; set; }

        public virtual ICollection<RespondentAnswer> Answers { get; set; }
    }
}
