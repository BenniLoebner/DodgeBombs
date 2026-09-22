using UnityEngine;
using TMPro;

//sits on the HUD score Text object; registers itself with the persistent ScoreManager
//every scene load, since ScoreManager survives restarts but this Text object doesn't
public class ScoreTextUI : MonoBehaviour
{
    //Start, not Awake: Awake order across GameObjects isn't deterministic, and this needs
    //ScoreManager.Instance to already be assigned (which happens in ScoreManager.Awake)
    void Start()
    {
        ScoreManager.Instance.RegisterScoreText(GetComponent<TMP_Text>());
    }
}
