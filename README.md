 # Airtable .NET API Client

Airtable.net is the C-Sharp client of the public APIs of Airtable. It builds a Windows class library that targets .NET Standard named AirtableClientApi.dll.
AirtableClientApi.dll facilitates the usage of Airtable APIs without having to worry about interfacing with raw HTTP,
the low-level concepts like HTTP status codes and records paging. The users can write C-Sharp applications which integrate with
Airtable by consuming what Airtable public APIs have to offer programmatically such as List Records, Create Record, Retrieve Record,
Update Record, Replace Record, Delete Record.

# Installation
Install the latest nuget package Airtable.1.3.0.nupkg

## Requirements

Operating System: Windows 10 or newer
Microsoft Visual Studio 2022 or newer (only if you choose to build AirtableApiClient.dll from source files using the instructions below.)
.NET Standard 2.0 and the usage of System.Text.Json are supported in Visual Studio 2022.

## Build AirtableApiClient.dll

Download Airtable.net c# source files from github.com. To compile to an assembly, simply create a new project in visual studio
of C# .NET Standard Class Library and add these source files to the project.

Refer to this link for downloading VS 2022 Community Edition:
https://visualstudio.microsoft.com/free-developer-offers/

Refer to the link below for downloading .NET SDK for VS 2022
https://github.com/dotnet/core/blob/main/release-notes/7.0/7.0.3/7.0.3.md?WT.mc_id=dotnet-35129-website

Refer to the link below to learn more about what's in .NET Standard 2.0
https://docs.microsoft.com/en-us/dotnet/standard/net-standard

# Quickstart

Example demonstrating usage of the API to list records:

----------------------

```

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using AirtableApiClient;

readonly string baseId = YOUR_BASE_ID;
readonly string appKey = YOUR_APP_KEY_OR_ACCESS_TOKEN;

```

----------------------


```

    string offset = null;
    string errorMessage = null;
    var records = new List<AirtableRecord>();

    using (AirtableBase airtableBase = new AirtableBase(appKeyOrAccessToken, baseId))
    {
       //
       // Use 'offset' and 'pageSize' to specify the records that you want
       // to retrieve.
       // Only use a 'do while' loop if you want to get multiple pages
       // of records.
       //

       do
       {
            Task<AirtableListRecordsResponse> task = airtableBase.ListRecords(
                   YOUR_TABLE_ID_OR_NAME,
                   offset,
                   fieldsArray,
                   filterByFormula,
                   maxRecords,
                   pageSize,
                   sort,
                   view,
                   cellFormat,
                   timeZone,
                   userLocale,
                   returnFieldsByFieldId,
                   includeCommentCount);

            AirtableListRecordsResponse response = await task;

            if (response.Success)
            {
                records.AddRange(response.Records.ToList());
                offset = response.Offset;
            }
            else if (response.AirtableApiError is AirtableApiException)
            {
                errorMessage = response.AirtableApiError.ErrorMessage;
                if (response.AirtableApiError is AirtableInvalidRequestException)
                {
                    errorMessage += "\nDetailed error message: ";
                    errorMessage += response.AirtableApiError.DetailedErrorMessage;
                }
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
