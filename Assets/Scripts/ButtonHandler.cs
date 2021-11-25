using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public void Generate()
    {
        GameManager.LevelCompletedText.SetActive(false);

        gameObject.SetActive(false);
        
        GameManager.ButtonClickAudio.Play(0);

        var script = GameObject.Find("Generator").GetComponent<FiguresGenerator>();
        script.Generate();
        GameManager.StartLevel();
    }

    public void GenerateRegular()
    {
        GameManager.IsLevelHardnessAdjustable = false;
        GameManager.DoEasy();
        MenuButtonBase();
    }

    public void GenerateHard()
    {
        GameManager.IsLevelHardnessAdjustable = false;
        GameManager.DoExpert();
        MenuButtonBase();
    }

    private void MenuButtonBase()
    {
        GameManager.Menu.SetActive(false);
        GameManager.HomeButton.SetActive(true);
        GameManager.ButtonClickAudio.Play(0);
        var script = GameObject.Find("Generator").GetComponent<FiguresGenerator>();
        script.Generate();
        GameManager.StartLevel();
    }
}
