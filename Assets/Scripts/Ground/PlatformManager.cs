using UnityEngine;
using System.Collections.Generic;

public class PlatformManager : MonoBehaviour
{
    public GameObject platformPrefab; // Your platform prefab
    public Transform player; // Reference to the player object
    public Camera mainCamera; // Reference to the main camera
    public float spawnOffset = 10f; // How far ahead of the player to spawn platforms
    public float despawnOffset = 5f; // How far off-screen platforms should be before despawning

    private List<GameObject> platforms = new List<GameObject>(); // List to hold the platforms
    private Vector2 screenBounds; // To store the screen bounds
    private float minGap = 0.3f; // Minimum gap between platforms, adjust this value as needed


    private void Start()
    {
        // Calculate the screen bounds
        float camHeight = mainCamera.orthographicSize * 2;
        float camWidth = camHeight * mainCamera.aspect;
        screenBounds = new Vector2(camWidth, camHeight) / 2;
    }

    private void Update()
    {
        // Determine the direction of the player's movement
        float direction = Input.GetAxis("Horizontal");

        // Spawn platforms if needed
        if (direction > 0)
        {
            SpawnPlatform(player.position.x + spawnOffset, player.position.y);
        }
        else if (direction < 0)
        {
            SpawnPlatform(player.position.x - spawnOffset, player.position.y);
        }

        // Check platforms for despawning
        foreach (var platform in new List<GameObject>(platforms)) // Create a copy to modify the original list during iteration
        {
            if (platform.transform.position.x < mainCamera.transform.position.x - screenBounds.x - despawnOffset ||
                platform.transform.position.x > mainCamera.transform.position.x + screenBounds.x + despawnOffset)
            {
                platforms.Remove(platform);
                Destroy(platform);
            }
        }
    }


    private void SpawnPlatform(float xPosition, float yPosition)
    {
        // The size of the platform, adjust if your platform has different dimensions
        Vector2 platformSize = platformPrefab.GetComponent<Renderer>().bounds.size;

        // Adjust the xPosition to account for the gap
        // This assumes platforms move to the right; for left movement, you would subtract the gap
        xPosition += platformSize.x + minGap;

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
            return;

        // Perform an overlap box check to see if there's already a platform
        Collider2D overlapCheck = Physics2D.OverlapBox(new Vector2(xPosition, yPosition), platformSize, 0, LayerMask.GetMask("Ground"));
        if (overlapCheck != null)
        {
            // There's already a platform here, so don't spawn another
            return;
        }

        // Spawn a new platform at the given position
        Vector3 spawnPosition = new Vector3(xPosition, yPosition, 0);
        GameObject newPlatform = Instantiate(platformPrefab, spawnPosition, Quaternion.identity);
        newPlatform.layer = LayerMask.NameToLayer("Ground");
        platforms.Add(newPlatform);
    }


}
