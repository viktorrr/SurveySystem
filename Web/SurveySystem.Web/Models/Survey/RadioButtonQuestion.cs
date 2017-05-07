namespace SurveySystem.Web.Models.Survey
{
    using SurveySystem.Data.Models;

    public class RadioButtonQuestion : SingleAnswerQuestion
    {
        public override QuestionType QuestionType => QuestionType.RadioButton;
    }
}