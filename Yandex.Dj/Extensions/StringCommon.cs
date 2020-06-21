using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Yandex.Dj.Extensions
{
    /// <summary>
    /// Свой тип, чтобы не привязывать дополнительно класс регулярных выражений
    /// </summary>
    public enum RegExOptions
    {
        None = 0,
        IgnoreCase = 1,
        Multiline = 2,
        ExplicitCapture = 4,
        Singleline = 16,
        IgnorePatternWhitespace = 32,
        RightToLeft = 64,
        ECMAScript = 256,
        CultureInvariant = 512,
    }

    /// <summary>
    /// Предоставляет набор методов расширения для работы со строками.
    /// </summary>
    public static class StringCommon
    {
        #region Работа с регулярными выражениями

        /// <summary>
        /// Замена в строке через регулярное выражение
        /// </summary>
        /// <param name="regExpr">Регулярное выражение</param>
        /// <param name="str">Строка</param>
        /// <param name="replStr">Строка-замена</param>
        /// <param name="options">Опции регулярного выражения</param>
        /// <returns></returns>
        public static string ReplaceRegex(this string str, string regExpr, string replStr, RegExOptions options)
        {
            return str == null
                ? string.Empty
                : Regex.Replace(str, regExpr, replStr, (RegexOptions)options);
        }

        /// <summary>
        /// Замена в строке через регулярное выражение
        /// </summary>
        /// <param name="regExpr">Регулярное выражение</param>
        /// <param name="str">Строка</param>
        /// <param name="replStr">Строка-замена</param>
        /// <returns></returns>
        public static string ReplaceRegex(this string str, string regExpr, string replStr)
        {
            return ReplaceRegex(str, regExpr, replStr, RegExOptions.None);
        }

        /// <summary>
        /// Удаляет буквы и знаки препинания в строке
        /// </summary>
        /// <param name="str">Строка</param>
        public static string RemoveLetters(this string str)
        {
            return ReplaceRegex(str, @"[^\d]", "");
        }

        /// <summary>
        /// Удаляет все вхождения указанной подстроки в строке
        /// </summary>
        /// <param name="str">Строка</param>
        /// <param name="subStr">Подстрока</param>
        public static string RemoveSubStr(this string str, string subStr)
        {
            return ReplaceRegex(str, subStr, "");
        }

        /// <summary>
        /// Заменяет все вхождения указанной подстроки в строке
        /// </summary>
        /// <param name="str">Строка</param>
        /// <param name="subStr">Подстрока</param>
        /// <param name="newSubStr">Новая подстрока</param>
        public static string ReplaceSubStr(this string str, string subStr, string newSubStr)
        {
            return ReplaceRegex(str, subStr, newSubStr);
        }

        /// <summary>
        /// Удаляет цифры в начале и конце строки
        /// </summary>
        /// <param name="str">Строка</param>
        public static string TrimNumbers(this string str)
        {
            return ReplaceRegex(str, @"^[\d]*|[\d]*$", "");
        }

        /// <summary>
        /// Удаляет цифры в строке
        /// </summary>
        /// <param name="str">Строка</param>
        public static string RemoveNumbers(this string str)
        {
            return ReplaceRegex(str, @"[\d]", "");
        }

        /// <summary>
        /// Удаляет буквы и знаки препинания в начале и конце строки
        /// </summary>
        /// <param name="str">Строка</param>
        public static string TrimLetters(this string str)
        {
            string exp = @"-?[\d](.*)(?<![^\d])";
            if (Regex.IsMatch(str, exp))
                return Regex.Match(str, exp).Captures[0].Value;
            return string.Empty;
        }

        /// <summary>
        /// Удаляет лишние пробелы и переносы в строке
        /// </summary>
        /// <param name="str">Строка</param>
        public static string RemoveSpaces(this string str)
        {
            return ReplaceRegex(str, @"\s+", "");
        }

        /// <summary>
        /// Заменяет лишние пробелы и переносы в строке на указанную строку
        /// </summary>
        public static string ReplaceSpaces(this string str, string replacementStr)
        {
            return ReplaceRegex(str, @"\s+", replacementStr);
        }

        /// <summary>
        /// Проверяет соответствие регулярному выражению
        /// </summary>
        public static bool IsMatch(this string str, string pattern, RegExOptions options)
        {
            return Regex.IsMatch(str, pattern, (RegexOptions)options);
        }

        /// <summary>
        /// Проверяет соответствие регулярному выражению
        /// </summary>
        public static bool IsMatch(this string str, string pattern)
        {
            return IsMatch(str, pattern, RegExOptions.IgnoreCase);
        }

        /// <summary>
        /// Возвращает совпадения для регулярного выражения
        /// </summary>
        public static string [] GetMatches(this string str, string pattern, RegExOptions options)
        {
            if (str.IsMatch(pattern, options))
                return Regex.Matches(str, pattern, (RegexOptions)options).Cast<Match>().Select(m => m.Value).ToArray();

            return new string[] { };
        }

        /// <summary>
        /// Возвращает совпадения для регулярного выражения
        /// </summary>
        public static string[] GetMatches(this string str, string pattern)
        {
            return str.GetMatches(pattern, RegExOptions.IgnoreCase);
        }

        #endregion Работа с регулярными выражениями
    }
}
