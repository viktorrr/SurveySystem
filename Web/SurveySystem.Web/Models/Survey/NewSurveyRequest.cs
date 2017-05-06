namespace SurveySystem.Web.Models.Survey
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using SurveySystem.Data.Models;
    using SurveySystem.Web.Infrastructure.Mapping;

    public class NewSurveyRequest : IMapTo<Survey>
    {
        public NewSurveyRequest()
        {
            this.Questions = new List<SurveyQustionDetails>();
        }

        [Required(ErrorMessage = "Заглавието е задължително.")]
        public string Title { get; set; }

        public bool Anonymous { get; set; }

        [Required(ErrorMessage = "Началната дата е задължителна.")]
        public DateTime BeginsOn { get; set; }

        public IList<SurveyQustionDetails> Questions { get; set; }
    }
}