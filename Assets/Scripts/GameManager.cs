using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	public static int NumberOfMoves = 0;
	public static int PuzzlesSolved = 0;
	public static DateTime LevelStartTime;
	public static bool IsFirstGame = true;

	public static int levelHardnessChangability;
	public static AudioSource LevelCompletedAudio;
	public static AudioSource FigureUpAudio;
	public static AudioSource FigureDownAudio;
	public static AudioSource ButtonClickAudio;
	public static Text GameProgressText;
	
	static bool IsEasy() {return FiguresGenerator.maxSize == 10; }
	static bool IsHard() {return FiguresGenerator.maxSize == 9; }
	static bool IsExpert() {return FiguresGenerator.maxSize == 8; }

	static void DoEasy() { FiguresGenerator.maxSize = 10; }
	static void DoHard() { FiguresGenerator.maxSize = 9; }
	static void DoExpert() { FiguresGenerator.maxSize = 8; }
	
	 static bool levelCompleted = false;
	 public static bool IsLevelCompleted() => levelCompleted;
	
	public void Start()
	{
		LevelCompletedAudio = GameObject.FindWithTag("levelCompletedAudio").GetComponent<AudioSource>();
		FigureUpAudio = GameObject.FindWithTag("figureUpAudio").GetComponent<AudioSource>();
		FigureDownAudio = GameObject.FindWithTag("figureDownAudio").GetComponent<AudioSource>();
		ButtonClickAudio = GameObject.FindWithTag("buttonClickAudio").GetComponent<AudioSource>();
		GameProgressText = GameObject.FindWithTag("gameProgress").GetComponent<Text>();

		PuzzlesSolved = PlayerPrefs.GetInt(nameof(PuzzlesSolved));
		var maxSize = PlayerPrefs.GetInt(nameof(FiguresGenerator.maxSize));
		if (maxSize > 0)
		{
			FiguresGenerator.maxSize = maxSize;
		}
	}
	

	public static void StartLevel()
	{
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
		//Debug.Log(hhmmss);

		AdjustHardnessLevel(timeSpent);

		PlayerPrefs.SetInt(nameof(FiguresGenerator.maxSize), FiguresGenerator.maxSize);
		PlayerPrefs.SetInt(nameof(PuzzlesSolved), PuzzlesSolved);
		PlayerPrefs.Save();
	}

	private static void AdjustHardnessLevel(TimeSpan timeSpent)
	{
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
		var level = 11 - FiguresGenerator.maxSize;
		GameProgressText.text = ""; //$"puzzles solved: {PuzzlesSolved}, current moves: {NumberOfMoves}, l:{level}, c:{levelHardnessChangability}";
	}
}
