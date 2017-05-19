namespace SurveySystem.Web.Models.Survey
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public abstract class SingleAnswerQuestion : BaseSurveyQuestion
    {
        public IList<string> AvailableAnswers { get; set; }

        [Required(ErrorMessage = "Полето е задължително.")]
        public string Answer { get; set; }
    }
}