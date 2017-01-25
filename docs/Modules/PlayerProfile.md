# Player Profile

This document explains how Player Profiles are handled throughout the application.

A Player Profile contains data on a given player, allowing multiple players to use the application
 and to keep updates on their progression.

### Contents

A Player Profile contains the following data:
 * **Profile information**
	* *Id*: unique Id assigned to the player.
	* *Age*: age of the player.
	* *Name*: player name.
	* *AvatarId*: chosen avatar image Id.
 * **Journey progression state**
	* *ProfileCompletion*: a value that defines the state of the profile in respect to tutorial scenes.
	* *CurrentJourneyPosition*: the current selected position in the map journey.
	* *MaxJourneyPosition*: the maximum reached position in the map journey.
 * **Rewards state**
	* *TotalNumberOfBones*: number of bones collected.
	* *CurrentAnturaCustomizations*: selected customization for Antura.

Note that additional progression data is also contained in the runtime database (see Database.md).

Note that each profile is assigned an unique Id.
This Id is used for:
 * selecting and identifying the player profile by the Player Profile Manager
 * identifying the database assigned to the player (see Database.md)


### Serialization

All data related to the Player Profile is serialized using the PlayerPrefs functionality of Unity.

*** note: this should change so that all data will be in the existing MySQLdatabase after refactoring ***

### Creation & Deletion

The Player Profile Manager handles creation, selection, and deletion of player profiles.
The system is designed to support a maximum number of players, defined as ***PlayerProfileManager.MaxNumberOfPlayerProfiles**.

A list of existing player profiles can be retrieved from the **AppManager.GameSettings.AvailablePlayers**.

Whenever a Player Profile is created, an exclusive Avatar Id is also selected,
  which represents the avatar image assigned to that profile.

**PlayerProfileManager.CurrentPlayer** holds the current player profile.
**AppManager.I.GameSettings.LastActivePlayerId** contains the Id of the profile last accessed through the application.

At runtime, creation, deletion, and selection of player profiles is performed in the Home (_Start) scene
 through the **Profile Selector**.


### Journey position

The Journey position is defined as a hierarchical structure, made of Stages, Learning Blocks, and Play Sessions.

 * **Stages** define overall learning goals. Each stage is assigned to a specific Map.
 * **Learning Blocks** define general learning requirements for a set of play sessions.
 * **Play Sessions** define single play instances, composed of several minigames and a result.
  A Play Session may be considered an **Assessment**, in this case the value is always 100.

Each is defined by a sequential integer value.
A combination of three values identifies a single playing session, which is referred to as **Journey Position**.

A Journey Position is thus identified by a the sequence **X.Y.Z** where X is the Stage,
  Y the Learning Block, and Z the Play Session.

### Refactoring Notes
 * Mood and PlaySkill values in PlayerProfile are not used. Should be removed.
 * Rewards unlock should be in the SQLite DB
 * We should correctly separate data that should go into the PlayerProfile from data that should be in the log database
