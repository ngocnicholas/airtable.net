using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;

namespace AirtableApiClient
{
    public enum SortDirection
    {
        Asc,
        Desc
    }


    public class Sort
    {
        [JsonProperty("fields")]
        public string Field { get; set; }

        [JsonProperty("direction")]
        public SortDirection Direction { get; set; }
    }


    public class Fields
    {
        [JsonProperty("fields")]
        public Dictionary<string, object> FieldsCollection { get; set; } = new Dictionary<string, object>();

        public void AddField(string fieldName, object fieldValue)
        {
            FieldsCollection.Add(fieldName, fieldValue);
        }
    }


    public class IdFields : Fields
    {
        public IdFields(string id)
        {
            this.id = id;
        }

        [JsonProperty("id")]
        public string id;
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
}   // end namespace 
