﻿using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Garland.Data
{
    public static class Utils
    {
        public static KeyValuePair<string, JToken> GetPair(JObject value)
        {
            var collection = (ICollection<KeyValuePair<string, JToken>>)value;
            return collection.First();
        }

        public static JToken GetFirst(JArray value)
        {
            return value.First();
        }
        public static string CapitalizeWords(string str)
        {
            var parts = str.Split(' ');
            for (var i = 0; i < parts.Length; i++)
            {
                var s = parts[i];
                if (s.Length > 0)
                    parts[i] = char.ToUpper(s[0]) + s.Substring(1);
            }

            return string.Join(" ", parts);
        }

        public static string SanitizeTags(string str)
        {
            return str
                .Replace("<Emphasis>", "")
                .Replace("</Emphasis>", "")
                .Replace("<SoftHyphen/>", "")
                .Replace("<Indent/>", "");
        }

        public static string RemoveLineBreaks (string str)
        {
            return str
                .Replace("\r", "")
                .Replace("\n", "");
        }

        public static string SqlEscape(string str)
        {
            return str.Replace("\\", "\\\\").Replace("'", "''");
        }

        public static string Capitalize(string str)
        {
            var characters = str.ToCharArray();
            characters[0] = char.ToUpper(characters[0]);
            return new string(characters);
        }

        public static object Unbox(JToken token)
        {
            if (token.Type == JTokenType.Integer)
                return (int)token;
            if (token.Type == JTokenType.String)
                return (string)token;

            throw new NotImplementedException();
        }

        public static IEnumerable<string[]> Tsv(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);
            return lines.Select(l => l.Split('\t'));
        }

        public static IEnumerable<string[]> Csv(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);
            return lines.Select(l => l.Split(','));
        }

        public static JToken Json(string path)
        {
            var lines = System.IO.File.ReadAllText(path);
            return JToken.Parse(lines);
        }

        private static string[] _comma = new string[] { ", " };
        public static string[] Comma(string str)
        {
            return str.Split(_comma, StringSplitOptions.None);
        }

        public static int[] IntComma(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            return Comma(str).Select(int.Parse).ToArray();
        }

        public static float[] FloatComma(string str)
        {
            if (string.IsNullOrEmpty(str))
                return null;

            return Comma(str).Select(i => float.Parse(i, CultureInfo.InvariantCulture)).ToArray();
        }

        public static string[] Tokenize(string[] delimiters, string str)
        {
            var tokens = new List<string>();
            var buf = new StringBuilder();

            for (var i = 0; i < str.Length; i++)
            {
                var c = str[i];
                var gotToken = false;
                foreach (var delimiter in delimiters)
                {
                    if (c == delimiter[0] && str.Substring(i, delimiter.Length) == delimiter)
                    {
                        if (buf.Length > 0)
                        {
                            tokens.Add(buf.ToString());
                            buf.Clear();
                        }

                        gotToken = true;
                        tokens.Add(delimiter);
                        i += delimiter.Length - 1;
                        break;
                    }
                }

                if (!gotToken)
                    buf.Append(c);
            }

            if (buf.Length > 0)
                tokens.Add(buf.ToString());

            return tokens.ToArray();
        }
    }
}
