# Light Grid is a mobile game prototype in which the player tap to rotate nodes and connect them on a grid

<img width="250" height="211" alt="flamel" src="https://github.com/user-attachments/assets/c1fee814-7899-4129-bcbe-3c36359aed1a" />

Features
- 10 levels
  - 4 handcrafted levels
  - 6 procedural levels
  - Square grids
  - Hexagonal grids
  - Wifi tiles: connect at a distance
- Satisfying visual feedbacks
- Relaxing soundtrack
- Save level progress and high score
- Amplitude SDK implementation for analytics

The game adapts the concept of bit shift operations to represent the rotation of the tile connections. A tile would have a bit for each of its sides, a positive value would represent the presence of a connection on that side and a negative value would represent its absence.
A standardization was set to define which bit represents which side of a tile. The bit array would always start from the top side of the tile, with its subsequent ones following a clockwise direction ( top - right - bottom - left).
To bring this concept to a little higher level of understanding, the bit array was represented by a bool array, with its bit shift operations being made as cyclic array rotations. 
In this case, a square tile with a straight line would be represented by the bool array [true, false, true, false], having connections on the top and bottom sides. A 90 degrees clockwise rotation of this tile would culminate in the bool array [false, true, false, true]

<img width="687" height="343" alt="diagram" src="https://github.com/user-attachments/assets/07d86cc0-46ef-4417-ae91-84cc7e2ece1b" />

For the handcrafted levels, the grid is manually placed on the scene and the game will load all the tiles, detect wether it is a square or hexagonal grid and  create its representation. Each time a tile is rotated a Breadth-first search (BFS) will determine which tiles are connected to the Energy Source and will light up.
Each tile estores its visual representation, its default bool array for connections and the current number of rotations it has performed. The first 4 levels are handcrafted with this methodology. Levels 1-3 are square grids while level 4 is hexagonal.
Level 3 has a special type of tile: the Wifi tile. When a Wifi tile is lit, all other Wifi tiles on the grid will also lit as they are always connected to each other independent of having a connection on the grid.

Levels 5-10 are procedural levels. These kinds of levels are only hexagonal, but adapting it to generate both square and hexagonal grids is possible. There are no Wifi tiles on procedural levels, each level differentiate from each other by having 2 Lamp tiles more than the previous level.
Procedural levels work similarly as the handcrafted ones. They first generate a hexagonal grid on the scene, then place a energy source tile and a number of lamp tiles ramdomly in the grid. Then a BFS search is performed to detect the sortest path from the energy source to each of the lamp tiles. Having the paths, the correspondent tiles are spawned in the grid to represent them and complete the level.

The game runs smoothly on the Android device, even though having array operations, they are not extensive enough to have a negative impact on performance. 
The code was made aiming to follow the SOLID principles and using additive scenes for each level.
