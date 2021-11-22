using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static int NumberOfMoves = 0;
	public static int PuzzlesSolved = 0;
	public static DateTime LevelStartTime;

	public static int levelHardnessChangability;
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
	
	 static bool levelCompleted = false;
	 public static bool IsLevelCompleted() => levelCompleted;

	 public static GameObject Menu;
	 public static GameObject HomeButton;
	
	public void Start()
	{
		HomeButton = GameObject.FindWithTag("homeButton");
		HomeButton.SetActive(false);
		
		LevelCompletedAudio = GameObject.FindWithTag("levelCompletedAudio").GetComponent<AudioSource>();
		FigureUpAudio = GameObject.FindWithTag("figureUpAudio").GetComponent<AudioSource>();
		FigureDownAudio = GameObject.FindWithTag("figureDownAudio").GetComponent<AudioSource>();
		ButtonClickAudio = GameObject.FindWithTag("buttonClickAudio").GetComponent<AudioSource>();
		GameProgressText = GameObject.FindWithTag("gameProgress").GetComponent<Text>();

		Menu = GameObject.FindWithTag("menu");

		PuzzlesSolved = PlayerPrefs.GetInt(nameof(PuzzlesSolved));
		var maxSize = PlayerPrefs.GetInt(nameof(Settings.MaxSize));
		if (maxSize > 0)
		{
			Settings.MaxSize = maxSize;
		}
	}
	

	public static void StartLevel()
	{
		FigureHandler.ResetZ();
		NumberOfMoves = 0;
		LevelStartTime = DateTime.Now;
		TrackProgress();
		levelCompleted = false;
	}
	
	public static void CompleteLevel()
	{
		PuzzlesSolved++;
		levelCompleted = true;
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

				levelHardnessChangability--;
			}
			else
			{
				if (IsEasy())
				{
					DoHard();
					levelHardnessChangability++;
				}
				else if (levelHardnessChangability == 5)
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

	public static class Settings
	{
		public static int Xlength = 6; 
		public static int Ylength = 4;
		public static int FigureN = 8;
		public static int MinSize = 3;
		public static int MaxSize = 10;
	}
}
