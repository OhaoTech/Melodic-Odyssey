using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Collections;

public class PlayMusic : MonoBehaviour
{    private string filePath;

    private void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "beat.json");
        StartCoroutine(WaitForFileAndReadData());
    }

    private IEnumerator WaitForFileAndReadData()
    {
        // Wait until the file exists
        while (!File.Exists(filePath))
        {
            yield return null;
        }

        // Read the JSON content
        string jsonContent = File.ReadAllText(filePath);
        ResponseData responseData = JsonUtility.FromJson<ResponseData>(jsonContent);

        // Print the data
        // foreach (var pitchData in responseData.pitch_data)
        // {
        //     Debug.Log($"Time: {pitchData.time}, Pitch: {pitchData.pitch}");
        // }
    }
}

[System.Serializable]
public class PitchData
{
    public float time;
    public float pitch;
}

[System.Serializable]
public class ResponseData
{
    public List<PitchData> pitch_data;
}
