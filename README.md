# Potion Collector

A simple potion collector game as an assignment for for Felicity Games (Unity 6000.0.54f1)

## Editor Heads up
1. Some large files are not committed like TextMeshPro imports, Firebase v13.2.0 x86_64 plugins(necessary for editor working). Download from [here](https://developers.google.com/unity/archive#firebase)
2. Potion materials fallbacks to pink, probably due to addressables play mode set to Android and downloaded from remote 

## Game
### Input
The game uses a simple interaction logic by raycasting objects on click. A ray is fired from an input point(screen space coords) as world space ray using *ScreenPointToRay*.
   ### Flow
   1. The game flow starts from **Init** scene which is responsible for initializing stuff for the game, like saves and addressables.
   2. Once initialization is done it switches to **MainMenu** scene, where items like leaderboard and game start point reside. Input field acts as a *Name* setting point for the Player.
   3. Once started, **Game** scene is loaded where player start collecting potion by the defined input. As the potions are collected the HUD UI updates the score.
   4. From the gameplay state, by pressing the *Exit* btn the flow loops back to MainMenu scene, along with saving the score.
   ### Potion Spawner
   The potion spawning is handled by **PotionSpawner** instance, the holds the **PotionSpawnerData** reference which in turn holds list of potions to be spawned within certain time period.

   **PotionData** as a ScriptableObject holds different parameters like name, type, color, description, etc. which can be modified using the custom inspector drawn using **PotionDataEditor** class
 
## Addressables
Addressable groups where made for PotionData and the Potion prefab. the groups were set to remote config along with remote catalog, in-case we want to upgrade for future versions of PotionData with multiple prefab types.

**AddressableHandler** takes care of loading remote catalog as well as loading potion data

Apart from loading potion data, instantiating of prefab happens using Addressables by **PotionSpawner** using the *prefabReference*(AssetReference) variable of PotionData

Remote content i.e. the bundles and catalog files were manually uploaded to https://FelicityAssign-PotionCollector.b-cdn.net/addressables/ path on bunny.net hosting service.

## Event System
**GameEventSystem** acts as custom event system and takes care of the events mentioned throughout the game, acting as a centralized way of listening and firing events.
1. GameStarted -> listened by GameSaveManager and PotionSpawner
2. GameEnded -> listened by GameSaveManager
3. ScoreUpdated -> listened by GameSaveManager and UIHUD
4. LeaderboardLoaded -> listened by UIMainMenu
5. Other events as mentioned in the doc didn't need to be listened

All events are dispatched irrespective of their usage, ideally the need for events should arise from design needs or simple intuition

## Firebase
   ### Setup
   Simple fire base setup by importing the database package which contains the auth and app packages. The External Dependency Manager or EDM4AU was imported from google scoped registry for keeping the building process up to date.
   