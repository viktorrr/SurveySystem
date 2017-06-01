namespace SurveySystem.Web.Models.Survey
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Newtonsoft.Json;

    public abstract class SingleAnswerQuestion : BaseSurveyQuestion
    {
        [JsonIgnore]
        public IList<string> AvailableAnswers { get; set; }

        [Required(ErrorMessage = "Полето е задължително.")]
        public string Answer { get; set; }
    }
}