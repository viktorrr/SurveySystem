namespace SurveySystem.Web.Models.Survey
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SurveySystem.Data.Models;

    public class CheckBoxQuestion : MultiAnswerQuestion
    {
        public IList<bool> Answered { get; set; }

        public override QuestionType QuestionType => QuestionType.Checkbox;
    }
}