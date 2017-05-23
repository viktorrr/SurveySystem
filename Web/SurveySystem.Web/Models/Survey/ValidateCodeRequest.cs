namespace SurveySystem.Web.Models.Survey
{
    using System.ComponentModel.DataAnnotations;

    public class ValidateCodeRequest
    {
        [Required(ErrorMessage = "Полето е задължително.")]
        public string Code { get; set; }

        public int SurveyId { get; set; }
    }
}