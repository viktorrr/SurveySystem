namespace SurveySystem.Web.Models.Survey
{
    using System.ComponentModel.DataAnnotations;

    public class SurveyRespondent
    {
        [Required(ErrorMessage = "Полето е задължително.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Полето е задължително.")]
        public string LastName { get; set; }

        [EmailAddress(ErrorMessage = "Невалиден е-майл адрес.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Полето е задължително.")]
        public string FacultyNumber { get; set; }
    }
}