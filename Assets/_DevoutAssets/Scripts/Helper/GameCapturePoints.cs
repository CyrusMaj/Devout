using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameCapturePoints
{
	public int Team_1_Score = 0;
	public int Team_2_Score = 0;
	public int CurrentRound;
	public int MaxRound;
	/// <summary>
	/// Max round time in minutes
	/// </summary>
	public float MaxTimer;
	public STATE State;

	public const int TIMER_COUNTER_ROUND = 5;
	public const int TIMER_COUNTER_GAME = 10;

	public enum STATE
	{
		PREPARED,
		IN_PROGRESS,
		ROUND_OVER,
		ROUND_READY_WAITING,
		GAME_OVER,
		NOT_STARTED
	}

	List<Round> Rounds;

	public GameCapturePoints ()
	{
		CurrentRound = 0;
		MaxRound = 5;
		State = STATE.NOT_STARTED;
		Rounds = new List<Round> ();
		MaxTimer = 4f;
		Round r = new Round ();
		r.Number = 0;
		r.Timer = 0f;
		r.Winner = TEAM.NULL;
		Rounds.Add (r);
	}

//	public void SetCurrentRoundTimer (float timer)
//	{
////		Rounds [CurrentRound].Timer = timer;
//		Rounds.Find (x => x.Number == CurrentRound).Timer = timer;
//	}

//	public void SetCurrentRoundWinner (TEAM winner)
//	{
//		Rounds.Find (x => x.Number == CurrentRound).Winner = winner;
////		Rounds[CurrentRound].Winner = winner;
//	}

	public Round GetCurrentRound ()
	{
		return Rounds.Find (x => x.Number == CurrentRound);
	}

	public int GetScore (TEAM team)
	{
		return Rounds.Where (x => x.Winner == team).Count ();
	}

	public void SetUpNewRound ()
	{
		CurrentRound++;
		State = STATE.PREPARED;
		Round r = new Round ();
		r.Number = CurrentRound;
		r.Timer = MaxTimer * 60;
		r.Winner = TEAM.NULL;
		Rounds.Add (r);
	}

	public class Round
	{
		public int Number;
		public float Timer;
		public TEAM Winner;
	}
}
