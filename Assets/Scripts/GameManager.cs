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
		public static int FigureN = 9;
		public static int MinSize = 2;
		public static int MaxSize = 15;

		public static bool ExactAmountOfFigures = false;
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

	static bool IsEasy() {return Settings.MaxSize == 10; }
	static bool IsHard() {return Settings.MaxSize == 9; }
	static bool IsExpert() {return Settings.MaxSize == 8; }
	public static void DoEasy() { Settings.MaxSize = 10; }
	public static void DoHard() { Settings.MaxSize = 9; }
	public static void DoExpert() { Settings.MaxSize = 6; }

	public static bool IsLevelHardnessAdjustable = true;
	
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
		var maxSize = PlayerPrefs.GetInt(nameof(Settings.MaxSize));
		if (maxSize > 0)
		{
			Settings.MaxSize = maxSize;
		}
		
		//trying to disable multitouch here, to omit figure drawing but
		Input.multiTouchEnabled = false;
	}
	
	public static void StartLevel()
	{
		FilledPercentage.GetComponent<Text>().text = "Area filled by: 0 %";
		NumberOfMoves = 0;
		LevelStartTime = DateTime.Now;
		TrackProgress();
		_levelCompleted = false;
	}
	
	public static void CompleteLevel()
	{
		PuzzlesSolved++;
		_levelCompleted = true;
		TrackProgress();

		var timeSpent = (DateTime.Now - LevelStartTime);
		var hhmmss = $"{timeSpent.Hours}:{timeSpent.Minutes}:{timeSpent.Seconds}";
		
		if (timeSpent.Days > 1)
		{
			hhmmss = $"{timeSpent.Days} day(s)";
		}

		AdjustHardnessLevel(timeSpent);

		PlayerPrefs.SetInt(nameof(Settings.MaxSize), Settings.MaxSize);
		PlayerPrefs.SetInt(nameof(PuzzlesSolved), PuzzlesSolved);
		PlayerPrefs.Save();
	}

	private static void AdjustHardnessLevel(TimeSpan timeSpent)
	{
		if (!IsLevelHardnessAdjustable)
		{
			return;
		}
		if (PuzzlesSolved >= 17)
		{
			if (timeSpent.TotalSeconds > 60)
			{
				if (IsExpert())
				{
					DoHard();
				}else if (IsHard())
				{
					DoEasy();
				}

				LevelHardnessValue--;
			}
			else
			{
				if (IsEasy())
				{
					DoHard();
					LevelHardnessValue++;
				}
				else if (LevelHardnessValue == 5)
				{
					DoExpert();
				}
			}
		}
		else if (PuzzlesSolved >= 15)
		{
			DoExpert();
		}
		else if (PuzzlesSolved >= 5)
		{
			DoHard();
		}
	}

	public static void TrackProgress()
	{
		//expert -3, hard - 2, easy - 1
		var level = 11 - Settings.MaxSize;
		GameProgressText.text =
			""; //$"puzzles solved: {PuzzlesSolved}, current moves: {NumberOfMoves}, l:{level}, c:{levelHardnessChangability}";
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
				FilledPercentage.GetComponent<Text>().text = $"Area filled by: {percentage} %";
			}

			return notFilled <= 0;
		}

		return false;
	}
}