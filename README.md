# BadScenarioScanner
Scans a folder tree for all SpecFlow Feature files, looking for Examples tables that are not under Scenario Outlines.

This takes a single parameter, which is the path to the folder tree to be scanned.

Install
-------

This tool is compatible with Visual Studio, and can be added to the "Tools | External Tools..." list.

To do this:

1. Open Tools | External Tools... in Visual Studio.
2. Add a new entry, with title "Bad Scenario Scanner"
3. In the Command box, browse to the BadScenarioScanner.EXE file
4. In the Arguments box, enter "$(SolutionDir)".
5. Be sure to tick the "Use Output Window" box.



Here is an example of a suspicious scenario, where the Examples table really belongs under a Scenario Outline:

```
Scenario: Everyone is happy doing this and that
    Given something exists
    And something else is the case
    When something happens with <this> and <that>
    Then everyone is happy

    Examples:
    | this | that |
    | 1    | A    |
    | 2    | B    |
    | 3    | C    |
    | 4    | D    |
```

Here is an example of the intended scenario - namely, done as a Scenario Outline:

```
Scenario Outline: Everyone is happy doing this and that
    Given something exists
    And something else is the case
    When something happens with <this> and <that>
    Then everyone is happy

    Examples:
    | this | that |
    | 1    | A    |
    | 2    | B    |
    | 3    | C    |
    | 4    | D    |
```


