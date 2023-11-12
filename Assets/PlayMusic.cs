using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Collections;

public class PlayMusic : MonoBehaviour
{
    private List<PitchData> pitchDataList;
    private int currentIndex = 0;
    private float timer = 0f;
    private bool dataLoaded = false;

    [Serializable]
    public class PitchData
    {
        public float time;
        public float pitch;
    }

    private void Start()
    {
        StartCoroutine(WaitForFile());
    }

    private void Update()
    {
        if (dataLoaded && currentIndex < pitchDataList.Count)
        {
            timer += Time.deltaTime;
            while (currentIndex < pitchDataList.Count && timer >= pitchDataList[currentIndex].time)
            {
                PrintPitchData(currentIndex);
                currentIndex++;
            }
        }
    }

    private IEnumerator WaitForFile()
    {
        string tempFilePath = Path.Combine(Application.persistentDataPath, "beat.json");

        while (!System.IO.File.Exists(tempFilePath))
        {
            yield return null; // Wait for the next frame before checking again
        }

        string jsonData = System.IO.File.ReadAllText(tempFilePath);
        pitchDataList = JsonUtility.FromJson<SerializablePitchDataList>(jsonData).pitchDataList;
        dataLoaded = true;

        // Process and print the pitch data after the file becomes available
        for (int i = 0; i < pitchDataList.Count; i++)
        {
            float time = pitchDataList[i].time;
            float pitch = pitchDataList[i].pitch;
            Debug.Log($"Time: {time}, Pitch: {pitch}");
        }
    }

    private void PrintPitchData(int index)
    {
        float time = pitchDataList[index].time;
        float pitch = pitchDataList[index].pitch;
        Debug.Log($"Time: {time}, Pitch: {pitch}");
    }
}

[Serializable]
public class SerializablePitchDataList
{
    public List<PlayMusic.PitchData> pitchDataList;
}
