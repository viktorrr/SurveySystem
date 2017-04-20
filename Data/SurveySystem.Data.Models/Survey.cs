namespace SurveySystem.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using SurveySystem.Data.Common.Models;

    public class Survey : IAuditInfo
    {
        public string Title { get; set; }

        [Required]
        public DateTime BeginsOn { get; set; }

        [Required]
        public DateTime EndsOn { get; set; }

        [Required]
        public bool Anonymous { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? ModifiedOn { get; set; }
    }
}
