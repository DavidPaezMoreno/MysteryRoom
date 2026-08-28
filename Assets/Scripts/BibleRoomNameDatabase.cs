using System;
using UnityEngine;

public class BibleRoomNameDatabase : MonoBehaviour
{
    [Serializable]
    private class RoomData
    {
        public int RoomNumber;
        public string[] RoomName;
    }

    [Serializable]
    private class RoomDatabaseData
    {
        public RoomData[] Rooms;
    }

    [SerializeField] private TextAsset roomNamesJson;

    private RoomDatabaseData database;

    private void Awake()
    {
        if (roomNamesJson == null)
        {
            Debug.LogError("Room names JSON is not assigned.", this);
            return;
        }

        database = JsonUtility.FromJson<RoomDatabaseData>(roomNamesJson.text);
    }

    public string GetRoomName(int roomIndex, Language language = Language.English)
    {
        if (database == null || database.Rooms == null)
        {
            Debug.LogError("Room names database is not loaded.", this);
            return string.Empty;
        }

        if (roomIndex < 1 || roomIndex > database.Rooms.Length)
        {
            Debug.LogError($"Room index {roomIndex} is outside the valid range 1-{database.Rooms.Length}.", this);
            return string.Empty;
        }

        RoomData room = database.Rooms[roomIndex - 1];
        int languageIndex = (int)language;
        if (room.RoomName == null || languageIndex < 0 || languageIndex >= room.RoomName.Length)
        {
            Debug.LogError($"Language {language} is not available for room {roomIndex}.", this);
            return string.Empty;
        }

        return room.RoomName[languageIndex];
    }
}
