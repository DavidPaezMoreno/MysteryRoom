using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

public class GenerateRoomList : MonoBehaviour
{
    private TMP_Text roomListText;

    public string roomNamesTableName = "RoomNamesTable"; // Name of the asset table for room names

    public void GenerateList()
    {
        roomListText = GetComponent<TMP_Text>();

        if (roomListText == null)
        {
            Debug.LogError("TMP_Text component not found on the GameObject.");
            return;
        }

        // Generate the room list text
        StringTable roomTable = LocalizationSettings.StringDatabase.GetTable(roomNamesTableName);

        if (roomTable == null)
        {
            Debug.LogError($"{roomNamesTableName} asset table not found.");
            return;
        }

        roomListText.text = string.Empty;
        foreach (StringTableEntry roomEntry in roomTable.Values)
        {
            roomListText.text += $"No. {roomEntry.Key.Split('.')[2]} - {roomEntry.LocalizedValue}\n";
        }
    }
}
