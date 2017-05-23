namespace SurveySystem.Web.Models.Survey
{
    using System.ComponentModel.DataAnnotations;

    public class EmailInvitation
    {
        [Required(ErrorMessage = "Полето трябва да съдържа поне един адрес.")]
        public string EmailList { get; set; }

        public int SurveyId { get; set; }
    }
}