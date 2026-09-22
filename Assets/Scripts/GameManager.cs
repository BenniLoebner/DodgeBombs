using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameObject endScreen; // Reference to the end screen UI
    public int delayBeforeEndScreen = 2; // Delay in seconds before showing the end screen

    void Awake()
    {
        //persists across restarts so score/highscore tracking survives - scene-local references
        //like endScreen are re-supplied by EndScreenUI.RegisterEndScreen each time the scene (re)loads
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
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void RegisterEndScreen(GameObject screen)
    {
        endScreen = screen;
    }

    public void gameOver()
    {
        // Logic to handle game over, e.g., show game over screen
        Debug.Log("Game Over!");
        ScoreManager.Instance.DisableScoring(); // Disable scoring when the game is over
        // You can also load a game over scene or restart the game here
        StartCoroutine(ShowEndScreenCoroutine());
    }

    private IEnumerator ShowEndScreenCoroutine()
    {
        // Wait for a short duration before showing the end screen
        yield return new WaitForSeconds(delayBeforeEndScreen);

        endScreen.SetActive(true); // Show the end screen UI

        //let the player click the Restart/Quit buttons instead of staying locked for gameplay
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void RestartGame()
    {
        //cancel any still-pending end-screen delay from a previous death - GameManager
        //survives scene reloads, so its coroutines do too unless stopped explicitly
        StopAllCoroutines();

        // Logic to restart the game, e.g., reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        ScoreManager.Instance.ResetScore(); // Reset current score, but keep the high score
        ScoreManager.Instance.EnableScoring(); // Enable scoring when the game is restarted
    }

    public void Quit()
    {
        Application.Quit();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
