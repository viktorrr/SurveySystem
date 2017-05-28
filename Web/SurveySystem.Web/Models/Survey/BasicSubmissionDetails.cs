namespace SurveySystem.Web.Models.Survey
{
    using System;
    using System.Collections.Generic;

    public class BasicSubmissionDetails
    {
        public BasicSubmissionDetails(
            DateTime beganOn,
            DateTime completedOn,
            BasicRespondentDetails respondent)
        {
            this.BeganOn = beganOn;
            this.CompletedOn = completedOn;
            this.Respondent = respondent;
        }

        public DateTime BeganOn { get; set; }

        public DateTime CompletedOn { get; set; }

        public BasicRespondentDetails Respondent { get; set; }

    }
}