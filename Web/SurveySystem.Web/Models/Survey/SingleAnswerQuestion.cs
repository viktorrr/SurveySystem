namespace SurveySystem.Web.Models.Survey
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class SingleAnswerQuestion : BaseSurveyQuestion
    {
        public IList<string> AvailableAnswers { get; set; }

        public string Answer { get; set; }
    }
}