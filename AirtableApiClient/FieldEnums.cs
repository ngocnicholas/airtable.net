using System;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public enum FieldType
    {
        SingleLineText,
        Email, 
        Url, 
        MultilineText, 
        Number,
        Percent, 
        Currency, 
        SingleSelect, 
        MultipleSelects, 
        SingleCollaborator,
        MultipleCollaborators, 
        MultipleRecordLinks, 
        Date, 
        DateTime, 
        PhoneNumber,
        MultipleAttachments, 
        Checkbox, 
        Formula, 
        CreatedTime, 
        Rollup,
        Count, 
        MultipleLookupValues, 
        AutoNumber, 
        Barcode,
        Rating, 
        RichText, 
        Duration, 
        LastModifiedTime, 
        Button,
        CreatedBy, 
        LastModifiedBy, 
        ExternalSyncSource, 
        AiText
    }

    // Checkbox field enums
    public enum CheckboxColor
    {
        GreenBright,
        TealBright,
        CyanBright,
        BlueBright,
        PurpleBright,
        PinkBright,
        RedBright,
        OrangeBright,
        YellowBright,
        GrayBright
    }

    public enum CheckboxIcon
    {
        Check,
        XCheckbox,
        Star,
        Heart,
        ThumbsUp,
        Flag,
        Dot
    }

    // Date format enums
    public enum DateFormatType
    {
        L,           // local
        LL,          // friendly
        MDYYYY,      // M/D/YYYY - us
        DMYYYY,      // D/M/YYYY - european
        YYYYMMDD     // YYYY-MM-DD - iso
    }

    public enum DateFormatName
    {
        Local,
        Friendly,
        Us,
        European,
        Iso
    }

    // Time format enums
    public enum TimeFormatType
    {
        H12,         // h:mma
        H24          // HH:mm
    }

    public enum TimeFormatName
    {
        TwelveHour,  // 12hour
        TwentyFourHour // 24hour
    }

    // Duration format enum
    public enum DurationFormatType
    {
        HMM,         // h:mm
        HMMSS,       // h:mm:ss
        HMMSSS,      // h:mm:ss.S
        HMMSSSS,     // h:mm:ss.SS
        HMMSSSSS     // h:mm:ss.SSS
    }

    // Choice/Select colors enum
    public enum ChoiceColor
    {
        BlueLight2,
        CyanLight2,
        TealLight2,
        GreenLight2,
        YellowLight2,
        OrangeLight2,
        RedLight2,
        PinkLight2,
        PurpleLight2,
        GrayLight2,
        BlueLight1,
        CyanLight1,
        TealLight1,
        GreenLight1,
        YellowLight1,
        OrangeLight1,
        RedLight1,
        PinkLight1,
        PurpleLight1,
        GrayLight1,
        BlueBright,
        CyanBright,
        TealBright,
        GreenBright,
        YellowBright,
        OrangeBright,
        RedBright,
        PinkBright,
        PurpleBright,
        GrayBright,
        BlueDark1,
        CyanDark1,
        TealDark1,
        GreenDark1,
        YellowDark1,
        OrangeDark1,
        RedDark1,
        PinkDark1,
        PurpleDark1,
        GrayDark1
    }

    // Rating field enums
    public enum RatingColor
    {
        YellowBright,
        OrangeBright,
        RedBright,
        PinkBright,
        PurpleBright,
        BlueBright,
        CyanBright,
        TealBright,
        GreenBright,
        GrayBright
    }

    public enum RatingIcon
    {
        Star,
        Heart,
        ThumbsUp,
        Flag,
        Dot
    }
    /*
        createdBy, lastModifiedBy, externalSyncSource, aiText

     * */

    // ----------------- Extension methods to convert enums to API strings ------------------------
    public static class EnumExtensions
    {
        public static string ToApiString(this FieldType field)
        {
            return field switch
            { 
                FieldType.SingleLineText => "singleLineText",
                FieldType.Email => "email",
                FieldType.Url => "url",
                FieldType.MultilineText => "multilineText",
                FieldType.Number => "number",
                FieldType.Percent => "percent",
                FieldType.Currency => "currency",
                FieldType.SingleSelect => "singleSelect",
                FieldType.MultipleSelects => "multipleSelects",
                FieldType.SingleCollaborator => "singleCollaborator",
                FieldType.MultipleCollaborators => "multipleCollaborators",
                FieldType.MultipleRecordLinks => "multipleRecordLinks",
                FieldType.Date => "date",
                FieldType.DateTime => "dateTime",
                FieldType.PhoneNumber => "phoneNumber",
                FieldType.MultipleAttachments => "multipleAttachments",
                FieldType.Checkbox => "checkbox",
                FieldType.Formula => "formula",
                FieldType.CreatedTime => "createdTime",
                FieldType.Rollup => "rollup",
                FieldType.Count => "count",
                FieldType.MultipleLookupValues => "multipleLookupValues",
                FieldType.AutoNumber => "autoNumber",
                FieldType.Barcode => "barcode",
                FieldType.Rating => "rating",
                FieldType.RichText => "richText",
                FieldType.Duration => "duration",
                FieldType.LastModifiedTime => "lastModifiedTime",
                FieldType.Button => "button",
                FieldType.CreatedBy => "createdBy",
                FieldType.LastModifiedBy => "lastModifiedBy",
                FieldType.ExternalSyncSource => "externalSyncSource",
                FieldType.AiText => "aiText",
                _ => throw new ArgumentOutOfRangeException(nameof(field), field, "Unsupported FieldType")
            };
        }

        public static string ToApiString(this CheckboxColor color)
        {
            return color switch
            {
                CheckboxColor.GreenBright => "greenBright",
                CheckboxColor.TealBright => "tealBright",
                CheckboxColor.CyanBright => "cyanBright",
                CheckboxColor.BlueBright => "blueBright",
                CheckboxColor.PurpleBright => "purpleBright",
                CheckboxColor.PinkBright => "pinkBright",
                CheckboxColor.RedBright => "redBright",
                CheckboxColor.OrangeBright => "orangeBright",
                CheckboxColor.YellowBright => "yellowBright",
                CheckboxColor.GrayBright => "grayBright",
                _ => throw new ArgumentOutOfRangeException(nameof(color))
            };
        }

        public static string ToApiString(this CheckboxIcon icon)
        {
            return icon switch
            {
                CheckboxIcon.Check => "check",
                CheckboxIcon.XCheckbox => "xCheckbox",
                CheckboxIcon.Star => "star",
                CheckboxIcon.Heart => "heart",
                CheckboxIcon.ThumbsUp => "thumbsUp",
                CheckboxIcon.Flag => "flag",
                CheckboxIcon.Dot => "dot",
                _ => throw new ArgumentOutOfRangeException(nameof(icon))
            };
        }

        public static string ToApiString(this DateFormatType format)
        {
            return format switch
            {
                DateFormatType.L => "l",
                DateFormatType.LL => "LL",
                DateFormatType.MDYYYY => "M/D/YYYY",
                DateFormatType.DMYYYY => "D/M/YYYY",
                DateFormatType.YYYYMMDD => "YYYY-MM-DD",
                _ => throw new ArgumentOutOfRangeException(nameof(format))
            };
        }

        public static string ToApiString(this DateFormatName name)
        {
            return name switch
            {
                DateFormatName.Local => "local",
                DateFormatName.Friendly => "friendly",
                DateFormatName.Us => "us",
                DateFormatName.European => "european",
                DateFormatName.Iso => "iso",
                _ => throw new ArgumentOutOfRangeException(nameof(name))
            };
        }

        public static string ToApiString(this TimeFormatName name)
        {
            return name switch
            {
                TimeFormatName.TwelveHour => "12hour",
                TimeFormatName.TwentyFourHour => "24hour",
                _ => throw new ArgumentOutOfRangeException(nameof(name))
            };
        }

        public static string ToApiString(this TimeFormatType format)
        {
            return format switch
            {
                TimeFormatType.H12 => "h:mma",
                TimeFormatType.H24 => "HH:mm",
                _ => throw new ArgumentOutOfRangeException(nameof(format))
            };
        }

        public static string ToApiString(this DurationFormatType format)
        {
            return format switch
            {
                DurationFormatType.HMM => "h:mm",
                DurationFormatType.HMMSS => "h:mm:ss",
                DurationFormatType.HMMSSS => "h:mm:ss.S",
                DurationFormatType.HMMSSSS => "h:mm:ss.SS",
                DurationFormatType.HMMSSSSS => "h:mm:ss.SSS",
                _ => throw new ArgumentOutOfRangeException(nameof(format))
            };
        }

        public static string ToApiString(this ChoiceColor color)
        {
            return color switch
            {
                ChoiceColor.BlueLight2 => "blueLight2",
                ChoiceColor.CyanLight2 => "cyanLight2",
                ChoiceColor.TealLight2 => "tealLight2",
                ChoiceColor.GreenLight2 => "greenLight2",
                ChoiceColor.YellowLight2 => "yellowLight2",
                ChoiceColor.OrangeLight2 => "orangeLight2",
                ChoiceColor.RedLight2 => "redLight2",
                ChoiceColor.PinkLight2 => "pinkLight2",
                ChoiceColor.PurpleLight2 => "purpleLight2",
                ChoiceColor.GrayLight2 => "grayLight2",
                ChoiceColor.BlueLight1 => "blueLight1",
                ChoiceColor.CyanLight1 => "cyanLight1",
                ChoiceColor.TealLight1 => "tealLight1",
                ChoiceColor.GreenLight1 => "greenLight1",
                ChoiceColor.YellowLight1 => "yellowLight1",
                ChoiceColor.OrangeLight1 => "orangeLight1",
                ChoiceColor.RedLight1 => "redLight1",
                ChoiceColor.PinkLight1 => "pinkLight1",
                ChoiceColor.PurpleLight1 => "purpleLight1",
                ChoiceColor.GrayLight1 => "grayLight1",
                ChoiceColor.BlueBright => "blueBright",
                ChoiceColor.CyanBright => "cyanBright",
                ChoiceColor.TealBright => "tealBright",
                ChoiceColor.GreenBright => "greenBright",
                ChoiceColor.YellowBright => "yellowBright",
                ChoiceColor.OrangeBright => "orangeBright",
                ChoiceColor.RedBright => "redBright",
                ChoiceColor.PinkBright => "pinkBright",
                ChoiceColor.PurpleBright => "purpleBright",
                ChoiceColor.GrayBright => "grayBright",
                ChoiceColor.BlueDark1 => "blueDark1",
                ChoiceColor.CyanDark1 => "cyanDark1",
                ChoiceColor.TealDark1 => "tealDark1",
                ChoiceColor.GreenDark1 => "greenDark1",
                ChoiceColor.YellowDark1 => "yellowDark1",
                ChoiceColor.OrangeDark1 => "orangeDark1",
                ChoiceColor.RedDark1 => "redDark1",
                ChoiceColor.PinkDark1 => "pinkDark1",
                ChoiceColor.PurpleDark1 => "purpleDark1",
                ChoiceColor.GrayDark1 => "grayDark1",
                _ => throw new ArgumentOutOfRangeException(nameof(color))
            };
        }

        public static string ToApiString(this RatingColor color)
        {
            return color switch
            {
                RatingColor.YellowBright => "yellowBright",
                RatingColor.OrangeBright => "orangeBright",
                RatingColor.RedBright => "redBright",
                RatingColor.PinkBright => "pinkBright",
                RatingColor.PurpleBright => "purpleBright",
                RatingColor.BlueBright => "blueBright",
                RatingColor.CyanBright => "cyanBright",
                RatingColor.TealBright => "tealBright",
                RatingColor.GreenBright => "greenBright",
                RatingColor.GrayBright => "grayBright",
                _ => throw new ArgumentOutOfRangeException(nameof(color))
            };
        }

        public static string ToApiString(this RatingIcon icon)
        {
            return icon switch
            {
                RatingIcon.Star => "star",
                RatingIcon.Heart => "heart",
                RatingIcon.ThumbsUp => "thumbsUp",
                RatingIcon.Flag => "flag",
                RatingIcon.Dot => "dot",
                _ => throw new ArgumentOutOfRangeException(nameof(icon))
            };
        }
    }

    //-----------------------------------------------------------------------------------------------------------

    // Generic enum converter that uses ToApiString() extension methods
    public class ApiStringEnumConverter<T> : JsonConverter<T> where T : struct, Enum
    {
        public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException($"Expected string, got {reader.TokenType}");

            string? value = reader.GetString();
            if (string.IsNullOrEmpty(value))
                throw new JsonException("Enum value cannot be null or empty");

#if NET5_0_OR_GREATER
            // Try to find matching enum value by comparing API strings
            foreach (T enumValue in Enum.GetValues<T>())
#else
            foreach (T enumValue in (T[])Enum.GetValues(typeof(T)))
#endif
            {
                if (GetApiString(enumValue).Equals(value, StringComparison.OrdinalIgnoreCase))
                    return enumValue;
            }

            throw new JsonException($"Unable to convert '{value}' to {typeof(T).Name}");
        }

        public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(GetApiString(value));
        }

        private static string GetApiString(T enumValue)
        {
            // Use reflection to call the ToApiString() extension method
            var method = typeof(EnumExtensions).GetMethod("ToApiString", new[] { typeof(T) });
            if (method != null)
            {
                return (string)method.Invoke(null, new object[] { enumValue })!;
            }

            // Fallback to enum name if no ToApiString method exists
            return enumValue.ToString();
        }
    }

    // Specific converters for each enum type
    public class FieldTypeConverter : ApiStringEnumConverter<FieldType> { }
    public class CheckboxColorConverter : ApiStringEnumConverter<CheckboxColor> { }
    public class CheckboxIconConverter : ApiStringEnumConverter<CheckboxIcon> { }
    public class DateFormatTypeConverter : ApiStringEnumConverter<DateFormatType> { }
    public class DateFormatNameConverter : ApiStringEnumConverter<DateFormatName> { }
    public class TimeFormatTypeConverter : ApiStringEnumConverter<TimeFormatType> { }
    public class TimeFormatNameConverter : ApiStringEnumConverter<TimeFormatName> { }
    public class DurationFormatTypeConverter : ApiStringEnumConverter<DurationFormatType> { }
    public class ChoiceColorConverter : ApiStringEnumConverter<ChoiceColor> { }
    public class RatingColorConverter : ApiStringEnumConverter<RatingColor> { }
    public class RatingIconConverter : ApiStringEnumConverter<RatingIcon> { }
}
