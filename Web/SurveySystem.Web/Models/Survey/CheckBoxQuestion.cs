namespace SurveySystem.Web.Models.Survey
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class CheckBoxQuestion : MultiAnswerQuestion
    {
        public IList<bool> Answered { get; set; }

        public override SurveyQuestionType QuestionType => SurveyQuestionType.Checkbox;
    }
}