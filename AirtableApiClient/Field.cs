using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    // public interfaces can serve as markers since C# does not allow multiple inheritance of classes
    // but a class can inherite from multiple interfaces.
    public interface IFieldConfig { }   // for marking the WRITE fields, to be used in CreateBase

    //------------------------------------------------------------------

    public abstract class Field
    {
        [JsonPropertyName("id")][JsonInclude] 
        public string? Id { get; set; }

        [JsonPropertyName("description")][JsonInclude] 
        public string? Description { get; set; }

        [JsonPropertyName("type")]
        [JsonConverter(typeof(FieldEnumConverter))]
        [JsonInclude]
        public FieldEnum Type { get; set; }

        [JsonPropertyName("name")][JsonInclude] 
        public string? Name { get; set; }
    }

    //---------------------------------------------------------------------------

    public abstract class FieldConfig : Field, IFieldConfig { }

    // Generic base for write-options fields
    public abstract class FieldConfig<TConfigOptions> : FieldConfig
    {
        [JsonPropertyName("options")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TConfigOptions? ConfigOptions { get; set; }
    }

    //-------------------------------------------------------------------------
    public abstract class FieldModel : Field { }

    // Generic base for read-options fields
    public abstract class FieldModel<TModelOptions> : FieldModel
    {
        [JsonPropertyName("options")]
        public TModelOptions? ReadOptions { get; set; }
    }

    //-------------------------------------------------------------------------

    public sealed class AiTextFieldModel : FieldModel<AiTextReadOptions>       // R only, with options
    { 
        public AiTextFieldModel() { Type = FieldEnum.AiText; } 
    }

    public class AiTextReadOptions
    {
        [JsonPropertyName("prompt")]
        [JsonConverter(typeof(PromptItemListConverter))]    // attribute for its custom converter
        public List<PromptItem>? Prompt { get; set; }       // heterogeneous types: optional<array of (strings | objects of the class PromptItem)>

        [JsonPropertyName("referencedFieldIds")]
        public List<string>? ReferencedFieldIds { get; set; }   // A list of Field IDs referred to in all the PromptItems of Prompt.
                                                                // In the example below, the field ID of "Name" will be in this List.
    }

    public class FieldReference
    {
        [JsonPropertyName("fieldId")]
        public string? FieldId { get; set; }
    }

    public class PromptItem
    {
        /*
         * Example of a prompt which has three PromptItems:
         * CEO of [field ID of "Name"] Return the first, middle, and last name.
         * Prompt[0] has TextContent = "CEO of"
         * Prompt[1] has the FieldRef = field ID of "Name"
         * Prompt[2] has TextContent = "Return the first, middle, and last name."
         * */
        [JsonPropertyName("textContent")]
        public string? TextContent { get; set; }

        [JsonPropertyName("field")]
        public FieldReference? Field { get; set; }

        public static PromptItem FromText(string text) => new PromptItem { TextContent = text };
        public static PromptItem FromField(string fieldId) => new PromptItem { Field = new FieldReference { FieldId = fieldId } };
    }

    //-------------------------- end of AiTextField


    // Attachment RW, but has read options only
    public sealed class AttachmentFieldModel : FieldModel<AttachmentModelOptions>
    {
        public AttachmentFieldModel() => Type = FieldEnum.MultipleAttachments;
    }

    public sealed class AttachmentModelOptions
    {
        [JsonPropertyName("isReversed")]
        public bool IsReversed { get; set; }
    }

    // WRITE shape (no options allowed in payload)
    public sealed class AttachmentFieldConfig : FieldConfig
    {
        public AttachmentFieldConfig() => Type = FieldEnum.MultipleAttachments;
    }

    /// <summary>
    /// Auto number field
    /// </summary>
    public class AutoNumberFieldModel : FieldModel
    {
        public AutoNumberFieldModel()
        {
            Type = FieldEnum.AutoNumber;
        }
    }


    /// <summary>
    /// Barcode field
    /// </summary>
    public class BarcodeFieldModel : FieldModel
    {
        public BarcodeFieldModel()
        {
            Type = FieldEnum.Barcode;
        }
    }

    public class BarcodeFieldConfig : FieldConfig
    {
        public BarcodeFieldConfig()
        {
            Type = FieldEnum.Barcode;
        }
    }


    /// <summary>
    /// Button field
    /// </summary>
    public class ButtonFieldModel : FieldModel
    {
        public ButtonFieldModel()
        {
            Type = FieldEnum.Button;
        }
    }

    // Checkbox is RW, with same options
    public class CheckboxFieldModel : FieldModel<CheckboxModelOptions> 
    {
        public CheckboxFieldModel()
        {
            Type = FieldEnum.Checkbox;
        }
    }

    public class CheckboxFieldConfig : FieldConfig<CheckboxConfigOptions> 
    {
        public CheckboxFieldConfig()
        {
            Type = FieldEnum.Checkbox;
        }
    }
    public class CheckboxModelOptions
    {
        [JsonPropertyName("color")]
        public string? Color { get; set; }

        [JsonPropertyName("icon")]
        public string? Icon { get; set; }    // "check" | "xCheckbox" | "star" | "heart" | "thumbsUp" | "flag" | "dot"
    }

    public class CheckboxConfigOptions : CheckboxModelOptions { }

    /// <summary>
    /// Collaborator field
    /// </summary>
    public class CollaboratorFieldModel : FieldModel
    {
        public CollaboratorFieldModel()
        {
            Type = FieldEnum.SingleCollaborator;
        }
    }

    public class CollaboratorFieldConfig : FieldConfig
    {
        public CollaboratorFieldConfig()
        {
            Type = FieldEnum.SingleCollaborator;
        }
    }
    /// <summary>
    /// Count field
    /// </summary>
    public class CountFieldModel : FieldModel<CountModelOptions>
    {
        public CountFieldModel()
        {
            Type = FieldEnum.Count;
        }
    }

    public class CountModelOptions
    {
        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }   // false when recordLinkFieldId is null, e.g. the referenced column was deleted.

        [JsonPropertyName("recordLinkFieldId")]
        public string? RecordLinkFieldId { get; set; }   // optional<string | null >
    }

    /// <summary>
    /// Created By field
    /// </summary>
    public class CreatedByFieldModel : FieldModel
    {
        public CreatedByFieldModel()
        {
            Type = FieldEnum.CreatedBy;
        }
    }

    /// <summary>
    /// Created time field
    /// </summary>
    public class CreatedTimeFieldModel : FieldModel<CreatedTimeModelOptions>
    {
        public CreatedTimeFieldModel()
        {
            Type = FieldEnum.CreatedTime;
        }
    }

    public class CreatedTimeModelOptions
    {
        [JsonPropertyName("result")]
        public FieldModel? Result { get; set; } //optional but always be either DateField or DateTimeField

    }

    /// <summary>
    /// Currency field
    /// </summary>
    public class CurrencyFieldModel : FieldModel<CurrencyModelOptions>
    {
        public CurrencyFieldModel()
        {
            Type = FieldEnum.Currency;
        }
    }

    public class CurrencyFieldConfig : FieldConfig<CurrencyConfigOptions>
    {
        public CurrencyFieldConfig()
        {
            Type = FieldEnum.Currency;
        }
    }

    public class CurrencyModelOptions
    {
        private int _precision;

        [JsonPropertyName("precision")]
        public int Precision
        {
            get => _precision;
            set => _precision = value < 0 ? 0 : value > 7 ? 7 : value;  // Indicates the number of digits shown to right of the decimal point (0-7 inclusive)
        }

        [JsonPropertyName("symbol")]
        public string? Symbol { get; set; }
    }

    public class CurrencyConfigOptions : CurrencyModelOptions { }

    //-----------------------------------------

    /// <summary>
    /// Date field
    /// </summary>
    public class DateFieldModel : FieldModel<DateModelOptions>  // RW, with Options but Write's format is optional
    {
        public DateFieldModel()
        {
            Type = FieldEnum.Date;
        }
    }
    public class DateFieldConfig : FieldConfig<DateConfigOptions>  // RW, with Options but Write's format is optional
    {
        public DateFieldConfig()
        {
            Type = FieldEnum.Date;
        }
    }

    public class DateModelOptions
    {
        [JsonPropertyName("dateFormat")]
        public DateFormat? DateFormat { get; set; }
    }

    public class DateConfigOptions : DateModelOptions { }

    //---------------------------

    public class DateFormat
    {

        [JsonPropertyName("format")]        // "l" | "LL" | "M/D/YYYY" | "D/M/YYYY" | "YYYY-MM-DD"
                                            // format is always provided when reading.
                                            // (l for local, LL for friendly, M/D/YYYY for us, D/M/YYYY for european, YYYY-MM-DD for iso)
        public string? Format { get; set; }  // Format is optional when writing, but it must match the corresponding name if provided.

        [JsonPropertyName("name")]
        public string? Name { get; set; }    // "local" | "friend" | "us" | "european" | "iso" 
    }


    /// <summary>
    /// Date and time field
    /// RW, with Options but Write's format is optional
    /// </summary>
    public class DateTimeField : FieldModel<DateTimeModelOptions> 
    {
        public DateTimeField()
        {
            Type = FieldEnum.DateTime;
        }
    }
    public class DateTimeFieldConfig : FieldConfig<DateTimeConfigOptions> 
    {
        public DateTimeFieldConfig()
        {
            Type = FieldEnum.DateTime;
        }
    }

    public class DateTimeModelOptions
    {
        [JsonPropertyName("timeZone")]
        public string? TimeZone { get; set; }        // See https://airtable.com/developers/web/api/model/timezone for details

        [JsonPropertyName("dateFormat")]
        public DateFormat? DateFormat { get; set; }      // "h:mma" | "HH:mm"

        [JsonPropertyName("timeFormat")]
        public TimeFormat? TimeFormat { get; set; }      // "h:mma" | "HH:mm"
    }

    public class DateTimeConfigOptions : DateTimeModelOptions { }

    public class TimeFormat
    {
        [JsonPropertyName("format")]                        //format is always provided when reading. (l for local, LL for friendly, M/D/YYYY for us, D/M/YYYY for european, YYYY-MM-DD for iso)
        public string? Format { get; set; }                 // 	Optional for Write. "h:mma" | "HH:mm"

        [JsonPropertyName("name")]
        public string? Name { get; set; }                   // "12hour" | "24hour"
    }


    /// <summary>
    /// Duration field
    /// </summary>
    public class DurationFieldModel : FieldModel<DurationModelOptions>
    {
        public DurationFieldModel()
        {
            Type = FieldEnum.Duration;
        }
    }

    public class DurationFieldConfig : FieldConfig<DurationConfigOptions>
    {
        public DurationFieldConfig()
        {
            Type = FieldEnum.Duration;
        }
    }

    public class DurationModelOptions
    {
        [JsonPropertyName("durationFormat")]
        public string? DurationFormat { get; set; } //"h:mm" | "h:mm:ss" | "h:mm:ss.S" | "h:mm:ss.SS" | "h:mm:ss.SSS"
    }
    public class DurationConfigOptions : DurationModelOptions { }

    /// <summary>
    /// Email field
    /// </summary>
    public class EmailFieldModel : FieldModel
    {
        public EmailFieldModel()
        {
            Type = FieldEnum.Email;
        }
    }

    public class EmailFieldConfig : FieldConfig
    {
        public EmailFieldConfig()
        {
            Type = FieldEnum.Email;
        }
    }
    /// <summary>
    /// Formula field
    /// </summary>
    public class FormulaFieldModel : FieldModel<FormulaModelOptions>
    {
        public FormulaFieldModel()
        {
            Type = FieldEnum.Formula;
        }
    }

    public class FormulaModelOptions
    {
        //The formula including fields referenced by their IDs.For example, LEFT(4, { Birthday}) in the Airtable.com formula editor
        //will be returned as LEFT(4, { fldXXX}) via API.
        [JsonPropertyName("formula")]
        public string? Formula { get; set; }

        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("referencedFieldIds")]
        public string[]? ReferencedFieldIds { get; set; }       // array of strings | null
                                                                // All fields in the record that are used in the formula.

        [JsonPropertyName("result")]
        public FieldModel? Result { get; set; }
    }

    /// <summary>
    /// Represents information about who last modified the record.
    /// </summary>
    public class LastModifiedByFieldModel : FieldModel  // Read only, no options
    {
        public LastModifiedByFieldModel()
        {
            Type = FieldEnum.LastModifiedBy;
        }
    }

    //-----------------------------------------------------------------------------
    /// <summary>
    /// Last modified time field
    /// </summary>
    public class LastModifiedTimeFieldModel : FieldModel<LastModifiedTimeModelOptions>
    {
        public LastModifiedTimeFieldModel()
        {
            Type = FieldEnum.LastModifiedTime;
        }
    }

    public class LastModifiedTimeModelOptions
    {
        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("referencedFieldIds")]
        public string[]? ReferencedFieldIds { get; set; }       // array of strings | null
                                                                // All fields in the record that are used in the formula.
        [JsonPropertyName("result")]
        public FieldModel? Result { get; set; }      // null | Always be a DateField or DateFielField
    }

    //---------------------------------------------------------------------------

    /// <summary>
    /// Link to another record field
    /// </summary>
    /// <summary>
    /// Link to another record field
    /// </summary>
    public class LinkToAnotherRecordFieldModel : FieldModel<LinkToAnotherRecordModelOptions>
    {
        // Creating "multipleRecordLinks" fields is supported but updating options for existing "multipleRecordLinks" fields is not supported.
        public LinkToAnotherRecordFieldModel()
        {
            Type = FieldEnum.MultipleRecordLinks;
        }
    }

    public class LinkToAnotherRecordModelOptions
    {
        // Note: All properties are in the READ situations
        [JsonPropertyName("isReversed")]
        public bool IsReversed { get; set; }

        [JsonPropertyName("prefersSingleRecordLink")]
        public bool PrefersSingleRecordLink { get; set; }

        [JsonPropertyName("inverseLinkFieldId")]
        public string? InverseLinkFieldId { get; set; } // Optional. The ID of the field in the linked table that links back to this one.

        [JsonPropertyName("linkedTableId")]
        public string? LinkedTableId { get; set; }   // The ID of the table this field links to

        [JsonPropertyName("viewIdForRecordSelection")]
        public string? ViewIdForRecordSelection { get; set; } // Optional. The ID of the view in the linked table to use when showing a list of records to select from.    }

    }

    //......................................................

    public class LinkToAnotherRecordFieldConfig : FieldConfig<LinkToAnotherRecordConfigOptions>
    {
        // Creating "multipleRecordLinks" fields is supported but updating options for existing "multipleRecordLinks" fields is not supported.
        public LinkToAnotherRecordFieldConfig()
        {
            Type = FieldEnum.MultipleRecordLinks;
        }
    }

    public class LinkToAnotherRecordConfigOptions
    {
        // Note: All properties are in the Write situations
        // Note: Only the 2 properties below are in the CreateBase situation, not even in the UpdateBase situation)
        [JsonPropertyName("linkedTableId")]
        public string? LinkedTableId { get; set; }   // The ID of the table this field links to

        [JsonPropertyName("viewIdForRecordSelection")]
        public string? ViewIdForRecordSelection { get; set; } // Optional. The ID of the view in the linked table to use when showing a list of records to select from.    }

    }
    //--------------------------------------------------------------------------------------------

    /// <summary>
    /// Long text field (multi-line)
    /// </summary>    
    public class LongTextFieldModel : FieldModel
    {
        public LongTextFieldModel()
        {
            Type = FieldEnum.MultilineText;
        }
    }

    public class LongTextFieldConfig : FieldConfig
    {
        public LongTextFieldConfig()
        {
            Type = FieldEnum.MultilineText;
        }
    }

    //--------------------------------------------------------------------------------------
    /// <summary>
    /// Lookup field
    /// </summary>
    public class LookupFieldModel : FieldModel<LookupModelOptions>
    {
        public LookupFieldModel()
        {
            Type = FieldEnum.MultipleLookupValues;
        }
    }

    public class LookupModelOptions
    {
        [JsonPropertyName("fieldIdInLinkedTable")]
        public string? FieldIdInLinkedTable { get; set; }    // may be null

        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("recordLinkFieldId")]
        public string? RecordLinkFieldId { get; set; }       // may be null

        [JsonPropertyName("result")]
        public FieldModel? Result { get; set; } 

    }

    //--------------------------------------------------------------------------------

    public class MultipleCollaboratorFieldModel : FieldModel   // NO options, spec is wrong
    {
        public MultipleCollaboratorFieldModel()
        {
            Type = FieldEnum.MultipleCollaborators;
        }
    }

    public class MultipleCollaboratorFieldConfig : FieldConfig   //  NO options, spec is wrong
    {
        public MultipleCollaboratorFieldConfig()
        {
            Type = FieldEnum.MultipleCollaborators;
        }
    }

    /// <summary>
    /// Multi-select field
    /// </summary>
    public class MultipleSelectFieldModel : FieldModel<ChoiceModelOptions>  // RW with options but the Write's option is more flexible
    {
        public MultipleSelectFieldModel()
        {
            Type = FieldEnum.MultipleSelects;
        }
    }
    public class MultipleSelectFieldConfig : FieldConfig<ChoiceConfigOptions>  // RW with options but the Write's option is more flexible
    {
        public MultipleSelectFieldConfig()
        {
            Type = FieldEnum.MultipleSelects;
        }
    }

    public class ChoiceModelOptions
    {
        [JsonPropertyName("choices")]
        public List<Choice>? Choices { get; set; }
    }
    public class ChoiceConfigOptions : ChoiceModelOptions { }

    public class Choice
    {
        [JsonPropertyName("id")]                    // opional only during Write but is always required for SyncSource
                                                    // This is not specified when creating new options,
                                                    // useful when specifing existing options (for example: reordering options,
                                                    // keeping old options and adding new ones, etc)
        public string? Id { get; set; }

        [JsonPropertyName("color")]
        public string? Color { get; set; }          // optional during Read when the select field is configured to not use colors.           
                                                    // optional duiring Write - This is not specified when creating new options,
                                                    // useful when specifing existing options (for example: reordering options,
                                                    // keeping old options and adding new ones, etc)
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    //----------------------------------------------------------------------

    /// <summary>
    /// Number field
    /// </summary>
    public class NumberFieldModel : FieldModel<PrecisionModelOptions> 
    {
        public NumberFieldModel()
        {
            Type = FieldEnum.Number;
        }
    }

    public class NumberFieldConfig : FieldConfig<PrecisionConfigOptions>
    {
        public NumberFieldConfig()
        {
            Type = FieldEnum.Number;
        }
    }

    public class PrecisionModelOptions 
    {
        private int _precision;

        [JsonPropertyName("precision")]
        public int Precision                // Indicates the number of digits shown to the right of the decimal point for this field. (0-8 inclusive)
        {
            get => _precision;
            set => _precision = value < 0 ? 0 : value > 8 ? 8 : value;  // 0-8 inclusive per API
        }
    }
    public class PrecisionConfigOptions : PrecisionModelOptions { }

    /// <summary>
    /// Percent field
    /// </summary>
    public class PercentFieldModel : FieldModel<PrecisionModelOptions> 
    {
        public PercentFieldModel()
        {
            Type = FieldEnum.Percent;
        }
    }

    public class PercentFieldConfig : FieldConfig<PrecisionConfigOptions>
    {
        public PercentFieldConfig()
        {
            Type = FieldEnum.Percent;
        }
    }
    //---------------------------------------------

    /// <summary>
    /// Phone number field
    /// </summary>
    public class PhoneFieldModel : FieldModel 
    {
        public PhoneFieldModel()
        {
            Type = FieldEnum.PhoneNumber;
        }
    }

    public class PhoneFieldConfig : FieldConfig
    {
        public PhoneFieldConfig()
        {
            Type = FieldEnum.PhoneNumber;
        }
    }
    //-----------------------------

    /// <summary>
    /// Rating field
    /// </summary>
    public class RatingFieldModel : FieldModel<RatingFieldReadOptions> 
    {
        public RatingFieldModel()
        {
            Type = FieldEnum.Rating;
        }

    }
    public class RatingFieldConfig : FieldConfig<RatingConfigOptions>
    {
        public RatingFieldConfig()
        {
            Type = FieldEnum.Rating;
        }

    }

    public class RatingFieldReadOptions
    {
        [JsonPropertyName("color")]
        public string? Color { get; set; }
        [JsonPropertyName("icon")]
        public string? Icon { get; set; }

        private int _max;

        [JsonPropertyName("max")]
        public int Max
        {
            get => _max;
            set => _max = value < 1 ? 1 : value > 10 ? 10 : value;  // The maximum value for the rating, from 1 to 10 inclusive.
        }
    }

    public class RatingConfigOptions : RatingFieldReadOptions { }

    //--------------------------------------------

    /// <summary>
    /// Rich text field with formatting
    /// </summary>
    public class RichTextFieldModel : FieldModel
    {
        public RichTextFieldModel()
        {
            Type = FieldEnum.RichText;
        }
    }

    public class RichTextFieldConfig : FieldConfig
    {
        public RichTextFieldConfig()
        {
            Type = FieldEnum.RichText;
        }
    }

    //-----------------------------------------------

    /// <summary>
    /// Rollup field
    /// </summary>
    public class RollupFieldModel : FieldModel<RollupModelOptions>           // NEED EMMETT"S HELP in TESTING this field
    {
        public RollupFieldModel()
        {
            Type = FieldEnum.Rollup;
        }
    }

    public class RollupModelOptions
    {
        [JsonPropertyName("fieldIdInLinkedTable")]
        public string? FieldIdInLinkedTable { get; set; }    // optional

        [JsonPropertyName("recordLinkFieldId")]
        public string? RecordLinkFieldId { get; set; }       // optional

        [JsonPropertyName("result")]                        // optional<FieldModel | null> Can be null is invalid
        public FieldModel? Result { get; set; }            

        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }                   // valid

        [JsonPropertyName("referencedFieldIds")]
        public string[]? ReferencedFieldIds { get; set; }    // optional
    }

    //-----------------------------------------

    /// <summary>
    /// Single line text field
    /// </summary>
    public sealed class SingleLineTextFieldModel : FieldModel   // Read and Write but no options
    {
        public SingleLineTextFieldModel()
        {
            Type = FieldEnum.SingleLineText;
        }
    }

    public sealed class SingleLineTextFieldConfig : FieldConfig   // Read and Write but no options
    {
        public SingleLineTextFieldConfig()
        {
            Type = FieldEnum.SingleLineText;
        }
    }

    //-------------------------------------

    /// <summary>
    /// Single select field
    /// </summary>
    public class SingleSelectFieldModel : FieldModel<ChoiceModelOptions>
    { 
        public SingleSelectFieldModel()
        {
            Type = FieldEnum.SingleSelect;
        }
    }

    public class SingleSelectFieldConfig : FieldConfig<ChoiceConfigOptions>
    {
        public SingleSelectFieldConfig()
        {
            Type = FieldEnum.SingleSelect;
        }
    }
    //---------------------------------------

    public class SyncSourceFieldModel : FieldModel<ChoiceModelOptions>       // Should be Read only according to feedback from Airtable
    {
        public SyncSourceFieldModel()
        {
            Type = FieldEnum.ExternalSyncSource;
        }
    }

    //------------------------------------

    /// <summary>
    /// URL field
    /// </summary>
    public class UrlFieldModel : FieldModel
    {
        public UrlFieldModel()
        {
            Type = FieldEnum.Url;
        }
    }

    public class UrlFieldConfig : FieldConfig
    {
        public UrlFieldConfig()
        {
            Type = FieldEnum.Url;
        }
    }

    //---------------------------------------------

    public class UnknownFieldModel : FieldModel
    {
        public UnknownFieldModel()
        {
            Type = FieldEnum.UnknownField;
        }

        // The discriminator we saw (could be null/missing)
        public string? UnknownType { get; set; }

        // Raw JSON of the entire field object so we can re-emit it exactly
        public string FieldModelRawJson { get; set; } = "";

        // Optional convenience: parsed "options" if present
        public string? OptionsRawJson { get; set; }
    }

    // Pick one of the below.  We don't need both
#if true    
    ///
    /// Extension methods (ergonomic, discoverable)
    /// Usage:
    /// var opts = tables[0].Fields[18].RequireOptions<LookupModelOptions>();
    /// or 
    /// if (tables[0].Fields[18].TryGetOptions<LookupModelOptions>(out var o)) { use o }
    /// 

    public static class FieldModelExtensions    // Extension methods (ergonomic, discoverable)
    {
        public static TOpts RequireOptions<TOpts>(this FieldModel f) where TOpts : class
        {
            if (f is FieldModel<TOpts> m && m.ReadOptions is { } o) return o;
            throw new InvalidOperationException($"Not a {typeof(TOpts).Name} field or options missing.");
        }

        public static bool TryGetOptions<TOpts>(this FieldModel f, out TOpts? options) where TOpts : class
        {
            if (f is FieldModel<TOpts> m && m.ReadOptions is { } o) { options = o; return true; }
            options = null; return false;
        }
    }
#else   
    ///
    /// Static helper on FieldModel (simple)
    /// Usage:
    /// var opts = FieldModel.RequireOptions<LookupModelOptions>(tables[0].Fields[18]);
    /// 

    public abstract class FieldModel : FieldBase, IFieldModel
    {
        public static TOpts RequireOptions<TOpts>(FieldModel f) where TOpts : class
        {
            if (f is FieldModel<TOpts> m && m.ReadOptions is { } o) return o;
            throw new InvalidOperationException($"Not a {typeof(TOpts).Name} field or options missing.");
        }
    }
#endif
}