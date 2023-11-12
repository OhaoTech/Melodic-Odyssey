using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UnityEngine;

public class etMusicTempo : MonoBehaviour
{
    private async void Start()
    {
        // Path to the music file you want to upload
        string filePath = "C:\\Users\\djmax\\Desktop\\lab\\python\\john_wick.mp3";

        // Create a new HttpClient instance
        using (HttpClient client = new HttpClient())
        {
            // Define the Flask service URL
            string apiUrl = "http://127.0.0.1:5000/upload";

            // Create a new FormDataContent instance to send the file
            using (var form = new MultipartFormDataContent())
            {
                // Read the file into a stream
                using (var fileStream = File.OpenRead(filePath))
                {
                    // Create a StreamContent to represent the file
                    var fileContent = new StreamContent(fileStream);

                    // Set the Content-Disposition header with the name "file"
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    {
                        Name = "file",
                        FileName = Path.GetFileName(filePath)
                    };

                    // Add the file content to the form
                    form.Add(fileContent);

                    // Send the request to the Flask service
                    HttpResponseMessage response = await client.PostAsync(apiUrl, form);

                    // Check if the request was successful (HTTP status code 200)
                    if (response.IsSuccessStatusCode)
                    {
                        // Read the response content as a string
                        string responseContent = await response.Content.ReadAsStringAsync();

                        // Save the JSON response to a temporary file
                        string tempFilePath = Path.Combine(Application.persistentDataPath, "beat.json");
                        File.WriteAllText(tempFilePath, responseContent);

                        Debug.Log("Response from Flask service saved to: " + tempFilePath);
                    }
                    else
                    {
                        Debug.LogError($"HTTP Error: {response.StatusCode}");
                    }
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        // Delete the temporary file when the game stops
        string tempFilePath = Path.Combine(Application.persistentDataPath, "beat.json");
        if (File.Exists(tempFilePath))
        {
            File.Delete(tempFilePath);
            Debug.Log("Temporary file deleted: " + tempFilePath);
        }
    }
}
