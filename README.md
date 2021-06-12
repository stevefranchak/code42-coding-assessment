# Senior .NET Developer Homework Documentation

## Introduction

This project builds a CLI tool that produces a breakdown of number of users and number of user files for each
organization in an organization hierarchy. The organization hierarchy and user data are expected to be provided as
CSV files in the following formats:

**Organization Hierarchy CSV**:

Format: `orgId, parentOrgId, name`

Example:
```
1, null, Root1
2, 1, A
3, 1, B
21, 2, A1
4, null, Root2
41, 4, C1
```

*Note*: The Organization Hierarchy allows multiple hierarchy trees. The root of each tree is indicated by an organization
node with a `null` or `0` parentOrgId.

**User Data CSV**:

Format: `userId, orgId, numFiles`

Example:
```
101, 1, 1
102, 2, 20
103, 2, 30
105, 21, 40
109, 4, 12
```

The output of this CLI tool is written to a `org-collection-output.txt` file in a provided, already-existing output folder.
The contents of this file display the trees of the organization hierarchy per the following rules:
* One line per organization node
  * Format: `orgId, totalNumUsers, totalNumFiles` (totals include itself plus all of its children, recursively)
* The organization node is indented to match its depth in the tree
  * A root node has no identation
  * Identation is done using the tab control character
* An organization tree is traversed in recursive tree order
  * Recursive tree order visits a node, then its child nodes recursively, and then its sibling nodes, in which each
  sibling node will also recursively visit its child nodes before subsequent siblings.
* Sibling nodes in the tree are traversed by their ID in ascending order
* An organization with a duplicate ID is warned about but gracefully ignored and not displayed in the final output

Example `org-collection-output.txt` file contents based on the above CSV examples:
```
1, 4, 91
	2, 3, 90
		21, 1, 40
	3, 0, 0
4, 1, 12
	41, 0, 0
```

This CLI tool expects the following required arguments when invoked:

1. Fully-qualified path to the input test file OrgHierarchyData.csv
2. Fully-qualified path to the input test file UserData.csv
3. Fully-qualified path to the output folder (must already exist)

## Disclaimers

This project was completed under the following conditions:

* on a MacBook running macOS Big Sur (11.4)
* written using Visual Studio Code with the C# for Visual Studio Code extension
* developed against .NET 5.0

Output from author's `dotnet --info`:

```
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

Passed!  - Failed:     0, Passed:    23, Skipped:     0, Total:    23, Duration: 92 ms - /path/to/EngineerHomework.Tests/bin/Debug/net5.0/EngineerHomework.Tests.dll (net5.0)
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
| `EngineerHomework` | 95.7% | 92.5%
| `EngineerHomework.Models.Org` | 100% | 100%
| `EngineerHomework.Models.OrgCollection` | 100% | 100%
| `EngineerHomework.Models.User` | 100% | 100%
| `EngineerHomework.Program` | 78.1% | 80%
| `EngineerHomework.Service.DataLoadingUtil<T>` | 88.8% | 75%
| `EngineerHomework.Service.UserEntityBuilder` | 100% | N/A
| `EngineerHomework.Service.OrgEntityBuilder` | 100% | N/A

## Design

TODO to talk about:
* EngineerHomework.Models.OrgCollection.GetOrgTree was not used in EngineerHomework.Program.Main
  * Would need to recalculate tree depth from a linear flat list when we would already know the level at the time the tree is being iterated
  * Implemented a Visitor-like pattern to reuse visiting org nodes in recursive tree order
  * Even though EngineerHomework.Models.OrgCollection.GetOrgTree is not directly used, still implemented as it's a requirement for the internal public API
* Orgs can be fed into EngineerHomework.Models.OrgCollection.Generate out-of-order and there is no restriction on Org.ParentId needing to be less than Org.Id
* Orgs are stored in order of ascending Id so that, regardless of insertion order, the nodes are visited the same way
* Tried to conform to [Microsoft's C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).

Possible problems with a large data set:
* Overflowing ints?
* Stack overflow (recursion)
* Possible limit on Dictionary size?