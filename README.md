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

This project was developed under the following conditions:

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
starter project remained in a usable state.
* Documentation instructions contain bash/zsh commands using *nix filepaths.
* This project was not able to be tested on a Windows system.

## Running

```bash
# From the project root:
dotnet run --project="EngineerHomework" path/to/OrgHierarchyData.csv path/to/UserData.csv path/to/existing/output/directory
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

Passed!  - Failed:     0, Passed:    24, Skipped:     0, Total:    24, Duration: 94 ms - /path/to/EngineerHomework.Tests/bin/Debug/net5.0/EngineerHomework.Tests.dll (net5.0)
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
| `EngineerHomework` | 96.4% | 94.7%
| `EngineerHomework.Models.Org` | 100% | 100%
| `EngineerHomework.Models.OrgCollection` | 100% | 100%
| `EngineerHomework.Models.User` | 100% | 100%
| `EngineerHomework.Program` | 78.1% | 80%
| `EngineerHomework.Service.DataLoadingUtil<T>` | 100% | 100%
| `EngineerHomework.Service.UserEntityGenerator` | 100% | N/A
| `EngineerHomework.Service.OrgEntityGenerator` | 100% | N/A

## Design Decisions and Thoughts

### Not Using `OrgCollection.GetOrgTree`

Despite being a requirement for the internal public API, `EngineerHomework.Models.OrgCollection.GetOrgTree` is not
used by `EngineerHomework.Program.Main`. In the very first pass of this project,
`EngineerHomework.Models.OrgCollection.GetOrgTree` was attempted to be used, but the author felt that it ended up
making the code inefficient and convoluted. This is because one would need to recalculate tree depth for a given node
from a linear flat `List<Org>`. Even though the list is returned in recursive tree order, one loses the context of
where a node falls in the tree unless maintaining a local stack of parentOrgIds.

The following is a snippet of `EngineerHomework.Program.Main` from that first pass:

```c#
foreach (int rootOrgId in orgCollection.GetRootOrgIds())
{
    var parentIds = new Stack<int>();
    parentIds.Push(Org.ROOT_ORG_PARENT_ORG_ID);
    foreach (var org in orgCollection.GetOrgTree(rootOrgId, true))
    {
        var latestParentId = parentIds.Peek();
        if (org.ParentId != latestParentId)
        {
            // Assumption: a parentOrgId is always less than one of its childOrgIds
            if (org.ParentId > latestParentId)
            {
                parentIds.Push(org.ParentId);
            }
            else
            {
                // Pop items off the top of the stack until the head Org == org.ParentId
                do
                {
                    parentIds.Pop();
                    latestParentId = parentIds.Peek();
                } while (latestParentId != org.ParentId);
            }
        }
        // The first pass was printing to stdout instead of writing to a file
        // The number of Ids on the parentIds stack was used to determine the indent level
        Console.Write($"{String.Concat(Enumerable.Repeat("\t", parentIds.Count - 1))}");
        Console.WriteLine($"{org.Id}, {org.GetTotalNumUsers()}, {org.GetTotalNumFiles()}");
    }
}
```

The above algorithm also made the assumption that a deeper Org.ParentId must be greater than a shallower Org.ParentId.
The author wanted the input data to have more flexibility in this regard.

Instead of the above code, the author decided to implement a Visitor-like design pattern that would execute a
`System.Action<Org, int>` against each node that is visited in the `EngineeringHomework.Models.OrgCollection` instance
in recursive tree order. The entry point to this API is `OrgCollection.VisitOrgsInRecursiveOrder(int, Action<Org, int>)`.
The first pass of `OrgCollection.GetOrgTree` was rewritten to also utilize
`OrgCollection.VisitOrgsInRecursiveOrder(int, Action<Org, int>)`.

The above design allows one to:
* execute an action against an `Org` while knowing its depth in the tree,
* execute actions against `Orgs` in recursive tree order, and
* not have to reimplement a recursive tree order traversal for every use case that requires it.

Instead of iterating the collection twice as was done in the above C# snippet and recalculating the depth of
a node in the tree at the time of writing the Org stats, the Org stats can now be written while traversing the
tree and knowing the node's depth based on recursive calls to the overloaded
`OrgCollection.VisitOrgsInRecursiveOrder(int, Action<Org, int>, [int])`

### Flexible Input Organization Hierarchy Insertion Order

The author wanted to eliminate assumptions about the order of lines in the input organization hierarchy CSV file.

`OrgCollection.Generate(IEnumerable<Org>)` accounts for out-of-order Orgs; namely, Orgs that have a ParentId for an
Org that has not been added yet. `OrgCollection.Generate(IEnumerable<Org>)` maintains a local
`Dictionary<int, List<int>>`, where the key is the missing ParentId and the value is a list of child Ids for Orgs
that were already added. A Dictionary was used for the advantage of fast lookups, especially since every handled
Org needs to check whether it was another Org's missing parent. If a previously-missing parent Org is added to the
OrgCollection and its child nodes are updated to account for the childen that came before it, the key-value entry
in the local `Dictionary<int, List<int>>` is removed so that the garbage collector can reclaim some memory.

*Note*: The main in-memory store for Orgs in OrgCollection is a Dictionary. Org.Id is used in helper stores, like
`OrgCollection._rootOrgIds`, to reduce redundant memory utilization and for quick resolutions of orgId => Org.

Child Orgs in `Org` and Root Org Ids in `OrgCollection` are stored in `System.Collections.Generic.SortedSet<T>`s to
maintain ascending order by Org.Id. Through the combined use of `OrgCollection.GetRootOrgIds()` and
`OrgCollection.VisitOrgsInRecursiveOrder(int, Action<Org, int>)`, this allows one to visit nodes in the same order
regardless of insertion order from the input CSV file.

*Note*: Instead of storing Org Ids or Orgs in order, the sorting could have been done on-demand when
`OrgCollection.GetRootOrgIds()` and `OrgCollection.VisitOrgsInRecursiveOrder(int, Action<Org, int>)` are called.
This could still be considered if one observes performance issues with this current approach.

### IEntityBuilder to IEntityGenerator

The author replaced the `EngineerHomework.Interfaces.IEntityBuilder` and its derived classes
`EngineerHomework.Service.*EntityBuilder`  from the starter project with `EngineerHomework.Interfaces.IEntityGenerator`
and its derived classes `EngineerHomework.Service.*EntityGenerator`. Instead of adding items to a `List<T>` and returning
the `List<T>` after all items have been added to the derived Builders, the derived Generators for some type `T`
simply call their `Generate` method, which returns a single instance of `T`. These Generators can then be used in a
`Enumerable` method to allow for more efficient "streaming" of data. The time it takes from reading a serialized representation
of an `Org` to adding an instance of that `Org` to an `OrgCollection` is drastically reduced, especially for larger
organization hierarchies.

### Penalty of Using Recursive Algorithms

The author prefers recursive solutions when dealing with aggregating data from trees and iterating through
the nodes of a tree. However, this does prevent **very** deep organization hierarchies from being processed due to
overflowing the call stack.

The author has not pinpointed the exact value, but somewhere between an organization hierarchy tree that is 25,000
levels deep and one that is 50,000 levels deep does this CLI tool fail with a `StackOverflowException`. If this CLI tool
must meet the use case of processing these deep organization hierarchies, then the recursive methods of `Org` and
`OrgCollection` will need to be replaced with iterative solutions.

### Other Possible Problems at Scale

Some other theoretical issues that this CLI tool may have when operating on larger (~500 million record) data sets:

* The memory usage is too much for a reasonably-priced consumer computer. The author's MacBook has 64 GB physical memory,
and an ~14 GB organization hierarchy input CSV file containing 500 million lines exceeded 32 GB RAM before the process
was manually killed.
  * Possible solution: no longer hold the entire data set in memory and instead rely on a disk-based database
  management system.
  * Possible solution: distribute workloads across multiple computing resources and shard the data store, whether the
  data store continues to be in-memory or moves to a disk-based database management system.
* The sum of files for a given organization could exceed `System.Int32.MaxValue`. If a single organization has 500 million
users (directly or via its child organizations), and each user has an average of 5 files, the total number of files
would exceed `System.Int32.MaxValue`.
  * Possible solution: switch to using `System.Int64`.

### Other Notes

* This project conforms to [Microsoft's C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions).
* XML documentation comments' styling tested with Visual Studio Code tooltips and Doxygen.
  * XML documentation comments were not added to all public interfaces, classes, methods, fields, and properties in the
  interest of time.
