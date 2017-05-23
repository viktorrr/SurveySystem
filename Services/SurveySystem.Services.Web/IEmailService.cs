namespace SurveySystem.Services.Web
{
    public interface IEmailService
    {
        void SendNewReservationEmail(string email, string surveyTitle, string surveyUrl);
    }
}