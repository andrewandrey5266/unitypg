using System.Linq;
using System.Threading;
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
        MenuButtonBase();
    }

    private void MenuButtonBase()
    {
        GameManager.ButtonClickAudio.Play(0);
        GameManager.Menu.SetActive(false);
        GameManager.FilledPercentage.SetActive(true);
        GameManager.HomeButton.SetActive(true);
        var script = GameObject.Find("Generator").GetComponent<FiguresGenerator>();
        script.Generate();
        GameManager.StartLevel();
    }
}
