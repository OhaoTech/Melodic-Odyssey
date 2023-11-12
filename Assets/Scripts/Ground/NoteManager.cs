using System.Collections;
using UnityEngine;

public class NoteManager : MonoBehaviour
{
    public int scoreValue; // The score value of the note
    public float fadeOutTime = 1f; // Fade-out time
    public float enlargeScale = 1.5f; // 放大比例
    public float enlargeDuration = 0.5f; // 放大持续时间

    private SpriteRenderer spriteRenderer;
    private Collider2D collider;
    public float offScreenTimeThreshold = 3f; // Time in seconds a note can be off-screen before being destroyed

    private float offScreenTimer = 0f; // Timer to track how long the note has been off-screen
    private bool isOffScreen = false; // Flag to indicate whether the note is off-screen

    public MusicManager musicManager; // Reference to the MusicManager

    private void Start()
    {
        // Initialize the score value with a random number
        scoreValue = Random.Range(10, 100); // For example, between 10 and 100

        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        

    }

    private void Update()
    {
        if (isOffScreen)
        {
            // Increment the off-screen timer
            offScreenTimer += Time.deltaTime;

            // Check if the note has been off-screen for longer than the threshold
            if (offScreenTimer >= offScreenTimeThreshold)
            {
                Destroy(gameObject);
            }
        }
    }


	private void OnTriggerEnter2D(Collider2D other)
	{
	    if (other.gameObject.CompareTag("Player"))
	    {
	        ConsumeNote();
	        FindObjectOfType<MusicManager>().PlayMusicSegment(scoreValue); // 通知 GameManager
	    }
	}


    public void ConsumeNote()
    {
        // Disable the collider
        collider.enabled = false;

        // Start the fade-out and enlarge animation
        StartCoroutine(EnlargeAndFadeOut());
    }

    private IEnumerator EnlargeAndFadeOut()
    {
        float elapsedTime = 0;
        Vector3 originalScale = transform.localScale;

        while (elapsedTime < enlargeDuration)
        {
            // Gradually enlarge the note
            transform.localScale = Vector3.Lerp(originalScale, originalScale * enlargeScale, elapsedTime / enlargeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Now start fading out
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsedTime = 0;

        while (elapsedTime < fadeOutTime)
        {
            // Gradually change the opacity
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeOutTime);
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnBecameInvisible()
    {
        // Start the timer when the note becomes invisible
        isOffScreen = true;
    }

    private void OnBecameVisible()
    {
        // Reset the timer and flag when the note becomes visible again
        isOffScreen = false;
        offScreenTimer = 0f;
    }

}
