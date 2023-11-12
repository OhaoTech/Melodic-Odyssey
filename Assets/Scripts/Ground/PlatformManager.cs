using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class PlatformManager : MonoBehaviour
{
    public GameObject platformPrefab;
    public Transform player;
    public Camera mainCamera;
    public float spawnOffset = 10f;
    public float despawnOffset = 5f;
    public int minPlatformCount = 1; // Minimum number of platforms to spawn each time
    public int maxPlatformCount = 5; // Maximum number
    public float minHorizontalSpacing = 0.5f; // Minimum horizontal spacing between platforms
    public float maxHorizontalSpacing = 2.0f; // Maximum horizontal spacing
    private float minGap = 0.01f;

    private List<GameObject> platforms = new List<GameObject>();
    private Vector2 screenBounds;
    private GameObject lastSpawnedPlatform = null;

    public GameObject notePrefab; 
    public float noteHeight = 2f; 

	public MusicManager musicManager;

    private void Start()
    {
        float camHeight = mainCamera.orthographicSize * 2;
        float camWidth = camHeight * mainCamera.aspect;
        screenBounds = new Vector2(camWidth, camHeight) / 2;
		musicManager = GetComponent<MusicManager>();
    }

    private void Update()
    {
        float direction = Input.GetAxis("Horizontal");

        if (direction > 0)
        {
            TrySpawnPlatform(player.position.x + spawnOffset, player.position.y);
        }
        else if (direction < 0)
        {
            TrySpawnPlatform(player.position.x - spawnOffset, player.position.y);
        }

        DespawnPlatforms();
    }

    private void TrySpawnPlatform(float xPosition, float yPosition)
    {
        if (lastSpawnedPlatform != null)
        {
            float distanceFromLastPlatform = Mathf.Abs(lastSpawnedPlatform.transform.position.x - xPosition);
            if (distanceFromLastPlatform < spawnOffset)
            {
                return;
            }
        }

        int platformCount = Random.Range(minPlatformCount, maxPlatformCount + 1);
        for (int i = 0; i < platformCount; i++)
        {
            lastSpawnedPlatform = SpawnPlatform(xPosition, yPosition);
            if (lastSpawnedPlatform == null) break;

            if (Random.value <= 0.7f) // 70% chance to spawn note on platform
            {
                SpawnNoteAbovePlatform(lastSpawnedPlatform);
            }
            else
            {
                SpawnNoteInAirOrGround(xPosition, yPosition); // New method to spawn note in air/ground
            }

            float horizontalSpacing = Random.Range(minHorizontalSpacing, maxHorizontalSpacing);
            xPosition += lastSpawnedPlatform.GetComponent<Renderer>().bounds.size.x + horizontalSpacing;
        }
    }

    private GameObject SpawnPlatform(float xPosition, float yPosition)
    {
        // The size of the platform, adjust if your platform has different dimensions
        Vector2 platformSize = platformPrefab.GetComponent<Renderer>().bounds.size;

        // Determine the direction of movement
        float direction = Mathf.Sign(xPosition - player.position.x);
        // Adjust the xPosition to account for the gap and direction
        xPosition += (platformSize.x + minGap) * direction;

        // Check the nearest platform on the x-axis and see if the gap is large enough
        bool gapIsLargeEnough = true;
        foreach (var platform in platforms)
        {
            if (Mathf.Abs(platform.transform.position.x - xPosition) < platformSize.x + minGap)
            {
                gapIsLargeEnough = false;
                break; // A platform is too close on the x-axis, so don't spawn a new one
            }
        }

        if (!gapIsLargeEnough)
            return null;

        // Perform an overlap box check to see if there's already a platform
        Collider2D overlapCheck = Physics2D.OverlapBox(new Vector2(xPosition, yPosition), platformSize, 0, LayerMask.GetMask("Ground"));
        if (overlapCheck != null)
        {
            // There's already a platform here, so don't spawn another
            return null;
        }

        // Spawn a new platform at the given position
        Vector3 spawnPosition = new Vector3(xPosition, yPosition, 0);
        GameObject newPlatform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        newPlatform.layer = LayerMask.NameToLayer("Ground");
        platforms.Add(newPlatform);
        if(newPlatform != null)
        {

        SpawnNoteAbovePlatform(newPlatform);
        }

        return newPlatform;
    }

    private void DespawnPlatforms()
    {
        // Use a temporary list to avoid modifying the list while iterating
        List<GameObject> platformsToRemove = new List<GameObject>();

        foreach (GameObject platform in platforms)
        {
            // Check if the platform is off-screen
            if (platform.transform.position.x < mainCamera.transform.position.x - screenBounds.x - despawnOffset ||
                platform.transform.position.x > mainCamera.transform.position.x + screenBounds.x + despawnOffset)
            {
                platformsToRemove.Add(platform);
            }
        }

        // Now remove and destroy the off-screen platforms
        foreach (GameObject platform in platformsToRemove)
        {
            platforms.Remove(platform);
            Destroy(platform);
        }
    }

    private void SpawnNoteAbovePlatform(GameObject platform)
    {
        Vector3 notePosition = platform.transform.position + new Vector3(0, noteHeight, 0);

        GameObject newNote = Instantiate(notePrefab, notePosition, Quaternion.identity);

        NoteManager noteManager = newNote.AddComponent<NoteManager>();
    }

	    private void SpawnNoteInAirOrGround(float xPosition, float yPosition)
    {
        // Logic to spawn note in air or on the ground
        // Adjust position as needed
        Vector3 notePosition = new Vector3(xPosition, yPosition + Random.Range(-2f, 5f), 0);
        Instantiate(notePrefab, notePosition, Quaternion.identity);
    }


}
