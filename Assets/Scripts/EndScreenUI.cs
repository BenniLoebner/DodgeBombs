using UnityEngine;
using UnityEngine.UI;

//sits on the EndScreen root; registers itself with the persistent GameManager every scene
//load, since GameManager survives restarts but this UI object doesn't. Also wires the
//buttons in code instead of a serialized OnClick target, because a serialized target would
//point at whichever GameManager instance existed in THIS scene load - which self-destroys
//as a duplicate of the persistent singleton, leaving the button's target missing after
//the very first restart.
public class EndScreenUI : MonoBehaviour
{
    public Button restartButton;
    public Button quitButton;

    //Start, not Awake: Awake order across GameObjects isn't deterministic, and this needs
    //GameManager.Instance to already be assigned (which happens in GameManager.Awake)
    void Start()
    {
        restartButton.onClick.AddListener(() => GameManager.Instance.RestartGame());
        quitButton.onClick.AddListener(() => GameManager.Instance.Quit());

        GameManager.Instance.RegisterEndScreen(gameObject);
        gameObject.SetActive(false);
    }
}
