using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Lumina.Excel.GeneratedSheets;

namespace Garland.Data
{
    public static class Utils
    {
        public static string? GetShopName(uint argument, string label)
        {
            if (label.Contains("FCCSHOP"))
                return "Spend company credits (items)";
            else if (label == "MOBSHOP1")
                return "Exchange Centurio Seals";
            else if (label == "MOBSHOP2")
                return "Exchange Centurio Seals (Advanced)";
            else if (label == "SHOP_SPOIL")
                return "Exchange Spoils";
            else if (label == "SPECIAL_SHOP0" && argument == 1769813)
                return "Achievement Rewards";
            else if (label == "SPECIAL_SHOP1" && argument == 1769845)
                return "Achievement Rewards 2";
            else if (label == "SPECIAL_SHOP2" && argument == 1769846)
                return "Achievement Rewards 3";
            else if (label == "SHOP_0" && argument == 1769842)
                return "Gold Certificates of Commendation";
            else if (label == "SHOP_1" && argument == 1769841)
                return "Silver Certificates of Commendation";
            else if (label == "SHOP_2" && argument == 1769956)
                return "Bronze Certificates of Commendation";
            else if (label == "SHOP" && argument == 1769812)
                return "PVP Rewards";
            else if (label == "REPLICA_SHOP0" && argument == 262918)
                return "Purchase a Eureka weapon replica (DoW).";
            else if (label == "REPLICA_SHOP1" && argument == 262922)
                return "Purchase a Eureka weapon replica (DoM).";
            else if (label == "FREE_SHOP_BATTLE" && argument == 1769898)
                return "Battle Achievement Rewards";
            else if (label == "FREE_SHOP_PVP" && argument == 1769899)
                return "PvP Achievement Rewards";
            else if (label == "FREE_SHOP_CHARACTER" && argument == 1769900)
                return "Character Achievement Rewards";
            else if (label == "FREE_SHOP_ITEM" && argument == 1769901)
                return "Item Achievement Rewards";
            else if (label == "FREE_SHOP_CRAFT" && argument == 1769902)
                return "Crafting Achievement Rewards";
            else if (label == "FREE_SHOP_GATHERING" && argument == 1769903)
                return "Gathering Achievement Rewards";
            else if (label == "FREE_SHOP_QUEST" && argument == 1769904)
                return "Quest Achievement Rewards";
            else if (label == "FREE_SHOP_EXPLORATION" && argument == 1769905)
                return "Exploration Achievement Rewards";
            else if (label == "FREE_SHOP_GRANDCOMPANY" && argument == 1769906)
                return "Grand Company Achievement Rewards";

            else if (label == "SPSHOP_HANDLER_ID" && argument == 1770041)
                return "Skybuilders' Scrips";
            else if (label == "SPSHOP2_HANDLER_ID" && argument == 1770281)
                return "Skybuilders' Scrips (Gear/Furnishings)";
            else if (label == "SPSHOP3_HANDLER_ID" && argument == 1770301)
                return "Skybuilders' Scrips (Materials/Materia/Items)";
            else if (label == "SPSHOP4_HANDLER_ID" && argument == 1770343)
                return "Fête Tokens";
            else
            {
                return null;
            }
        }
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
