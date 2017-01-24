Contributing to EA4S - Antura and the Letters
=================

*Edits:*

<table>
  <tr>
    <td>11-01-2017</td>
    <td>Michele Pirovano</td>
  </tr>
</table>

Developers should follow these guidelines for contributing to the project.

### Coding conventions

  * Indent using four spaces (no tabs)
  * Use Unix newline
  * Use [Allman style](http://en.wikipedia.org/wiki/Indent_style#Allman_style) braces
  * Use **camelCase** for internal and private fields
  * Use **CamelCase** for public fields
  * Use **CamelCase** for all methods, public and private, for classes, enum types and enum values.
  * Use **THIS_STYLE** for constants.
  * Regions can be used to group code logically. Use **CamelCase** for region names.
  * No copyright notice nor author metadata should be present at the start of the file, unless it is of a third party
  
### Naming conventions

  * Use *MiniGame*, not *Minigame* (but *minigame* when lowercase)
  * All data related to the learning content should be termed *Dictionary* (instead of the triad Letter/Word/Phrase)
  
  
### Namespaces

The whole codebase is under the **EA4S** namespace.
The main systems can be accessed through the EA4S namespace and thus fall under it.

All minigames are under the **EA4S.MiniGames** namespace.
Each minigame needs its own namespace in the form of **EA4S.MiniGames.GAME_ID** with GAME_ID being the name of the minigame.

Specific subsystem code is inside a **EA4S.SUBSYSTEM** namespace, where SUBSYSTEM is the subsystem's name.

  EA4S
  
What follows is a list of all subsystems with their namespaces:

 * **EA4S.AnturaSpace** for code related to the Antura Space scene.
 * **EA4S.PlayerBook** for code related to the Player Book scene.
 * **EA4S.GamesSelector** handles the Games Selector scene.
 * **EA4S.Animations** for general animation utilities.
 * **EA4S.Db** for database access and organization.
 * **EA4S.LivingLetters** for scripts related to the Living Letter characters.


### Git Commit Messages

  * to be expanded

### Documentation Styleguide

  * to be expanded
 
### Current issues
 
  * nothing to report for now