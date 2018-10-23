# Parallel Execution of Sql Code

SQL coders may want to run jobs in parallel, but not know how.  (Sql server management studio allows only a single job to run per window.)  A solution is provided to this problem in the form of a program that runs a set of SQL files in parallel consist with the run instructions.  It contains an executable that runs SQL files placed in a folder according to the instructions provided by the user.  The instructions allow for arbitrary complex parallel execution.  Multiple examples are given.

If you work within my healthcare system and are interested in this please send me an email, ronald.hauser@(healthcare suffix)

## Getting Started

Simply download the project and find the executable.  This project requires no prerequisites or installation.

## Tests

The project has tests for serial execution, parallel execution (splits and merge), and tests for errors that may occur with the SQL code.  The tests also have examples for how to write the instructions for the program to follow.

## Brief Description on How It Works

Given a path to a set of SQL files, and instruction for how to run them, validate the instructions then execute the SQL files according to the instructions until (1) all files complete, or (2) an error occurs.  Report the files that were started to run, the files that finished running, the time it took between the start and end of the run (i.e., start to error or complete), and the result of the run.

This program makes no assumptions about the state of the database before or after the run.  It’s up to the user to write code that can be overwritten or rolls back on error.

Functionality was added to resume a failed run.  If the 'log.txt' file contains the string '(Success)' then that node is considered run.  Since the log file changes between runs, it may be in your best interest to save a copy of the log file to track successful nodes in the event of multiple errors.  

## Parallel-Merged-Parameterized Queries

This is a common design pattern for my work:
1. Run a query each time for an entity.  The index is optimized for this query.
2. Store the results in an intermediate table
3. Merge the results from each run into a single table.
4. Delete the intermediate tables, leaving only the final table.  

Additional parameters were added to support this type of query.  Specifically,
- Table name: The name of the final table including the schema.  The database will add a suffix to this name to create the intermediate tables, so name collisons is a potential issue.
- Parameters: Each query is permitted to have a set of strings.  For example, the query could have three intermediate tables each with two parameters.

## Built With

C# in Visual Studio

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
