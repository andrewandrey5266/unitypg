using UnityEngine;
using UnityEngine.UI;
using GM = GameManager;

public class ArrowMovement : MonoBehaviour
{
   private static float[] _levelAngles = { 60f, 25f, -25f, -60f };
   private static string[] _levelTexts = { "e a s y", "m e d i u m", "h a r d", "e x p e r t" };
   private static Transform _arrowTransform;
   private static Text _levelText;
   
   private bool _change;
   private bool clockWise = true;

   public void Start()
   {
      _arrowTransform = GameObject.FindWithTag("tabloArrow").GetComponent<Transform>();
      _levelText = GameObject.FindWithTag("tabloText").GetComponent<Text>();
      
      LoadSavedLevel();
   }

   public void ChangeLevel()
   {
      if (_change)
         return;
      
      if (GM.Level < 3)
      {
         GM.Level++; 
         clockWise = true;
      }
      else
      {
         GM.Level = 0;
         clockWise = false;
      }
      GM.ButtonClickAudio.Play(0);
      _change = true;
      GM.SaveLevelHardness();
   }
   
   public void Update()
   {
      if (_change)
      {
         if (clockWise && _arrowTransform.rotation.z * 100 < _levelAngles[GM.Level]
             || !clockWise && _arrowTransform.rotation.z * 100 > _levelAngles[GM.Level])
         {
            _change = false;
            _levelText.text = _levelTexts[GM.Level];
            return;
         }
         Vector3 direction = clockWise ? Vector3.back : Vector3.forward;
         float angle = clockWise ? 1.5f : 3.0f;
         _arrowTransform.Rotate(direction, angle);
      }
   }

   public static void LoadSavedLevel()
   {
      _arrowTransform.rotation = Quaternion.Euler(0, 0, _levelAngles[GM.Level]);
      _levelText.text = _levelTexts[GM.Level];
   }
}
