using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{

    public static ScoreManager Instance;
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public bool scoringEnabled = true; // Flag to enable or disable scoring

    private int score = 0;
    public int highScore = 0;

    void Awake()
    {
        //persists across restarts so the high score survives - scoreText is scene-local and
        //re-supplied by ScoreTextUI.RegisterScoreText each time the scene (re)loads
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

    public void RegisterScoreText(TMP_Text text)
    {
        scoreText = text;
        scoreText.text = score.ToString();
    }

    public void RegisterHighScoreText(TMP_Text text)
    {
        highScoreText = text;
        highScoreText.text = "HIGH SCORE: " + highScore.ToString();
    }

    public void AddScore(int amount)
    {
        if (!scoringEnabled)
            return;
        score += amount;
        if (score > highScore)
        {
            highScore = score;
        }
        scoreText.text = score.ToString();
        highScoreText.text = "HIGH SCORE: " + highScore.ToString();
    }

    public void ResetScore()
    {
        score = 0;
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void DisableScoring()
    {
        scoringEnabled = false;
    }
    public void EnableScoring()
    {
        scoringEnabled = true;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
