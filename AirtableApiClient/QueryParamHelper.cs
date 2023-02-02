using System.Collections.Generic;
using System.Web;
using System.Text.Json.Serialization;

namespace AirtableApiClient
{
    public enum SortDirection
    {
        Asc,
        Desc
    }

    public class Sort           // This class is here for backward compatibility, no longer used for (De)serialization.
    {
        public string Field { get; set; }

        public SortDirection Direction { get; set; }
    }

    internal class SortWithDirectionString
    {
        [JsonPropertyName("field")]
        public string Field { get; set; }

        [JsonPropertyName("direction")]
        public string Direction { get; set; }       // Direction is now a string for the support of having the 'sort' parameter in the request body of ListRecords.
    }


    public class Fields
    {
        [JsonPropertyName("fields")]
        public IDictionary<string, object> FieldsCollection { get; set; } = new Dictionary<string, object>();

        public void AddField(string fieldName, object fieldValue)
        {
            FieldsCollection.Add(fieldName, fieldValue);
        }

        public void AddField(KeyValuePair<string, object> fld)
        {
            FieldsCollection.Add(fld);
        }
    }


    /// <summary>
    /// This class is used in the Update and Replace record(s) operations; Record ID + (Field ID/Field Name + Field value)
    /// </summary>
    public class IdFields : Fields
    {
        public IdFields(string id)
        {
            this.id = id;
        }

        // Note: System.Text.Json's Serialization includes Properties by default
        // So it's good practice to use Property instead of Fields.
        // Change 'field' to 'property' but not changing the case of 'i' in 'Id'
        // to keep backward compatiblity.
        [JsonPropertyName("id")]
        public string id { get; set; }      // Note: this is the record ID of the record containing Fields (not to be confused with the field ID)
    }


    internal class QueryParamHelper
    {
        static internal string
        FlattenSortParam(
            IEnumerable<Sort> sort)
        {
            int i = 0;
            string flattenSortParam = string.Empty;
            string toInsert = string.Empty;
            foreach (var sortItem in sort)
            {
                if (string.IsNullOrEmpty(toInsert) && i > 0)
                {
                    toInsert = "&";
                }

                // Name of fields to be sorted
                string param = $"sort[{i}][field]";
                flattenSortParam += $"{toInsert}{HttpUtility.UrlEncode(param)}={HttpUtility.UrlEncode(sortItem.Field)}";

                // Direction for sorting
                param = $"sort[{i}][direction]";
                flattenSortParam += $"&{HttpUtility.UrlEncode(param)}={HttpUtility.UrlEncode(sortItem.Direction.ToString().ToLower())}";
                i++;
            }
            return flattenSortParam;
        }


        static internal string
        FlattenFieldsParam(
            IEnumerable<string> fields)
        {
            int i = 0;
            string flattenFieldsParam = string.Empty;
            string toInsert = string.Empty;
            foreach (var fieldName in fields)
            {
                if (string.IsNullOrEmpty(toInsert) && i > 0)
                {
                    toInsert = "&";
                }
                string param = "fields[]";
                flattenFieldsParam += $"{toInsert}{HttpUtility.UrlEncode(param)}={HttpUtility.UrlEncode(fieldName)}";
                i++;
            }
            return flattenFieldsParam;
        }

    }   // end class


    internal class ListRecordsParameters
    {
        [JsonPropertyName("offset")]
        [JsonInclude]
        public string Offset { get; set; }

        [JsonPropertyName("fields")]
        [JsonInclude]
        public string[] Fields { get; set; }

        [JsonPropertyName("filterByFormula")]
        [JsonInclude]
        public string FilterByFormula { get; set; }

        [JsonPropertyName("maxRecords")]
        [JsonInclude]
        public int? MaxRecords { get; set; }

        [JsonPropertyName("pageSize")]
        [JsonInclude]
        public int? PageSize { get; set; }

        [JsonPropertyName("sort")]
        [JsonInclude]
        public List<SortWithDirectionString> Sort { get; set; }
        //public Dictionary<string, string> Sort { get; set; }   

        [JsonPropertyName("view")]
        [JsonInclude]
        public string View { get; set; }

        [JsonPropertyName("cellFormat")]
        [JsonInclude]
        public string CellFormat { get; set; }

        [JsonPropertyName("timeZone")]
        [JsonInclude]
        public string TimeZone { get; set; }

        [JsonPropertyName("userLocale")]
        [JsonInclude]
        public string UserLocale { get; set; }

        [JsonPropertyName("returnFieldsByFieldId")]
        [JsonInclude]
        public bool ReturnFieldsByFieldId { get; set; }
    }


    internal class UpSertRecordsParameters
    {
        [JsonPropertyName("performUpsert")]
        [JsonInclude]
        public PerformUpsert PerformUpsert { get; set; }

        [JsonPropertyName("returnFieldsByFieldId")]
        [JsonInclude]
        public bool ReturnFieldsByFieldId { get; set; }

        [JsonPropertyName("typecast")]
        [JsonInclude]
        public bool Typecast { get; set; }

        [JsonPropertyName("records")]
        [JsonInclude]
        public IdFields[] Records { get; internal set; }
    }

    public class PerformUpsert
    {
        [JsonPropertyName("fieldsToMergeOn")]
        [JsonInclude]
        public string[] FieldsToMergeOn { get; set; }
    }

    public class UserIdAndScopes
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("scopes")]
        public IEnumerable<string> Scopes { get; set; }
    }
}   // end namespace 
