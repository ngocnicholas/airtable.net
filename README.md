# Airtable .NET API Client

Coming soon

# Installation

Coming soon

# Quickstart

Example demonstrating usage of the API to list records:

----------------------

```

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AirtableApiClient;

readonly string baseId = YOUR_BASE_ID;
readonly string appKey = YOUR_APP_KEY;

```

----------------------


```

    string offset = null;
    string errorMessage = null;
    var records = new List<AirtableRecord>();

    using (AirtableBase airtableBase = new AirtableBase(appKey, baseId))
    {
 
       // Only use a 'do while' loop if you want to get multiple pages
       // of records.
       //

       do
       {
            Task<AirtableListRecordsResponse> task = airtableBase.ListRecords(
                   YOUR_TABLE_NAME, 
                   offset, 
                   fieldsArray, 
                   filterByFormula, 
                   maxRecords, 
                   pageSize, 
                   sort, 
                   view);

            AirtableListRecordsResponse response = await task;

            if (response.Success)
            {
                records.AddRange(response.Records.ToList());
                offset = response.Offset;
            }
            else if (response.AirtableApiError is AirtableApiException)
            {
                errorMessage = response.AirtableApiError.ErrorMessage;
                break;
            }
            else
            {
                errorMessage = "Unknown error";
                break;
            }
        } while (offset != null);
    }

    if (!string.IsNullOrEmpty(errorMessage))
    {
        // Error reporting
    }
    else
    {
        // Do something with the retrieved 'records' and the 'offset'
        // for the next page of the record list.
    } 

```

-------------------------------------

# Documentation

[View the full documentation](https://github.com/ngocnicholas/airtable.net/wiki/Documentation)