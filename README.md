# Airtable .NET API Client

Airtable.net is the C-Sharp client of the public APIs of Airtable. It builds a Windows class library named AirtableClientApi.dll.
AirtableClientApi.dll facilitates the usage of Airtable APIs without having to worry about interfacing with raw HTTP, 
the low-level concepts like HTTP status codes and records paging. The users can write C-Sharp applications which integrate with
Airtable by consuming what Airtable public APIs have to offer programmatically such as List Records, Create Record, Retrieve Record, 
Update Record, Replace Record, Delete Record.

# Installation
Install the latest nuget package Airtable.0.9.0.0.nupkg

## Requirements

.NET Framework 4.5.2 or newer
Operating System: Windows 10 or newer
Microsoft Visual Studio 2015 or newer (only if you choose to build AirtableApiClient.dll from source files using the instructions below.)

## Build AirtableApiClient.dll

Download Airtable.net c# source files from github.com. To compile to an assembly, simply create a new project in visual studio 
of type Class Library and add these source files to the project. You will have to reference the following assemblies in order to 
successfully compile as a lot of library functions that are used in Airtable.net are defined there.

Newtonsoft.Json.dll.9.0.1 or newer can be downloaded from http://www.newtonsoft.com/json or from https://www.nuget.org/packages/newtonsoft.json/
.NETFramework\v4.5.2\Microsoft.CSharp.dll
.NETFramework\v4.5.2\System.dll
.NETFramework\v4.5.2\System.Core.dll
.NETFramework\v4.5.2\System.Net.Http.dll
.NETFramework\v4.5.2\System.Web.dll
.NETFramework\v4.5.2\System.Web.Extensions.dll

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