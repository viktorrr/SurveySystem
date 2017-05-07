namespace SurveySystem.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using SurveySystem.Data.Common.Models;

    public class Survey : BaseModel<int>
    {
        public Survey()
        {
            this.Questions = new HashSet<Question>();
        }

        [Required]
        public string Title { get; set; }

        [Required]
        public bool IsAnonymous { get; set; }

        [Required]
        public DateTime BeginsOn { get; set; }

        public ICollection<Question> Questions { get; set; }
    }
}
