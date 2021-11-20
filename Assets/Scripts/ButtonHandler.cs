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

        var menu = GameObject.FindGameObjectWithTag("menu");
        if (menu != null && menu.activeSelf)
        {
            GameObject.FindGameObjectWithTag("menu").SetActive(false);
        }
        else
        {
            gameObject.SetActive(false);
        }
        
        GameManager.ButtonClickAudio.Play(0);

        if (GameManager.IsFirstGame)
        {
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
