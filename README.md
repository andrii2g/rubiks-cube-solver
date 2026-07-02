# rubiks-cube-solver

Rubik's Cube solver and 2D Blazor visualizer written in .NET 10.

Current implementation status:

- Solution skeleton with Core, Solver, CLI, Web, and test projects.
- Immutable 54-facelet cube model.
- MVP move parsing for `U R F D L B` with optional `'`, `i`, or `2` suffixes.
- Generated sticker move tables from one coordinate mapping.
- Sticker-level 54-facelet validation.
- Deterministic scramble generation.
- Core unit tests for parsing, moves, validation, and scramble invariants.

Arbitrary 54-facelet state solving is not implemented yet. Until the two-phase solver is implemented and verified, state input is for parsing, validation, and visualization work only.

## Build

```bash
dotnet restore
dotnet build
dotnet test
```
