# Solver Design

The first complete solver will solve scramble input by applying the scramble to a solved cube, inverting the move sequence, and verifying that the inverse solves the cube.

Arbitrary 54-facelet state solving is planned for a later two-phase solver milestone. Until then, valid state input must not be presented as solved unless verification passes.
