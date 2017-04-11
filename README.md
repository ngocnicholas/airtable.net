# Airtable .NET API Client

Airtable.net is the C-Sharp client of the public API of Airtable.
The goal of Airtable.net is to facilitate the usage of Airtable APIs without having to worry about interfacing with raw HTTP, the low-level concepts like HTTP status codes and records paging. The users can write C-Sharp code to consume what Airtable public APIs have to offer programmatically such as List Records, Create Record, Retrieve Record, Update Record, Replace Record, Delete Record.

# Installation

## Requirement

Operating System: Windows 10 or newer
.NET Framework 4.5.2 or newer
Microsoft Visual Studio 2015 or newer (only if you build AirtableApiClient.dll from source files.)

## Use AirtableApiClient.dll

If you downloaded AirtableApiClient.dll directly from github.com, register the dll using regsvr32. You then can make use of it by referencing to it in your project.
The .dll was built for an x86 machine so it can used in either an x86 or x64 computer.

If you build AirtableApiClient.dll from the instructions described in the "Build AirtableClientApi.dll" section, your AirtableClientApi.dll will be ready to use by referencing it in your project.

## Build AirtableApiClient.dll

Download Airtable.net c# source files from github.com. To compile to an assembly, simply create a new project in visual studio of type Class Library and add these source files to the project. You will have to reference the following assemblies in order to successfully compile as a lot of library functions that are used in Airtable.net are defined there. Newtonsoft.Json.dll.9.0.1 or newer can be downloaded from http://www.newtonsoft.com/json or from https://www.nuget.org/packages/newtonsoft.json/

C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\Microsoft.CSharp.dll
C:\Projects\airtable.net\packages\Newtonsoft.Json.9.0.1\lib\portable-net40+sl5+wp80+win8+wpa81\Newtonsoft.Json.dll
C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\System.dll
C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\System.Core.dll
C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\System.Net.Http.dll
C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\System.Web.dll
C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.5.2\System.Web.Extensions.dll

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