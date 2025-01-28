using System;
using System.Collections;
using UnityEngine;

public class BasketSensor : MonoBehaviour
{
    public Action OnBasket;
    private GameObject entered;
    private bool isBasketScored = false;

    [SerializeField] private ParticleSystem basketVFX; // Reference to the VFX particle system
    [SerializeField] private float vfxDuration = 1f;   // Duration for the VFX to play
    [SerializeField] private float fadeOutDuration = 0.25f; // Fade out duration

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Simulation"))
        {
            return;
        }

        if (other.CompareTag("Ball") && entered == null)
        {
            entered = other.gameObject;
            isBasketScored = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Simulation"))
        {
            return;
        }

        if (other.gameObject.Equals(entered) && !isBasketScored)
        {
            OnBasket?.Invoke();
            isBasketScored = true;
            entered = null;

            // Play the basket VFX
            playBasketVFX();
        }
    }

    private void playBasketVFX()
    {
        if (basketVFX != null)
        {
            // Start the particle system
            basketVFX.Play();

            // Start a coroutine to stop the VFX after the duration
            StartCoroutine(FadeOutVFX());
        }
        else
        {
            Debug.LogWarning("Basket VFX is not assigned!");
        }
    }

    private IEnumerator FadeOutVFX()
    {
        // Wait for the VFX duration
        yield return new WaitForSeconds(vfxDuration);

        // Get the particle system's main module
        var main = basketVFX.main;
        float initialStartSize = main.startSize.constant;
        float initialStartAlpha = main.startColor.color.a;

        float elapsedTime = 0f;

        // Gradually fade out the particle system
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;

            // Lerp the size and alpha
            float newStartSize = Mathf.Lerp(initialStartSize, 0, elapsedTime / fadeOutDuration);
            float newStartAlpha = Mathf.Lerp(initialStartAlpha, 0, elapsedTime / fadeOutDuration);

            // Update the particle system settings
            main.startSize = newStartSize;
            main.startColor = new Color(
                main.startColor.color.r,
                main.startColor.color.g,
                main.startColor.color.b,
                newStartAlpha
            );

            yield return null; // Wait for the next frame
        }

        // Stop the particle system completely
        basketVFX.Stop();
    }
}
