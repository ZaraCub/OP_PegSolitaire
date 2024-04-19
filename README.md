# OP_PegSolitaire

## Project Overview

This project presents a digital version of the traditional Peg Solitaire puzzle. Peg Solitaire is a single-player logic game where the objective is to leave only one peg on the board after a series of valid moves. In this digital adaptation, players interact with a visualized board, selecting pegs and choosing target positions to move them.

The application includes several functionalities to enhance the gaming experience. It features automatic move validation, ensuring that each move adheres to the game's rules. Additionally, the application provides result tracking, allowing players to monitor their progress and scores throughout the game.

## Game Rules

The rules of Peg Solitaire are simple:
- The game is played on a board with holes, initially filled with pegs except for one hole.
- Players make moves by jumping a peg over another adjacent peg, removing the peg that was jumped over.
- The objective is to eliminate as many pegs as possible until only one peg remains on the board.
- Valid moves are those where a peg jumps over an adjacent peg into an empty hole, horizontally or vertically.

## Implementation

The project is implemented in C# and employs principles of object-oriented programming. It encompasses classes responsible for game logic, user interface, and data storage. Utilizing the SQLite library, the application stores game data such as player scores and game duration for tracking and analysis purposes.
