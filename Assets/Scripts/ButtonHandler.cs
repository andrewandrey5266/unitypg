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
        GameManager.ButtonClickAudio.Play(0);
        GameManager.Menu.SetActive(false);
        GameManager.FilledPercentage.SetActive(true);
        GameManager.HomeButton.SetActive(true);
        var script = GameObject.Find("Generator").GetComponent<FiguresGenerator>();
        script.Generate();
        GameManager.StartLevel();
    }

    public void CompletedPuzzlesButtonHandler()
    {
        GameManager.ButtonClickAudio.Play(0);
        GameManager.Menu.SetActive(false);
        GameManager.CompletedPuzzlesMenu.SetActive(true);
        
        GameManager.HomeButton.SetActive(true);

    }
}
