namespace SurveySystem.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using SurveySystem.Data.Common.Models;

    public class SubmissionCode : BaseModel<int>
    {
        [Required]
        public string Code { get; set; }

        public int? SubmissionId { get; set; }
    }
}
