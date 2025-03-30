using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages celebration animations when the player wins a level using only sprites.
/// </summary>
public class CelebrationManager : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private int numberOfStars = 5;
    [SerializeField] private float animationDuration = 3.0f;
    [SerializeField] private float delayBetweenStars = 0.2f;

    [Header("Sprites")]
    [SerializeField] private Sprite goldStarSprite;
    [SerializeField] private Sprite whiteSparkleSprite;

    // Singleton pattern for easy access
    public static CelebrationManager Instance { get; private set; }

    private bool isPlaying = false;
    private List<GameObject> celebrationObjects = new List<GameObject>();

    private void Awake()
    {
        // Set up singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Play the celebration animation when a level is won.
    /// </summary>
    public void PlayCelebration()
    {
        if (isPlaying) return;

        isPlaying = true;
        StartCoroutine(PlayCelebrationSequence());
    }

    /// <summary>
    /// Coroutine to play the full celebration sequence.
    /// </summary>
    private IEnumerator PlayCelebrationSequence()
    {
        // Create stars
        for (int i = 0; i < numberOfStars; i++)
        {
            GameObject star = CreateAnimatedStar();
            celebrationObjects.Add(star);

            // Create a burst of 3-5 sparkles around the star
            int sparkleCount = Random.Range(3, 6);
            for (int j = 0; j < sparkleCount; j++)
            {
                GameObject sparkle = CreateSparkle(star.transform.position);
                celebrationObjects.Add(sparkle);
            }

            // Wait before creating the next star
            yield return new WaitForSeconds(delayBetweenStars);
        }

        // Wait for the full animation duration
        yield return new WaitForSeconds(animationDuration);

        // Clean up all created objects
        foreach (GameObject obj in celebrationObjects)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }
        celebrationObjects.Clear();

        isPlaying = false;


    }

    /// <summary>
    /// Creates an animated star at a random position on the screen.
    /// </summary>
    private GameObject CreateAnimatedStar()
    {
        // Create a new GameObject for the star
        GameObject starObj = new GameObject("CelebrationStar");

        // Add a sprite renderer
        SpriteRenderer renderer = starObj.AddComponent<SpriteRenderer>();
        renderer.sprite = goldStarSprite;
        renderer.sortingOrder = 100; // Ensure it's rendered on top

        // Position randomly on screen in camera view
        Camera mainCamera = Camera.main;
        float heightHalf = mainCamera.orthographicSize;
        float widthHalf = heightHalf * mainCamera.aspect;

        // Keep slightly away from edges
        float edgeBuffer = 1.0f;
        float randomX = Random.Range(-widthHalf + edgeBuffer, widthHalf - edgeBuffer);
        float randomY = Random.Range(-heightHalf + edgeBuffer, heightHalf - edgeBuffer);

        starObj.transform.position = new Vector3(randomX, randomY, -1f); // Slightly in front of other elements

        // Start the animation
        StartCoroutine(AnimateStar(starObj));

        return starObj;
    }

    /// <summary>
    /// Creates a sparkle effect at a position with a random offset.
    /// </summary>
    private GameObject CreateSparkle(Vector3 centerPosition)
    {
        // Create a new GameObject for the sparkle
        GameObject sparkleObj = new GameObject("Sparkle");

        // Add a sprite renderer
        SpriteRenderer renderer = sparkleObj.AddComponent<SpriteRenderer>();
        renderer.sprite = whiteSparkleSprite;
        renderer.sortingOrder = 101; // Above the stars

        // Position with a random offset from the center
        float offsetX = Random.Range(-0.5f, 0.5f);
        float offsetY = Random.Range(-0.5f, 0.5f);

        sparkleObj.transform.position = new Vector3(
            centerPosition.x + offsetX,
            centerPosition.y + offsetY,
            centerPosition.z - 0.1f); // Slightly in front of stars

        // Start the animation
        StartCoroutine(AnimateSparkle(sparkleObj));

        return sparkleObj;
    }

    /// <summary>
    /// Animates a star with scaling, rotation, and fade effects.
    /// </summary>
    private IEnumerator AnimateStar(GameObject star)
    {
        SpriteRenderer renderer = star.GetComponent<SpriteRenderer>();
        float duration = 1.5f;
        float elapsed = 0f;

        // Initial state
        star.transform.localScale = Vector3.zero;
        renderer.color = new Color(1f, 1f, 1f, 0f);

        // Grow and fade in
        while (elapsed < duration * 0.4f)
        {
            float t = elapsed / (duration * 0.4f);
            float scale = Mathf.Sin(t * Mathf.PI * 0.5f) * 1.5f; // Easing function
            star.transform.localScale = new Vector3(scale, scale, 1f);
            renderer.color = new Color(1f, 1f, 1f, t);
            star.transform.Rotate(0, 0, 90f * Time.deltaTime); // Spin

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Hold at full size
        float holdDuration = 0.8f;
        elapsed = 0f;
        while (elapsed < holdDuration)
        {
            star.transform.Rotate(0, 0, 45f * Time.deltaTime); // Slower spin
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Shrink and fade out
        elapsed = 0f;
        while (elapsed < duration * 0.3f)
        {
            float t = elapsed / (duration * 0.3f);
            float scale = Mathf.Cos(t * Mathf.PI * 0.5f) * 1.5f; // Easing function
            star.transform.localScale = new Vector3(scale, scale, 1f);
            renderer.color = new Color(1f, 1f, 1f, 1f - t);
            star.transform.Rotate(0, 0, 120f * Time.deltaTime); // Fast spin

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Animation complete
        star.transform.localScale = Vector3.zero;
        renderer.color = new Color(1f, 1f, 1f, 0f);
    }

    /// <summary>
    /// Animates a sparkle with pulsing and fade effects.
    /// </summary>
    private IEnumerator AnimateSparkle(GameObject sparkle)
    {
        SpriteRenderer renderer = sparkle.GetComponent<SpriteRenderer>();
        float duration = 0.7f;
        float elapsed = 0f;

        // Random rotation for variety
        sparkle.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

        // Initial state
        sparkle.transform.localScale = Vector3.zero;
        renderer.color = new Color(1f, 1f, 1f, 0f);

        // Grow and fade in
        while (elapsed < duration * 0.3f)
        {
            float t = elapsed / (duration * 0.3f);
            float scale = t * 0.7f; // Start smaller than stars
            sparkle.transform.localScale = new Vector3(scale, scale, 1f);
            renderer.color = new Color(1f, 1f, 1f, t * 0.8f); // Not fully opaque

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Pulse
        elapsed = 0f;
        float pulseDuration = duration * 0.4f;
        while (elapsed < pulseDuration)
        {
            float t = elapsed / pulseDuration;
            float pulseScale = 0.7f + Mathf.Sin(t * Mathf.PI) * 0.3f;
            sparkle.transform.localScale = new Vector3(pulseScale, pulseScale, 1f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Fade out
        elapsed = 0f;
        while (elapsed < duration * 0.3f)
        {
            float t = elapsed / (duration * 0.3f);
            float scale = 0.7f * (1 - t);
            sparkle.transform.localScale = new Vector3(scale, scale, 1f);
            renderer.color = new Color(1f, 1f, 1f, 0.8f * (1 - t));

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Animation complete
        sparkle.transform.localScale = Vector3.zero;
        renderer.color = new Color(1f, 1f, 1f, 0f);
    }

    /// <summary>
    /// Static method to play celebration from anywhere.
    /// </summary>
    public static void PlayWinAnimation()
    {
        if (Instance != null)
        {
            Instance.PlayCelebration();
        }
        else
        {
            Debug.LogError("CelebrationManager instance not found!");
        }
    }
}