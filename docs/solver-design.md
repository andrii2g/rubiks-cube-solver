# Solver Design

The first complete solver solves scramble input by applying the scramble to a solved cube, inverting the move sequence, and verifying that the inverse solves the cube.

54-facelet input is validated in two layers:

1. Sticker validation checks length, allowed labels, counts, and canonical centers.
2. Cubie validation checks edge/corner identity, orientation sums, and permutation parity.

Arbitrary unsolved 54-facelet state solving is planned for a later two-phase solver milestone. Until then, valid unsolved state input returns `SolverUnavailable` and must not be presented as solved.
