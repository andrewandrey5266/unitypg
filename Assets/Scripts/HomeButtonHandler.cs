using System;
using UnityEngine;

public class HomeButtonHandler : MonoBehaviour
{
    public void HandleClick()
    {
        GameManager.ButtonClickAudio.Play(0);
        gameObject.SetActive(false);
        GameManager.NextLevelButton.SetActive(false);
        GameManager.LevelCompletedText.SetActive(false);
        GameManager.Menu.SetActive(true);
    }

    public void HandleSoundButtonClick()
    {
        
        GameManager.SoundOn = !GameManager.SoundOn;
        AudioListener.volume = Convert.ToSingle(GameManager.SoundOn);
        GameManager.SoundOnButton.SetActive(GameManager.SoundOn);
        GameManager.SoundOffButton.SetActive(!GameManager.SoundOn);
    }
}
