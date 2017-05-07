namespace SurveySystem.Web.Models.Survey
{
    using SurveySystem.Data.Models;

    public class FreeTextQuestion : SingleAnswerQuestion
    {
        public override QuestionType QuestionType => QuestionType.FreeText;
    }
}