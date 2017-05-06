namespace SurveySystem.Web.Util
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Ajax.Utilities;

    using SurveySystem.Web.Models.Survey;

    public static class EnumUtil
    {
        private static readonly Dictionary<SurveyQuestionType, string> QuestionDictionary =
            new Dictionary<SurveyQuestionType, string>
            {
                { SurveyQuestionType.FreeText, "Свободен текст" },
                { SurveyQuestionType.Checkbox, "Множество отговори(чекбокс)" },
                { SurveyQuestionType.RadioButton, "Единствен отговор(радио бутон)" },
            };

        public static string ToString(SurveyQuestionType type)
        {
            return QuestionDictionary[type];
        }

        public static IEnumerable<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
    }
}
