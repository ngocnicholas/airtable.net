using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public enum FieldTypeEnum
    {
        singleLineText, email, url, multilineText, number, 
        percent, currency, singleSelect, multipleSelects, singleCollaborator, 
        multipleCollaborators, multipleRecordLinks, date, dateTime, phoneNumber, 
        multipleAttachments, checkbox, formula, createdTime, rollup, 
        count, lookup, multipleLookupValues, autoNumber, barcode, 
        rating, richText, duration, lastModifiedTime, button, 
        createdBy, lastModifiedBy, externalSyncSource, aiText
    }


    /// <summary>
    /// Barcode field
    /// </summary>
    public class BarcodeField : FieldModel                     // TESTED, I just entered a number. Verify with Emmett
    {
        public BarcodeField()
        {
            Type = "barcode";
        }
    }



    /// <summary>
    /// Checkbox field
    /// </summary>
    public class CheckboxField : FieldModelOptions<CheckboxOptions> 
    {
        public CheckboxField()
        {
            Type = "checkbox";
        }
    }

    public class CheckboxOptions  
    {
        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }    // "check" | "xCheckbox" | "star" | "heart" | "thumbsUp" | "flag" | "dot"
    }

    /// <summary>
    /// Collaborator field
    /// </summary>
    public class CollaboratorField : FieldModel  
    {
        public CollaboratorField()
        {
            Type = "singleCollaborator";
        }
    }


    /// <summary>
    /// Currency field
    /// </summary>
    public class CurrencyField : FieldModelOptions<CurrencyOptions>  
    {
        public CurrencyField()
        {
            Type = "currency";
        }
    }

    public class CurrencyOptions 
    {
        [JsonPropertyName("precision")]
        public int Precision { get; set; }

        [JsonPropertyName("symbol")]
        public string Symbol { get; set; }
    }


    /// <summary>
    /// Date field
    /// </summary>
    public class DateField : FieldModelOptions<DateOptions> 
    {
        public DateField()
        {
            Type = "date";
        }
    }

    public class DateOptions 
    {
        [JsonPropertyName("dateFormat")]
        public DateFormat DateFormat { get; set; }
    }

    public class DateFormat  
    {

        [JsonPropertyName("format")]        // "l" | "LL" | "M/D/YYYY" | "D/M/YYYY" | "YYYY-MM-DD"
                                            // format is always provided when reading.
                                            // (l for local, LL for friendly, M/D/YYYY for us, D/M/YYYY for european, YYYY-MM-DD for iso)
        public string Format { get; set; }  // Format is optional when writing, but it must match the corresponding name if provided.

        [JsonPropertyName("name")]
        public string Name { get; set; }    // "local" | "friend" | "us" | "european" | "iso" 
    }


    /// <summary>
    /// Date and time field
    /// </summary>
    public class DateTimeField : FieldModelOptions<DateTimeOptions>          // for Read and for Write 
    {
        public DateTimeField()
        {
            Type = "dateTime";
        }
    }

    public class DateTimeOptions
    { 
        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; }        // See https://airtable.com/developers/web/api/model/timezone for details

        [JsonPropertyName("dateFormat")]
        public DateFormat DateFormat { get; set; }      // "h:mma" | "HH:mm"

        [JsonPropertyName("timeFormat")]
        public TimeFormat TimeFormat { get; set; }      // "h:mma" | "HH:mm"
    }

    public class TimeFormat 
    {
        [JsonPropertyName("format")]                        //format is always provided when reading. (l for local, LL for friendly, M/D/YYYY for us, D/M/YYYY for european, YYYY-MM-DD for iso)
        public string Format { get; set; }                  // 	Optional for Write. "h:mma" | "HH:mm"

        [JsonPropertyName("name")]
        public string Name { get; set; }                    // "12hour" | "24hour"
    }


    /// <summary>
    /// Duration field
    /// </summary>
    public class DurationField : FieldModelOptions<DurationOptions>  
    {
        public DurationField()
        {
            Type = "duration";
        }
    }

    public class DurationOptions  
    {
        [JsonPropertyName("durationFormat")]
        public string DurationFormat { get; set; } //"h:mm" | "h:mm:ss" | "h:mm:ss.S" | "h:mm:ss.SS" | "h:mm:ss.SSS"
    }

    /// <summary>
    /// Email field
    /// </summary>
    public class EmailField : FieldModel 
    {
        public EmailField()
        {
            Type = "email";
        }
    }


    /// <summary>
    /// Long text field (multi-line)
    /// </summary>    
    public class LongTextField : FieldModel
    {
        public LongTextField()
        {
            Type = "multilineText";
        }
    }


    public class MultipleCollaboratorField : FieldModel 
    {
        public MultipleCollaboratorField()
        {
            Type = "multipleCollaborators";
        }
    }


 
    /// <summary>
    /// Number field
    /// </summary>
    public class NumberField : FieldModelOptions<PrecisionOptions> 
    {
        public NumberField()
        {
            Type = "number";
        }
    }

    public class PrecisionOptions 
    {
        [JsonPropertyName("precision")]
        public int Precision { get; set; }  // Indicates the number of digits shown to the right of the decimal point for this field. (0-8 inclusive)
    }


    /// <summary>
    /// Percent field
    /// </summary>
    public class PercentField : FieldModelOptions<PrecisionOptions> 
    {
        public PercentField()
        {
            Type = "percent";
        }
    }

    /// <summary>
    /// Phone number field
    /// </summary>
    public class PhoneNumberField : FieldModel 
    {
        public PhoneNumberField()
        {
            Type = "phoneNumber";
        }
    }

    /// <summary>
    /// Rating field
    /// </summary>
    public class RatingField : FieldModelOptions<RatingOptions> 
    {
        public RatingField()
        {
            Type = "rating";
        }

    }

    public class RatingOptions 
    {
        [JsonPropertyName("color")]
        public string Color { get; set; }       // TODO: make an EMUM for this

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonPropertyName("max")]
        public int Max { get; set; }            // The maximum value for the rating, from 1 to 10 inclusive.
    }


    /// <summary>
    /// Rich text field with formatting
    /// </summary>
    public class RichTextField : FieldModel 
    {
        public RichTextField()
        {
            Type = "richText";
        }
    }


    /// <summary>
    /// Single line text field
    /// </summary>
    public class SingleLineTextField : FieldModel 
    {
        public SingleLineTextField()
        {
            Type = "singleLineText";
        }
    }


    /// <summary>
    /// Single select field
    /// </summary>
    public class SingleSelectField : FieldModelOptions<ChoiceOptions>
    { 
        public SingleSelectField()
        {
            Type = "singleSelect";
        }
    }


    public class SyncSourceField: FieldModelOptions<ChoiceOptions>
    {
        public SyncSourceField()
        {
            Type = "externalSyncSource";
        }
    }


    /// <summary>
    /// URL field
    /// </summary>
    public class UrlField : FieldModel 
    {
        public UrlField()
        {
            Type = "url";
        }
    }
}