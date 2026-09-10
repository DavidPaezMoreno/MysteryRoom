# Bible Mystery Room

Bible Mystery Room is a mobile application built in Unity, designed for team-building activities, ice-breakers, Bible study activities, youth groups, or simply testing your knowledge of Bible stories.

## SCENES

### MainGameScene

- This is the scene in which the rooms take place. Here is where you can configure a new room (previously made into a prefab).
- The camera **SHOULD NOT be moved**. It is at the origin, and it should stay there to keep the current rooms in the desired position.

### StartScene

- This is the start menu screen. Here, the user can configure the Language and Time Limit, view the application Info, the Controls, and the Room Names.
- Here, the game loads the `TimeLimit` and the `SelectedLanguage` from the Unity `PlayerPrefs`, so it's important to pass through this scene to load that information.

### PrefabTestScene / ViewBuilder

- These two scenes are used to test new rooms. Neither of them has ambient light, so you can implement custom illumination for each room.

---

## CONTROLLER

- The code is rather easy to understand. It implements the Google Cardboard package to make the VR visor work and uses the phone's gyroscope for the **"No Visor"** mode.
- The gyroscope code is implemented in `GyroHeadTracking`. This class has functionality to rotate the padding rotation of the camera in case you need to change how the rotation is implemented, using the **"Debug Update"** GameObject in the `MainGameScene` (this is used for testing/debugging only).
- Google Cardboard is implemented by the `CardboardStartup` script in the `Startup` GameObject and the `EnableVR()` function on the `StartManager` from the `Manager` GameObject in the `StartScene`.

---

## FOLDERS

The relevant folders are as follows and should be used correctly:

```text
Assets/
├── Art/
│   ├── Animations/
│   ├── Fonts/
│   ├── Images/
│   │   ├── Misc/
│   │   └── Textures/
│   │       ├── Noise/
│   │       ├── Particles/
│   │       ├── Props/  (only small individual model textures should be saved here, and the ones here are all free to use in other rooms in this app)
│   │       └── Tiles/  (only tileable textures should be set here)
│   ├── UI/
│   ├── Materials/
│   │   ├── Colors/
│   │   ├── Decals/
│   │   ├── Particles/
│   │   ├── Props/  (only small individual model materials should be saved here, and the ones here are all free to use in other rooms in this app)
│   │   ├── Tiles/
│   │   └── UI/
│   ├── Models/
│   │   ├── Characters/
│   │   └── Props/  (only small individual models should be saved here, and the ones here are all free to use in other rooms in this app)
│   └── Shaders/
├── Localization/
│   ├── CSV/
│   ├── Locals/
│   └── Tables/
├── Prefabs/
│   └── Rooms/  (here are the room prefabs that the MainGameScene contains)
├── Scenes/
├── Scripts/
│   ├── Enums/
│   ├── Gameplay/  (these are scripts that are used in the MainGame)
│   ├── Managers/  (these are scripts that are used in the StartScene)
│   └── Utils/
├── Settings/
├── TextMesh Pro/
├── TutorialInfo/
└── XR/
```

## HOW TO ADD A NEW ROOM

The process to add a new room is as follows:

1. Import the necessary models, textures, etcetera, into the corresponding folder.
2. Generate the necessary materials.
3. Set the room in one of the test scenes inside an empty GameObject and add the necessary lights to achieve what you are looking for.
4. Drag the parent object to the `Prefabs > Rooms` folder with a distinctive name to generate a new prefab.
5. In the `MainGameScene`, generate a new `EmptyGameObject` at the origin position (recommended) inside the `RoomManager` GameObject.
6. In `RoomManager (GameObject) > RotateRoom (Script) > Waypoints (Variable)`, add a new Element and assign the new room to it.
7. Go to `Window > Asset Management > Localization Tables` and select the `RoomNamesTable` to add the new room name to the list in the available languages. It should maintain the same format for the key: `room.name.#`, where the number will be the one in the new GameObject.
8. Test the application from the main menu to see if your new room is added to the game and to the Room List from the Main Menu in the available languages (this list is loaded from the localization table).

## LANGUAGES

Supported Languages via Unity Localization:

1. 🇺🇸 English
2. 🇪🇸 Spanish

In case you want to add a new Language to the application, you should also add it to the `Language` enum object at `Scripts > Enums`.
