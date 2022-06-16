# Sudoku

Copyright 2022 Jann Emken

A graphical version of the Sudoku game written in C#. The [Windows](https://github.com/q-g-j/Sudoku/tree/master/WPF) version is almost finished - an Android version is in the works.

## Windows Version:

**Written in C# with WPF for .NET Framework 4.7**

### Features:

- random puzzle generator with 3 difficulties
- menu option to only generate Sudokus with a single solution
- menu action to fill in all markers automatically
- 5 save slots *(save files location: %APPDATA%\SudokuGame)*
- solve current Sudoku from the menu
- place up to 9 markers in each square
- English and German localization (according to the system language)
- basic printing functionality

### Controls:

|Mouse button|Action|
|-|-|
|Left click|place a number|
|Right click|place a marker|

|Key|Action|
|-|-|
|Numbers 1-9|place a number / marker (after left / right click)|
|Delete Key|delete a number / all markers (after left / right click)|
