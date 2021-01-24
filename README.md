# Airtable .NET API Client

Airtable.net is the C-Sharp client of the public APIs of Airtable. It builds a Windows class library that targets .NET Standard named AirtableClientApi.dll.
AirtableClientApi.dll facilitates the usage of Airtable APIs without having to worry about interfacing with raw HTTP, 
the low-level concepts like HTTP status codes and records paging. The users can write C-Sharp applications which integrate with
Airtable by consuming what Airtable public APIs have to offer programmatically such as List Records, Create Record, Retrieve Record, 
Update Record, Replace Record, Delete Record.

# Installation
Install the latest nuget package Airtable.1.1.5.nupkg

## Requirements

.NET Framework 4.7.2 or newer
Operating System: Windows 10 or newer
Microsoft Visual Studio 2017 or newer (only if you choose to build AirtableApiClient.dll from source files using the instructions below.)
.NET Standard 2.0 is supported in Visual Studio 2017.

## Build AirtableApiClient.dll

Download .NET SDK 2.1.202 or newer.

Download Airtable.net c# source files from github.com. To compile to an assembly, simply create a new project in visual studio 
of C# .NET Standard Class Library and add these source files to the project. 
Refer to this link for creating a .NET Standard class library creation project using VS 2017 or newer:
https://blogs.msdn.microsoft.com/dotnet/2017/08/14/announcing-net-standard-2-0/
This link also shows how to use Manage Nuget Packages in Visual Studio to refer to the Newtonsoft.Json.dll.11.0.2.

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
       //
       // Use 'offset' and 'pageSize' to specify the records that you want
       // to retrieve.
       // Only use a 'do while' loop if you want to get multiple pages
       // of records.
       //

       do
       {
            Task<AirtableListRecordsResponse> task = airtableBase.ListRecords(
                   YOUR_TABLE_NAME, 
                   offset, 
                   fields, 
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
