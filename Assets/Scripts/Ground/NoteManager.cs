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

    private void Start()
    {
        // Initialize the score value with a random number
        scoreValue = Random.Range(10, 100); // For example, between 10 and 100

        spriteRenderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
        

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object is the player
        if (other.gameObject.CompareTag("Player"))
        {
            ConsumeNote();
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


}
