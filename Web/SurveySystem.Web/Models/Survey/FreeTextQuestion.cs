namespace SurveySystem.Web.Models.Survey
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class FreeTextQuestion : SingleAnswerQuestion
    {
        public override SurveyQuestionType QuestionType => SurveyQuestionType.FreeText;
    }
}