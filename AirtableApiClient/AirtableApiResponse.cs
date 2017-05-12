using System.Collections.Generic;

namespace AirtableApiClient
{
    public abstract class AirtableApiResponse
    {
        protected AirtableApiResponse()
        {
            Success = true;
            AirtableApiError = null;
        }

        protected AirtableApiResponse(AirtableApiException error)
        {
            Success = false;
            AirtableApiError = error;
        }

        public readonly bool Success;
        public readonly AirtableApiException AirtableApiError;
    }


    public class AirtableListRecordsResponse : AirtableApiResponse
    {
        public AirtableListRecordsResponse(AirtableApiException error) : base(error)
        {
            Offset = null;
            Records = null;
        }


        public AirtableListRecordsResponse(AirtableRecordList recordList) : base()
        {
            Offset = recordList.Offset;
            Records = recordList.Records;
        }

        public readonly IEnumerable<AirtableRecord> Records;
        public readonly string Offset;
    }


    public class AirtableRetrieveRecordResponse : AirtableApiResponse
    {
        public AirtableRetrieveRecordResponse(AirtableApiException error) : base(error)
        {
            Record = null;
        }

        public AirtableRetrieveRecordResponse(AirtableRecord record) : base()
        {
            Record = record;
        }

        public readonly AirtableRecord Record;
    }


    public class AirtableCreateUpdateReplaceRecordResponse : AirtableApiResponse
    {
        public AirtableCreateUpdateReplaceRecordResponse(AirtableApiException error) : base(error)
        {
            Record = null;
        }

        public AirtableCreateUpdateReplaceRecordResponse(AirtableRecord record) : base()
        {
            Record = record;
        }

        public readonly AirtableRecord Record;
    }


    public class AirtableDeleteRecordResponse : AirtableApiResponse
    {
        public AirtableDeleteRecordResponse(AirtableApiException error) : base(error)
        {
            Deleted = false;
            Id = null;
        }

        public AirtableDeleteRecordResponse(bool deleted, string id) : base()
        {
            Deleted = deleted;
            Id = id;
        }

        public readonly bool Deleted;
        public readonly string Id;
    }
}
