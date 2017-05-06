namespace SurveySystem.Web.Models.Survey
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class MultiAnswerQuestion : BaseSurveyQuestion
    {
        public IList<string> Answers { get; set; }
    }
}