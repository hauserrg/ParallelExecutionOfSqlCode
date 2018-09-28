# Parallel Execution of Sql Code

SQL coders may want to run jobs in parallel, but not know how.  (Sql server management studio allows only a single job to run per window.)  A solution is provided to this problem in the form of a program that runs a set of SQL files in parallel consist with the run instructions.  It contains an executable that runs SQL files placed in a folder according to the instructions provided by the user.  The instructions allow for arbitrary complex parallel execution.  Multiple examples are given.

If you work within my healthcare system and are interested in this please send me an email, ronald.hauser@(healthcare suffix)

## Getting Started

Simply download the project and find the executable.  This project requires no prerequisites or installation.

## Tests

The project has tests for serial execution, parallel execution (splits and merge), and tests for errors that may occur with the SQL code.

## Built With

C# in Visual Studio

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details
