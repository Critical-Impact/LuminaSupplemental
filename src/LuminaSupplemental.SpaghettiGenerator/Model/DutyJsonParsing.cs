using System;
using System.Globalization;

using LuminaSupplemental.Excel.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LuminaSupplemental.SpaghettiGenerator.Model
{
    public class DutyJson
    {
        [JsonProperty("category")]
        public Category Category { get; set; }

        [JsonProperty("version")]
        public Version Version { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("lvl")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Lvl { get; set; }

        [JsonProperty("ilvl")]
        public IlvlUnion Ilvl { get; set; }

        [JsonProperty("chests", NullValueHandling = NullValueHandling.Ignore)]
        public ChestElement[] Chests { get; set; }

        [JsonProperty("fights", NullValueHandling = NullValueHandling.Ignore)]
        public Fight[] Fights { get; set; }
    }

    public class ChestElement
    {
        [JsonProperty("name")]
        public ChestName Name { get; set; }

        [JsonProperty("coord")]
        public string Coord { get; set; }

        [JsonProperty("coords")]
        public Coords Coords { get; set; }

        [JsonProperty("items")]
        public string[] Items { get; set; }
    }

    public class Coords
    {
        [JsonProperty("x")]
        public string X { get; set; }

        [JsonProperty("y")]
        public string Y { get; set; }
    }

    public class Fight
    {
        [JsonProperty("chest")]
        public ChestEnum Chest { get; set; }

        [JsonProperty("boss")]
        public Boss[] Boss { get; set; }

        [JsonProperty("treasures", NullValueHandling = NullValueHandling.Ignore)]
        public Treasure[] Treasures { get; set; }

        [JsonProperty("drops", NullValueHandling = NullValueHandling.Ignore)]
        public Drop[] Drops { get; set; }

        [JsonProperty("token", NullValueHandling = NullValueHandling.Ignore)]
        public Token[] Token { get; set; }
    }

    public class Boss
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class Drop
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }

        [JsonProperty("condition", NullValueHandling = NullValueHandling.Ignore)]
        public Condition[] Condition { get; set; }
    }

    public class Token
    {
        [JsonProperty("name")]
        public TokenName Name { get; set; }

        [JsonProperty("amount")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long Amount { get; set; }
    }

    public class Treasure
    {
        [JsonProperty("name")]
        public ChestName Name { get; set; }

        [JsonProperty("items")]
        public string[] Items { get; set; }
    }

    public enum Category { Dungeon, Guildhest, Raid, Trial, Ultimate };

    public enum ChestName { TreasureCoffer, TreasureCoffer1, TreasureCoffer2, TreasureCoffer3, TreasureCoffer4, TreasureCoffer5, TreasureCoffer6, TreasureCoffer7, TreasureCoffer8 };


    public enum Condition { DropsAtAFixedRate, MustFirstCompleteTheQuestTripleTriadTrial, ThisItemCanOnlyBeObtainedOncePerWeek };

    public enum TokenName { 
        AllaganTomestoneOfAphorism,
        AllaganTomestoneOfAstronomy,
        AllaganTomestoneOfPoetics,
        AllaganTomestoneofCausality,
        AllaganTomestoneofAesthetics,
        AllaganTomestoneofHeliometry 
    };

    public enum IlvlEnum { Empty };

    public enum Version { ARealmReborn, Endwalker, Heavensward, Shadowbringers, Stormblood, Dawntrail };

    public partial struct IlvlUnion
    {
        public IlvlEnum? Enum;
        public long? Integer;

        public static implicit operator IlvlUnion(IlvlEnum Enum) => new IlvlUnion { Enum = Enum };
        public static implicit operator IlvlUnion(long Integer) => new IlvlUnion { Integer = Integer };
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                CategoryConverter.Singleton,
                ChestNameConverter.Singleton,
                ChestEnumConverter.Singleton,
                ConditionConverter.Singleton,
                TokenNameConverter.Singleton,
                IlvlUnionConverter.Singleton,
                IlvlEnumConverter.Singleton,
                VersionConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class CategoryConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Category) || t == typeof(Category?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "dungeon":
                    return Category.Dungeon;
                case "guildhest":
                    return Category.Guildhest;
                case "raid":
                    return Category.Raid;
                case "trial":
                    return Category.Trial;
                case "ultimate":
                    return Category.Ultimate;
            }
            throw new Exception("Cannot unmarshal type Category");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Category)untypedValue;
            switch (value)
            {
                case Category.Dungeon:
                    serializer.Serialize(writer, "dungeon");
                    return;
                case Category.Guildhest:
                    serializer.Serialize(writer, "guildhest");
                    return;
                case Category.Raid:
                    serializer.Serialize(writer, "raid");
                    return;
                case Category.Trial:
                    serializer.Serialize(writer, "trial");
                    return;
                case Category.Ultimate:
                    serializer.Serialize(writer, "ultimate");
                    return;
            }
            throw new Exception("Cannot marshal type Category");
        }

        public static readonly CategoryConverter Singleton = new CategoryConverter();
    }

    internal class ChestNameConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ChestName) || t == typeof(ChestName?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Treasure Coffer":
                    return ChestName.TreasureCoffer;
                case "Treasure Coffer 1":
                    return ChestName.TreasureCoffer1;
                case "Treasure Coffer 2":
                    return ChestName.TreasureCoffer2;
                case "Treasure Coffer 3":
                    return ChestName.TreasureCoffer3;
                case "Treasure Coffer 4":
                    return ChestName.TreasureCoffer4;
                case "Treasure Coffer 5":
                    return ChestName.TreasureCoffer5;
                case "Treasure Coffer 6":
                    return ChestName.TreasureCoffer6;
                case "Treasure Coffer 7":
                    return ChestName.TreasureCoffer7;
                case "Treasure Coffer 8":
                    return ChestName.TreasureCoffer8;
            }
            throw new Exception("Cannot unmarshal type ChestName");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ChestName)untypedValue;
            switch (value)
            {
                case ChestName.TreasureCoffer:
                    serializer.Serialize(writer, "Treasure Coffer");
                    return;
                case ChestName.TreasureCoffer1:
                    serializer.Serialize(writer, "Treasure Coffer 1");
                    return;
                case ChestName.TreasureCoffer2:
                    serializer.Serialize(writer, "Treasure Coffer 2");
                    return;
                case ChestName.TreasureCoffer3:
                    serializer.Serialize(writer, "Treasure Coffer 3");
                    return;
                case ChestName.TreasureCoffer4:
                    serializer.Serialize(writer, "Treasure Coffer 4");
                    return;
                case ChestName.TreasureCoffer5:
                    serializer.Serialize(writer, "Treasure Coffer 5");
                    return;
                case ChestName.TreasureCoffer6:
                    serializer.Serialize(writer, "Treasure Coffer 6");
                    return;
                case ChestName.TreasureCoffer7:
                    serializer.Serialize(writer, "Treasure Coffer 7");
                    return;
                case ChestName.TreasureCoffer8:
                    serializer.Serialize(writer, "Treasure Coffer 8");
                    return;
            }
            throw new Exception("Cannot marshal type ChestName");
        }

        public static readonly ChestNameConverter Singleton = new ChestNameConverter();
    }

    internal class ChestEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(ChestEnum) || t == typeof(ChestEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "gold":
                    return ChestEnum.Gold;
                case "silver":
                    return ChestEnum.Silver;
            }
            throw new Exception("Cannot unmarshal type ChestEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (ChestEnum)untypedValue;
            switch (value)
            {
                case ChestEnum.Gold:
                    serializer.Serialize(writer, "gold");
                    return;
                case ChestEnum.Silver:
                    serializer.Serialize(writer, "silver");
                    return;
            }
            throw new Exception("Cannot marshal type ChestEnum");
        }

        public static readonly ChestEnumConverter Singleton = new ChestEnumConverter();
    }

    internal class ConditionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Condition) || t == typeof(Condition?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "* Drops at a fixed rate.":
                    return Condition.DropsAtAFixedRate;
                case "* Must first complete the quest \"Triple Triad Trial.\"":
                    return Condition.MustFirstCompleteTheQuestTripleTriadTrial;
                case "* This item can only be obtained once per week.":
                    return Condition.ThisItemCanOnlyBeObtainedOncePerWeek;
            }
            throw new Exception("Cannot unmarshal type Condition");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Condition)untypedValue;
            switch (value)
            {
                case Condition.DropsAtAFixedRate:
                    serializer.Serialize(writer, "* Drops at a fixed rate.");
                    return;
                case Condition.MustFirstCompleteTheQuestTripleTriadTrial:
                    serializer.Serialize(writer, "* Must first complete the quest \"Triple Triad Trial.\"");
                    return;
                case Condition.ThisItemCanOnlyBeObtainedOncePerWeek:
                    serializer.Serialize(writer, "* This item can only be obtained once per week.");
                    return;
            }
            throw new Exception("Cannot marshal type Condition");
        }

        public static readonly ConditionConverter Singleton = new ConditionConverter();
    }

    internal class ParseStringConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(long) || t == typeof(long?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (long)untypedValue;
            serializer.Serialize(writer, value.ToString());
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class TokenNameConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TokenName) || t == typeof(TokenName?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "Allagan Tomestone of Aphorism":
                    return TokenName.AllaganTomestoneOfAphorism;
                case "Allagan Tomestone of Astronomy":
                    return TokenName.AllaganTomestoneOfAstronomy;
                case "Allagan Tomestone of Poetics":
                    return TokenName.AllaganTomestoneOfPoetics;
                case "Allagan Tomestone of Causality":
                    return TokenName.AllaganTomestoneofCausality;
                case "Allagan Tomestone of Aesthetics":
                    return TokenName.AllaganTomestoneofAesthetics;
                case "Allagan Tomestone of Heliometry":
                    return TokenName.AllaganTomestoneofHeliometry;
            }
            throw new Exception("Cannot unmarshal type TokenName");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TokenName)untypedValue;
            switch (value)
            {
                case TokenName.AllaganTomestoneOfAphorism:
                    serializer.Serialize(writer, "Allagan Tomestone of Aphorism");
                    return;
                case TokenName.AllaganTomestoneOfAstronomy:
                    serializer.Serialize(writer, "Allagan Tomestone of Astronomy");
                    return;
                case TokenName.AllaganTomestoneOfPoetics:
                    serializer.Serialize(writer, "Allagan Tomestone of Poetics");
                    return;
                case TokenName.AllaganTomestoneofCausality:
                    serializer.Serialize(writer, "Allagan Tomestone of Causality");
                    return;
                case TokenName.AllaganTomestoneofAesthetics:
                    serializer.Serialize(writer, "Allagan Tomestone of Aesthetics");
                    return;
                case TokenName.AllaganTomestoneofHeliometry:
                    serializer.Serialize(writer, "Allagan Tomestone of Heliometry");
                    return;
            }
            throw new Exception("Cannot marshal type TokenName");
        }

        public static readonly TokenNameConverter Singleton = new TokenNameConverter();
    }

    internal class IlvlUnionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(IlvlUnion) || t == typeof(IlvlUnion?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            switch (reader.TokenType)
            {
                case JsonToken.String:
                case JsonToken.Date:
                    var stringValue = serializer.Deserialize<string>(reader);
                    if (stringValue == "-")
                    {
                        return new IlvlUnion { Enum = IlvlEnum.Empty };
                    }
                    long l;
                    if (Int64.TryParse(stringValue, out l))
                    {
                        return new IlvlUnion { Integer = l };
                    }
                    break;
            }
            throw new Exception("Cannot unmarshal type IlvlUnion");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            var value = (IlvlUnion)untypedValue;
            if (value.Enum != null)
            {
                if (value.Enum == IlvlEnum.Empty)
                {
                    serializer.Serialize(writer, "-");
                    return;
                }
            }
            if (value.Integer != null)
            {
                serializer.Serialize(writer, value.Integer.Value.ToString());
                return;
            }
            throw new Exception("Cannot marshal type IlvlUnion");
        }

        public static readonly IlvlUnionConverter Singleton = new IlvlUnionConverter();
    }

    internal class IlvlEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(IlvlEnum) || t == typeof(IlvlEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "-")
            {
                return IlvlEnum.Empty;
            }
            throw new Exception("Cannot unmarshal type IlvlEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (IlvlEnum)untypedValue;
            if (value == IlvlEnum.Empty)
            {
                serializer.Serialize(writer, "-");
                return;
            }
            throw new Exception("Cannot marshal type IlvlEnum");
        }

        public static readonly IlvlEnumConverter Singleton = new IlvlEnumConverter();
    }

    internal class VersionConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(Version) || t == typeof(Version?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "A Realm Reborn":
                    return Version.ARealmReborn;
                case "Endwalker":
                    return Version.Endwalker;
                case "Heavensward":
                    return Version.Heavensward;
                case "Shadowbringers":
                    return Version.Shadowbringers;
                case "Stormblood":
                    return Version.Stormblood;
                case "Dawntrail":
                    return Version.Dawntrail;
            }
            throw new Exception("Cannot unmarshal type Version");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (Version)untypedValue;
            switch (value)
            {
                case Version.ARealmReborn:
                    serializer.Serialize(writer, "A Realm Reborn");
                    return;
                case Version.Endwalker:
                    serializer.Serialize(writer, "Endwalker");
                    return;
                case Version.Heavensward:
                    serializer.Serialize(writer, "Heavensward");
                    return;
                case Version.Shadowbringers:
                    serializer.Serialize(writer, "Shadowbringers");
                    return;
                case Version.Stormblood:
                    serializer.Serialize(writer, "Stormblood");
                    return;
                case Version.Dawntrail:
                    serializer.Serialize(writer, "Dawntrail");
                    return;
            }
            throw new Exception("Cannot marshal type Version");
        }

        public static readonly VersionConverter Singleton = new VersionConverter();
    }
}
