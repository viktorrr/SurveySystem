namespace SurveySystem.Web.Util
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web.Mvc;

    using Newtonsoft.Json;

    using SurveySystem.Data.Models;
    using SurveySystem.Web.Models.Survey;

    using WebGrease.Css.Extensions;

    public static class HtmlHelperExtensions
    {
        public static Dictionary<string, object> BuildSurveryQuestionProperties(
            this HtmlHelper helper, QuestionType type)
        {
            var result = new Dictionary<string, object>
            {
                { "id", string.Empty }
            };

            if (type == QuestionType.FreeText)
            {
                result["checked"] = "checked";
            }

            return result;
        }

        public static Dictionary<string, object> BuildRadioButtonAnswerAttributes(
            this HtmlHelper helper, RadioButtonQuestion question, int index)
        {
            var result = new Dictionary<string, object>();
            var isChecked = false;

            if (string.IsNullOrEmpty(question.Answer))
            {
                isChecked = index == 0;
            }
            else
            {
                isChecked = question.AvailableAnswers[index] == question.Answer;
            }

            if (isChecked)
            {
                result["checked"] = "checked";
            }

            return result;
        }

        public static string GeneratePieLabels(this HtmlHelper helper, IDictionary<string, int> values)
        {
            return JsonConvert.SerializeObject(values.OrderBy(x => x.Key).Select(x => x.Key));
        }

        public static string GeneratePieValues(this HtmlHelper helper, IDictionary<string, int> values)
        {
            return JsonConvert.SerializeObject(values.OrderBy(x => x.Key).Select(x => x.Value));
        }

        public static string FormatQuestion(this HtmlHelper helper, int questionNumber, BaseSurveyQuestion question)
        {
            return $"{questionNumber + 1}. {question.Text}";
        }
    }
}