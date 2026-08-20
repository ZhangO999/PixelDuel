# Pixel Duel

A small local two-player platform shooter made with Unity and C#. Both players
share one keyboard, and the first player to five knockdowns wins.

## Project history

I started Pixel Duel in 2021. It was one of the first larger programming
projects I built end to end, and my first real attempt at using C# and
object-oriented programming. I was still fairly new to programming, so the
original version was pretty buggy.

I came back to it in 2026 to practise some C# again. With the assistance of AI,
I cleaned up parts of the game, replaced the UI and sprites, and updated the
project to work with Unity 6. The basic idea and most of the gameplay still come
from the original project.

## Demo

[![Pixel Duel gameplay](tools/arena_play.png)](media/platform-shooter-demo.mov)

[Watch the gameplay demo](media/platform-shooter-demo.mov)

## Running the game

1. Open this folder in Unity 6 (`6000.0.82f1`).
2. Press **Play**.

There is no scene setup required. The game creates the arena, players and UI at
runtime.

## Controls

| | Move | Aim | Jump | Fire |
|---|---|---|---|---|
| **Player 1** | `A` `D` | `W` `S` | Left Shift | Space |
| **Player 2** | `←` `→` | `↑` `↓` | `/` | Right Shift |

- `R` returns to the title screen.
- `Esc` quits.
- Hold down and jump to drop through a wooden platform.

## Notes

- The sprites are drawn from character grids in `Assets/Scripts/Art.cs`.
- The arena layout is stored as a text map in `Assets/Scripts/Level.cs`.
- Gameplay values such as movement speed and jump height are near the top of
  `Assets/Scripts/Player.cs`.
