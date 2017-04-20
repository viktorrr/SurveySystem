namespace SurveySystem.Web.Models.Survey
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using SurveySystem.Data.Models;
    using SurveySystem.Web.Infrastructure.Mapping;

    public class NewSurveyRequest : IMapTo<Survey>
    {
        [Required(ErrorMessage = "Заглавието е задължително.")]
        public string Title { get; set; }

        public bool Anonymous { get; set; }

        [Required(ErrorMessage = "Началната дата е задължителна.")]
        public DateTime BeginsOn { get; set; }

        [Required(ErrorMessage = "Крайната дата е задължителна.")]
        public DateTime EndsOn { get; set; }
    }
}