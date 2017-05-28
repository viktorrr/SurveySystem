namespace SurveySystem.Web.Models.Survey
{
    using System.Collections.Generic;

    public class BasicSurveyDetails
    {
        public int Id { get; set; }

        public bool IsAnonymous { get; set; }

        public IList<BasicSubmissionDetails> Submissions { get; set; }
    }
}