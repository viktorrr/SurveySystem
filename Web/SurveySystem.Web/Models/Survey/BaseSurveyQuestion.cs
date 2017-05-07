namespace SurveySystem.Web.Models.Survey
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using SurveySystem.Data.Models;

    public abstract class BaseSurveyQuestion
    {
        public int SequentialNumber { get; set; }

        public string Text { get; set; }

        public abstract QuestionType QuestionType { get; }
    }
}