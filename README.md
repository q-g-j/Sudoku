# Sudoku

Copyright 2022 Jann Emken

**Written in C# for .NET Framework 4.7**

### Features:

- mouse and keyboard input
- random puzzle generator with 3 difficulties
- mark conflicts in red
- menu action to fill in all markers automatically
- menu option to only generate Sudokus with a single solution
- solve current Sudoku from the menu:</br>
try to solve the sudoku logically first; if that fails, run a backtracking (brute-force) method
- 5 save slots *(save files location: %APPDATA%\SudokuGame)*
- basic printing functionality
- place up to 9 markers in each square
- English and German localization (according to the system language)

### Controls:

|Mouse button|Action|
|-|-|
|Left click|select a cell to place a number|
|Right click|select a cell to place a marker|

|Key|Action|
|-|-|
|Numbers 1-9|place a number / marker (after left / right click)|
|Delete Key|delete a number / all markers of current cell (after left / right click)|

### Screenshots:

<img src="https://github.com/q-g-j/Sudoku/raw/master/WPF/screenshot-number.jpg" width="500">
<img src="https://github.com/q-g-j/Sudoku/raw/master/WPF/screenshot-marker.jpg" width="500">
<img src="https://github.com/q-g-j/Sudoku/raw/master/WPF/screenshot-conflicts.jpg" width="500">
<img src="https://github.com/q-g-j/Sudoku/raw/master/WPF/screenshot-won.jpg" width="500">
