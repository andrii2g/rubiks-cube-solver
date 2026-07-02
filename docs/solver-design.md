# Solver Design

The first complete solver solves scramble input by applying the scramble to a solved cube, inverting the move sequence, and verifying that the inverse solves the cube.

54-facelet input is validated in two layers:

1. Sticker validation checks length, allowed labels, counts, and canonical centers.
2. Cubie validation checks edge/corner identity, orientation sums, and permutation parity.

Unsolved 54-facelet state solving uses a bounded bidirectional search over legal face turns. The search operates from facelet state alone and verifies every returned solution by applying the moves back to the initial cube.

If no solution is found within the configured max depth, the solver returns `MaxDepthExceeded` and must not present an unverified answer as solved. This is a correct bounded solver, not a speed-cubing-grade or full-depth Kociemba implementation.
