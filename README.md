# Senior .NET Developer Homework Documentation

## Introduction

TODO

### Disclaimer

This project was completed under the following conditions:

* on a MacBook running macOS Big Sur (11.4)
* written using Visual Studio Code with the C# for Visual Studio Code extension
* developed against .NET 5.0

Output from author's `dotnet --info`:

```bash
.NET SDK (reflecting any global.json):
 Version:   5.0.300
 Commit:    2e0c8c940e

Runtime Environment:
 OS Name:     Mac OS X
 OS Version:  11.0
 OS Platform: Darwin
 RID:         osx.11.0-x64
 Base Path:   /usr/local/share/dotnet/sdk/5.0.300/

Host (useful for support):
  Version: 5.0.6
  Commit:  478b2f8c0e

.NET SDKs installed:
  5.0.300 [/usr/local/share/dotnet/sdk]

.NET runtimes installed:
  Microsoft.AspNetCore.App 5.0.6 [/usr/local/share/dotnet/shared/Microsoft.AspNetCore.App]
  Microsoft.NETCore.App 5.0.6 [/usr/local/share/dotnet/shared/Microsoft.NETCore.App]
```

Some other miscellaneous notes:

* Since Visual Studio was not utilized during development, it is not known whether the `EngineerHomework.sln` from the
starter project remains usable.
* Documentation instructions contain bash/zsh commands using *nix filepaths.
* This project was not able to be tested on a Windows system.

## Running

```bash
# From the project root:
dotnet run --project="EngineerHomework" path/to/OrgHierarchyData.csv path/to/UserData.csv path/to/output/directory
```

Example invocation:
```bash
# From the project root:
dotnet run --project="EngineerHomework" EngineerHomework.Tests/Data/OrgHierarchyData.csv EngineerHomework.Tests/Data/UserData.csv EngineerHomework.Tests/TestResults

# View the output file from the project root:
cat EngineerHomework.Tests/TestResults/org-collection-output.txt
```

`org-collection-output.txt` contents from the above example invocation:
```
1, 8, 188
	2, 4, 100
		21, 1, 40
		22, 1, 10
			23, 1, 10
	3, 3, 87
		31, 1, 1
		32, 1, 6
			321, 1, 6
4, 2, 32
	41, 1, 20
	42, 0, 0
```

## Testing

```bash
# From the project root:
dotnet test
```

Example test output snippet:
```
Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:    21, Skipped:     0, Total:    21, Duration: 95 ms - /path/to/EngineerHomework.Tests/bin/Debug/net5.0/EngineerHomework.Tests.dll (net5.0)
```

### Generating Code Coverage

Requires:
```bash
dotnet tool install -g dotnet-reportgenerator-globaltool
```

Generating code coverage results:
```bash
# From the project root:
dotnet test --collect:"XPlat Code Coverage"
# Assumes all other test result subdirectories were removed
reportgenerator "-reports:EngineerHomework.Tests/TestResults/*/coverage.cobertura.xml" "-targetdir:EngineerHomework.Tests/coveragereport" -reporttypes:Html

# Open the generated index.html from the project root (macOS-specific?):
open EngineerHomework.Tests/coveragereport/index.html
```

Code coverage stats at the time of submission:
| Name | Line Coverage | Branch Coverage
|------|---------------|----------------
| `EngineerHomework` | 95.6% | 92.1%
| `EngineerHomework.Models.Org` | 100% | 100%
| `EngineerHomework.Models.OrgCollection` | 100% | 100%
| `EngineerHomework.Models.User` | 100% | 100%
| `EngineerHomework.Program` | 78.1% | 80%
| `EngineerHomework.Service.DataLoadingUtil<T>` | 88.8% | 75%
| `EngineerHomework.Service.UserEntityBuilder` | 100% | N/A
| `EngineerHomework.Service.OrgEntityBuilder` | 100% | N/A

## Design

TODO