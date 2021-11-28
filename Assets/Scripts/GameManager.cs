using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static class Settings
	{
		public static int Xlength; 
		public static int Ylength;
		public static int MinSize = 2;

		public static int FigureN = 9;
		public static int MaxSize = 15;
		public static bool ExactAmountOfFigures;
	}
	
	public static int NumberOfMoves = 0;
	public static int PuzzlesSolved = 0;
	public static DateTime LevelStartTime;

	public static int LevelHardnessValue;
	public static AudioSource LevelCompletedAudio;
	public static AudioSource FigureUpAudio;
	public static AudioSource FigureDownAudio;
	public static AudioSource ButtonClickAudio;
	public static Text GameProgressText;

	 static bool _levelCompleted;
	 public static bool IsLevelCompleted => _levelCompleted;

	 public static GameObject Menu;
	 public static GameObject HomeButton;
	 public static GameObject NextLevelButton;
	 public static GameObject LevelCompletedText;

	 public static GameObject SoundOnButton;
	 public static GameObject SoundOffButton;
	 public static GameObject BackgroundArea;
	 public static GameObject FilledPercentage;
	 
	 public static bool SoundOn = true;

	 public static List<GameObject> Figures = new List<GameObject>();
	 
	 public static int Level;

	public void Start()
	{
		HomeButton = GameObject.FindWithTag("homeButton");
		NextLevelButton = GameObject.FindWithTag("generateLevelButton");
		LevelCompletedText = GameObject.FindWithTag("levelCompletedText");
		SoundOnButton = GameObject.FindWithTag("soundOnButton");
		SoundOffButton = GameObject.FindWithTag("soundOffButton");
		FilledPercentage = GameObject.FindWithTag("filledPercentage");

		SoundOffButton.SetActive(false);
		NextLevelButton.SetActive(false);
		HomeButton.SetActive(false);
		LevelCompletedText.SetActive(false);
		FilledPercentage.SetActive(false);
		
		LevelCompletedAudio = GameObject.FindWithTag("levelCompletedAudio").GetComponent<AudioSource>();
		FigureUpAudio = GameObject.FindWithTag("figureUpAudio").GetComponent<AudioSource>();
		FigureDownAudio = GameObject.FindWithTag("figureDownAudio").GetComponent<AudioSource>();
		ButtonClickAudio = GameObject.FindWithTag("buttonClickAudio").GetComponent<AudioSource>();
		GameProgressText = GameObject.FindWithTag("gameProgress").GetComponent<Text>();
		BackgroundArea = GameObject.FindWithTag("backgroundArea");

		var backgroundSize = BackgroundArea.transform.localScale;
		Settings.Xlength = (int) backgroundSize.x;
		Settings.Ylength = (int) backgroundSize.y;

		Menu = GameObject.FindWithTag("menu");

		PuzzlesSolved = PlayerPrefs.GetInt(nameof(PuzzlesSolved));
		Level = PlayerPrefs.GetInt(nameof(Level));
		
		//trying to disable multitouch here, to omit figure drawing but
		Input.multiTouchEnabled = false;

		AdjustLevelSettings();
		ArrowMovement.LoadSavedLevel();
	}
	
	public static void StartLevel()
	{
		FilledPercentage.GetComponent<Text>().text = "Area filled 0%";
		NumberOfMoves = 0;
		LevelStartTime = DateTime.Now;
		_levelCompleted = false;
	}
	
	public static void CompleteLevel()
	{
		PuzzlesSolved++;
		_levelCompleted = true;

		PlayerPrefs.SetInt(nameof(PuzzlesSolved), PuzzlesSolved);
		PlayerPrefs.Save();
	}

	public static void AdjustLevelSettings()
	{
		if (Level == 0)
		{
			Settings.MaxSize = 15;
			Settings.FigureN = 4;
			Settings.ExactAmountOfFigures = true;
		}
		if (Level == 1)
		{
			Settings.MaxSize = 8;
			Settings.FigureN = 5;
			Settings.ExactAmountOfFigures = true;
		}
		if (Level == 2)
		{
			Settings.MaxSize = 5;
			Settings.FigureN = 7;
			Settings.ExactAmountOfFigures = true;
		}
		if (Level == 3)
		{
			Settings.MaxSize = 5;
			Settings.FigureN = 9;
			Settings.ExactAmountOfFigures = true;
		}
	}
	public static void SaveLevelHardness()
	{
		PlayerPrefs.SetInt(nameof(Level), Level);
		PlayerPrefs.Save();

		AdjustLevelSettings();
	}

	public static bool CheckLevelCompleted()
	{
		if (BackgroundArea != null)
		{
			var scale = BackgroundArea.transform.localScale;
			var position = BackgroundArea.transform.position;

			float startX =  position.x - scale.x / 2;
			float startY =  position.y - scale.y / 2;
			float endX =  startX + scale.x;
			float endY =  startY + scale.y;

			float sizeOfSquare = 1f;
			float stepOffset = 0.5f;

			int filled = 0, notFilled = 0;
            

			for (float y = startY; y < endY; y += sizeOfSquare)
			{
				for (float x = startX; x < endX; x += sizeOfSquare)
				{
					Vector3 rayCastPosition = new Vector3(x + stepOffset, y + stepOffset, -2.5f);
					if (Physics.Raycast(rayCastPosition, Vector3.forward, out RaycastHit hit, 5.0f))
					{
						if (hit.collider.gameObject.CompareTag(BackgroundArea.tag))
						{
							notFilled++;
						}
						else
						{
							filled++;
						}
					}
				}
			}

			if (filled + notFilled > 0)
			{
				int percentage = filled * 100 / (filled + notFilled);
				FilledPercentage.GetComponent<Text>().text = $"Area filled {percentage}%";
			}

			return notFilled <= 0;
		}

		return false;
	}
}