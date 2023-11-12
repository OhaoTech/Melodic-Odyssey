using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSyncManager : MonoBehaviour
{
    public AudioSource musicSource; // Assign this in the Unity Editor
    public TextAsset pitchDataJson; // Assign the JSON file in the Unity Editor

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

    private List<PitchData> pitchDataList;
    private int currentPitchIndex = 0;

    private void Start()
    {
        // Deserialize the JSON data
        ResponseData responseData = JsonUtility.FromJson<ResponseData>(pitchDataJson.text);
        pitchDataList = responseData.pitch_data;
        pitchDataList = FilterPitchData(pitchDataList, 0.5f); // Example: 0.5 second interval

    }

    private void Update()
    {
        // Check if 'P' key is pressed
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (currentPitchIndex < pitchDataList.Count)
            {
                PlayCurrentPitch();
                currentPitchIndex++;
            }
        }
    }
    private List<PitchData> FilterPitchData(List<PitchData> originalData, float minTimeInterval)
    {
        List<PitchData> filteredData = new List<PitchData>();
        float lastTime = -minTimeInterval;

        foreach (var data in originalData)
        {
            if (data.time - lastTime >= minTimeInterval)
            {
                filteredData.Add(data);
                lastTime = data.time;
            }
        }

        return filteredData;
    }
    private void PlayCurrentPitch()
    {
        PitchData currentPitchData = pitchDataList[currentPitchIndex];
        musicSource.time = currentPitchData.time;
        musicSource.Play();

        // Optionally, perform an action based on the pitch
        HandlePitch(currentPitchData.pitch);

        // Assuming each pitch segment lasts for a short duration (e.g., 1 second)
        StartCoroutine(StopMusicAfterDelay(1.0f));
    }

    private IEnumerator StopMusicAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        musicSource.Stop();
    }

    private void HandlePitch(float pitch)
    {
        // Implement your logic here based on the pitch value
        Debug.Log("Pitch event at pitch: " + pitch);
    }
}
