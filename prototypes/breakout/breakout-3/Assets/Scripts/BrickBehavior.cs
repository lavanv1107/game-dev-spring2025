using UnityEngine;
using System.Collections;

public class BrickBehavior : MonoBehaviour
{
    public Color originalColor;
    private Renderer brickRenderer;
    private Coroutine flashingCoroutine;
    private bool isFlashing = false;
    private Color currentFlashColor;  
    
    void Awake()
    {
        brickRenderer = GetComponent<Renderer>();
    }

    public void StartFlashing(Color flashColor)
    {
        isFlashing = true;
        currentFlashColor = flashColor;  // Store the flash color
        if (flashingCoroutine != null)
        {
            StopCoroutine(flashingCoroutine);
        }
        flashingCoroutine = StartCoroutine(FlashCoroutine());
    }

    public IEnumerator FlashCoroutine() 
    {
        while (isFlashing)
        {
            float lerp = Mathf.PingPong(Time.time * 2f, 1f);
            brickRenderer.material.color = Color.Lerp(originalColor, currentFlashColor, lerp);
            yield return null;
        }
    }

    public void StopFlashing()  // New method
    {
        isFlashing = false;
        if (flashingCoroutine != null)
        {
            StopCoroutine(flashingCoroutine);
            flashingCoroutine = null;
        }
        if (brickRenderer != null)
        {
            brickRenderer.material.color = originalColor;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("ball") && isFlashing)
        {
            // Find and scale the sword
            GameObject sword = GameObject.FindWithTag("sword");
            if (sword != null)
            {
                StartCoroutine(ScaleSwordEffect(sword));
            }
            
            // Handle brick destruction
            Destroy(gameObject);
        }
    }

    private IEnumerator ScaleSwordEffect(GameObject sword)
    {
        // Store the original scale
        Vector3 originalScale = sword.transform.localScale;
        Vector3 targetScale = originalScale * 20f; // Increase scale by 50%
        float duration = 2f;
        
        // Scale up over 1 second
        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / 1f;
            sword.transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
            yield return null;
        }

        // Wait a tiny bit at max scale
        yield return new WaitForSeconds(0.1f);
        
        // Scale back down over 1 second
        elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / 1f;
            sword.transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
            yield return null;
        }

        // Make absolutely sure we return to the original scale
        sword.transform.localScale = originalScale;
        Debug.Log("Sword scale reset complete"); // Add debug log
    }
}