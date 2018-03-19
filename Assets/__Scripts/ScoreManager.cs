﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// An enum to handle all the possible scoring events
public enum eScoreEvent {
	draw,
	mine,
	mineGold,
	gameWin,
	gameLoss
}

// ScoreManager handles all of the scoring
public class ScoreManager : MonoBehaviour {                 // a
	static private ScoreManager S;                            // b

	public Button share;
	public string name="check out Prospector Solitaire";
	private const string twitter = "http://twitter.com/intent/tweet";


	static public int   SCORE_FROM_PREV_ROUND = 0;
	static public int   HIGH_SCORE = 0;

	[Header("Set Dynamically")]
	// Fields to track score info
	public int chain = 0;
	public int scoreRun = 0;
	public int score = 0;

	void Awake() {
		if (S == null) {                                        // c
			S = this; // Set the private singleton 
		} else {
			Debug.LogError("ERROR: ScoreManager.Awake(): S is already set!");
		}

		// Check for a high score in PlayerPrefs
		if (PlayerPrefs.HasKey ("ProspectorHighScore")) {
			HIGH_SCORE = PlayerPrefs.GetInt("ProspectorHighScore");
		}
		// Add the score from last round, which will be >0 if it was a win
		score += SCORE_FROM_PREV_ROUND;
		// And reset the SCORE_FROM_PREV_ROUND
		SCORE_FROM_PREV_ROUND = 0;
	}







	static public void EVENT( eScoreEvent evt) {                  // d
		try { // try-catch stops an error from breaking your program 
			S.Event(evt);
		}  catch (System .NullReferenceException nre) {
			Debug.LogError ("ScoreManager:EVENT() called while S=null.\n"+nre );
		}
	}
	public void Event(eScoreEvent evt) {
		switch (evt) {
		// Same things need to happen whether it's a draw, a win, or a loss
		case eScoreEvent.draw:     // Drawing a card
		case eScoreEvent.gameWin:  // Won the round
		case eScoreEvent.gameLoss: // Lost the round
			S.chain = 0;             // resets the score chain
			S.score += S.scoreRun;     // add scoreRun to total score
			S.scoreRun = 0;          // reset scoreRun
			break;

		case eScoreEvent.mine:    // Remove a mine card
			S.chain++;              // increase the score chain
			S.scoreRun += S.chain;    // add score for this card to run
			break;
		}

		// This second switch statement handles round wins and losses
		switch (evt) {
		case eScoreEvent.gameWin:
			// If it's a win, add the score to the next round
			// static fields are NOT reset by SceneManager.LoadScene()
			SCORE_FROM_PREV_ROUND = S.score;
			print ("You won this round! Round score: "+S.score);
			break;

		case eScoreEvent.gameLoss:
			// If it's a loss, check against the high score
			if (HIGH_SCORE <= S.score) {
				print("You got the high score! High score: "+S.score);
				HIGH_SCORE = S.score;
				PlayerPrefs.SetInt("ProspectorHighScore", S.score);
			} else {
				print ("Your final score for the game was: "+S.score);
			}
			break;

		default:
			print ("score: " + S.score + " scoreRun:" + S.scoreRun + " chain:" + S.chain);//" card:"+S.gameObject);
			break;
		}
	}


	public void ShareOnTwitter(){
		string toShare = "Check out my score: "+S.score+"!";
		Application.OpenURL(twitter + "?text="+ WWW.EscapeURL(toShare +"\n" + name));

	}

	static public int CHAIN { get { return S.chain; } }             // e
	static public int SCORE { get { return S.score; } }
	static public int SCORE_RUN { get { return S.scoreRun; } }
} 