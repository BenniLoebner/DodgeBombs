using UnityEngine;

public class HighScoreTextUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ScoreManager.Instance.RegisterHighScoreText(GetComponent<TMPro.TMP_Text>());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
