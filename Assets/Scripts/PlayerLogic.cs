using UnityEngine;
using System.Collections;

public class PlayerLogic : MonoBehaviour
{

    private float puRunDuration = 5f; // Duration of the power-up effect

    //only one power-up effect can ever be active - picking up another one while
    //active just refreshes the remaining time instead of stacking the effect again
    private bool isPowerUpActive = false;
    private float powerUpTimeRemaining = 0f;
    private bool IsInvincible => isPowerUpActive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    void OnDisable()
    {
        //the Player is scene-local, so a scene reload destroys us mid-effect and kills the
        //coroutine before it reaches EndFleeEffect - Enemy's flee count is static and would
        //stay stuck above zero, leaving every enemy fleeing for the rest of the session
        if (isPowerUpActive)
        {
            Enemy.EndFleeEffect();
            isPowerUpActive = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Explosion"))
        {
            if (IsInvincible)
            {
                Debug.Log("Explosion ignored - player is invincible!");
                return;
            }

            // Logic to handle player collision with explosion
            Debug.Log("Player hit by explosion!");
            if (GameManager.Instance != null)
            {
                GetComponent<PlayerMovement>().enabled = false; // Disable player movement
                GameManager.Instance.gameOver();
            }
            else
            {
                Debug.LogWarning("GameManager instance is null. Cannot call gameOver().");
            }
        }
        else if (other.CompareTag("PURun"))
        {
            // Logic to handle player collision with points
            Debug.Log("Player collected Power Up Run!");
            // The actual score increment is handled in the Points script
            powerUpTimeRemaining = puRunDuration;

            if (!isPowerUpActive)
            {
                StartCoroutine(PowerUpRunEffectCoroutine());
            }
        }
    }

    private IEnumerator PowerUpRunEffectCoroutine()
    {
        Debug.Log("Power Up Run effect started!");
        isPowerUpActive = true;
        Enemy.BeginFleeEffect();
        transform.localScale *= 2f;

        //count down each frame instead of a single fixed wait, so a pickup mid-effect
        //(which just resets powerUpTimeRemaining) extends this same run instead of stacking
        while (powerUpTimeRemaining > 0f)
        {
            powerUpTimeRemaining -= Time.deltaTime;
            yield return null;
        }

        // Disable the power-up effect
        transform.localScale /= 2f;
        Enemy.EndFleeEffect();
        isPowerUpActive = false;
        Debug.Log("Power Up Run effect ended!");
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
