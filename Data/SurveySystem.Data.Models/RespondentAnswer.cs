namespace SurveySystem.Data.Models
{
    using System.ComponentModel.DataAnnotations;

    using SurveySystem.Data.Common.Models;

    public class RespondentAnswer : BaseModel<int>
    {
        public virtual QuestionAnswer QuestionAnswer { get; set; }

        public virtual Respondent Respondent { get; set; }

        public string Text { get; set; }
    }
}
