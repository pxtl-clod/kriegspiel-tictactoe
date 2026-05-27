# AGENTS.md

See [CONTRIB.md](CONTRIB.md) for coding style guide.

# Dev Environment

Codebase is using dotnet 10.0.  Use `dotnet build` to compile, and 

```sh
dotnet run --project kriegspiel-tictactoe/tests/KriegspielTicTacToe.Model.Tests
```
to run unit test suite.

# Testing

Unit tests are in `tests/KriegspielTicTacToe.Model.Tests`.

Do not attempt automated testing on the command-line tool. Test improvement can
be achieved by codifying more logic into model.

## Testing Warning about Out Of Memory

Note that in the event of a stack overflow, `dotnet test` can start failing with
OOM.  In that case, clear `/tmp`.  Also `dotnet run` the test project reduces
likelihood of OOM and still runs the tests.  You may have to use `top` and
`kill` to clear out orphaned dotnet processes.

# Refactoring

Do not attempt to refactor or otherwise clean-up the code unless specifically
instructed to do so. Do not remove comments unless they are untrue.

# Scratch space

Create any needed working files such as to-do lists within the `temp` dir, which is gitignored.