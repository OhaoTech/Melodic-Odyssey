using UnityEngine;

public class GroundManager : MonoBehaviour
{
    public GameObject groundTilePrefab;
    
    private GameObject[] groundTiles = new GameObject[2];
    private Transform playerTransform;
    private Vector2 groundTileSize;

    // Offset to position the ground correctly relative to the player's feet.
    private Vector2 offset = new Vector2(0.43f, -2f); 

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        InitializeGround();
    }

    void InitializeGround()
    {
        Vector2 scale = new Vector2(25f, 25f);
        groundTileSize = new Vector2(1 * scale.x, 1 * scale.y);

        for (int i = 0; i < groundTiles.Length; i++)
        {
            // Instantiate the tile and scale it.
            groundTiles[i] = Instantiate(groundTilePrefab, new Vector2(i * groundTileSize.x, 0), Quaternion.identity);
            groundTiles[i].transform.localScale = scale;

            // Set the tile's layer to "Ground"
            groundTiles[i].layer = LayerMask.NameToLayer("Ground");

            // Adjust y position to match the player's feet assuming the player's pivot is at the bottom.
            float playerFeetYPosition = playerTransform.position.y - (playerTransform.localScale.y / 2);
            groundTiles[i].transform.position = new Vector2(i * groundTileSize.x, playerFeetYPosition - (groundTileSize.y / 2)) + offset;
        }
    }


    void Update()
    {
        foreach (var tile in groundTiles)
        {
            // If the tile is off-screen to the left of the player, recycle it to the right.
            if (playerTransform.position.x - tile.transform.position.x > groundTileSize.x)
            {
                // Calculate the position for the tile to be recycled to.
                float newXPosition = tile.transform.position.x + groundTileSize.x * groundTiles.Length;
                tile.transform.position = new Vector3(newXPosition, tile.transform.position.y, tile.transform.position.z);
            }
            // If the tile is off-screen to the right of the player, recycle it to the left.
            else if (playerTransform.position.x - tile.transform.position.x < -groundTileSize.x)
            {
                // Calculate the position for the tile to be recycled to.
                float newXPosition = tile.transform.position.x - groundTileSize.x * groundTiles.Length;
                tile.transform.position = new Vector3(newXPosition, tile.transform.position.y, tile.transform.position.z);
            }
        }
    }
}
