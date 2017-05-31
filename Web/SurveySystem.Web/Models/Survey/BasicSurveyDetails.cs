namespace SurveySystem.Web.Models.Survey
{
    using System;
    using System.Collections.Generic;

    public class BasicSurveyDetails
    {
        public int Id { get; set; }

        public bool IsAnonymous { get; set; }

        public string Tittle { get; set; }

        public DateTime CreatedOn { get; set; }

        public IList<BasicSubmissionDetails> Submissions { get; set; }
    }
}