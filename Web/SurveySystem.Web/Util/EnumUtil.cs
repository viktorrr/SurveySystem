namespace SurveySystem.Web.Util
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;

    using Microsoft.Ajax.Utilities;

    using SurveySystem.Data.Models;
    using SurveySystem.Web.Models.Survey;

    public static class EnumUtil
    {
        private static readonly Dictionary<QuestionType, string> QuestionDictionary =
            new Dictionary<QuestionType, string>
            {
                { QuestionType.FreeText, "Свободен текст" },
                { QuestionType.Checkbox, "Множество отговори(чекбокс)" },
                { QuestionType.RadioButton, "Единствен отговор(радио бутон)" }
            };

        public static string ToString(QuestionType type)
        {
            return QuestionDictionary[type];
        }

        public static ICollection<T> GetEnumValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
    }
}
