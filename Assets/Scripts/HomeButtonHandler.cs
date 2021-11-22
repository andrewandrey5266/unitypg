using UnityEngine;

public class HomeButtonHandler : MonoBehaviour
{
    public void HandleClick()
    {
        GameManager.ButtonClickAudio.Play(0);
        gameObject.SetActive(false);
        GameManager.Menu.SetActive(true);
    }
}
