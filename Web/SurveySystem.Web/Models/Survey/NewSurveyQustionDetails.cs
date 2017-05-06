﻿namespace SurveySystem.Web.Models.Survey
{
    using System.ComponentModel.DataAnnotations;

    public class NewSurveyQustionDetails
    {
        public NewSurveyQustionDetails()
        {
            this.Type = SurveyQuestionType.FreeText;
        }

        [Required]
        public string Text { get; set; }

        public string Answer { get; set; }

        public SurveyQuestionType Type { get; set; }
    }
}