using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHandler : MonoBehaviour
{
    public void Generate()
    {
        Resources
            .FindObjectsOfTypeAll<Text>()
            .First(x => x.CompareTag($"levelCompletedText"))
            .gameObject.SetActive(false);
        gameObject.SetActive(false);
        GameManager.ButtonClickAudio.Play(0);

        if (GameManager.IsFirstGame)
        {
            gameObject.GetComponentInChildren<Text>().text = "n e x t  l e v e l";
            GameManager.IsFirstGame = false;
            Resources
                .FindObjectsOfTypeAll<Text>()
                .First(x => x.CompareTag($"welcomeText"))
                .gameObject.SetActive(false);
        }
        
        
        var script = GameObject.Find("Generator").GetComponent<FiguresGenerator>();
        script.Generate();
        GameManager.StartLevel();
    }
}
