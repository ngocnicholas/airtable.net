using System.Collections.Generic;
using System.Collections.Specialized;
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


    internal class QueryParamHelper
    { 
        static internal void 
        FlattenSortParam(
            IEnumerable<Sort> sort, 
            NameValueCollection query)
        {
            int i = 0;
            foreach (var sortItem in sort)
            {
                query["sort[" + i + "][field]"] = sortItem.Field;                               // name of fields to be sorted
                query["sort[" + i + "][direction]"] = sortItem.Direction.ToString().ToLower();  // direction for sorting
                i++;
            }
        }


        static internal void
        FlattenFieldsParam(
            IEnumerable<string> fields, 
            NameValueCollection query)
        {
            int i = 0;
            foreach (var fieldName in fields)
            {
                query["fields[" + i + "]"] = fieldName;
                i++;
            }
        }

    }

}
