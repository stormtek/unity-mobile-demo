using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public GUISkin skin;

	private Player[] players;
	private Player activePlayer;
	private bool started = false, finished = false, showMenu = true, invalid = false;
	private string winningMessage = "";
	private float padding = 20;
	
	void Start () {
		players = FindObjectsOfType(typeof(Player)) as Player[];
		//ideally this would be from a player selection menu
		//but that is beyond what is needed for this demo
		foreach(Player player in players) {
			if(player.isHuman) activePlayer = player;
		}
	}

	void Update () {
		if(!activePlayer) {
			invalid = true;
			finished = true;
			winningMessage = "Unable to play without a human player selected";
		}
		List<Player> undefeatedPlayers = new List<Player>();
		foreach(Player player in players) {
			if(!player.IsDefeated()) {
				undefeatedPlayers.Add(player);
			}
		}
		if(undefeatedPlayers.Count == 1) {
			finished = true;
			showMenu = true;
			winningMessage = undefeatedPlayers[0].displayName + " Wins!";
			foreach(Player player in players) player.Finish();
		}
	}

	void OnGUI() {
		GUI.skin = skin;
		GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
		if(showMenu) {
			float menuWidth = 300;
			float menuHeight = 200;
			float menuLeft = Screen.width / 2 - menuWidth / 2;
			float menuTop = Screen.height / 2 - menuHeight / 2;

			float buttonWidth = menuWidth - 2 * padding;
			float buttonHeight = 70;
			float buttonLeft = menuLeft + padding;
			float buttonTop = menuTop + padding;
			GUI.Box(new Rect(menuLeft, menuTop, menuWidth, menuHeight), "");
			if(!invalid) {
				string startButtonText = "Start";
				if(started && !finished) startButtonText = "Resume";
				else if(finished) startButtonText = "New Game";
				if(GUI.Button(new Rect(buttonLeft, buttonTop, buttonWidth, buttonHeight), startButtonText)) {
					Time.timeScale = 1.0f;
					started = true;
					showMenu = false;
					foreach(Player player in players) {
						player.Begin();
					}
				}
			}
			buttonTop += buttonHeight + padding;
			if(GUI.Button(new Rect(buttonLeft, buttonTop, buttonWidth, buttonHeight), "Exit")) {
				Application.Quit();
			}
			if(finished) {
				float labelHeight = 40;
				float labelWidth = Screen.width - 4 * padding;
				float boxHeight = labelHeight + padding;
				float boxWidth = labelWidth + 2 * padding;
				float boxTop = menuTop - boxHeight - padding;
				float boxLeft = padding;
				GUI.Box(new Rect(boxLeft, boxTop, boxWidth, boxHeight), "");
				GUI.Label(new Rect(boxLeft + padding, boxTop + padding / 2, labelWidth, labelHeight), winningMessage);
			}
		} else {
			float buttonWidth = 250;
			float buttonHeight = 70;
			float buttonLeft = Screen.width - padding - buttonWidth;
			float buttonTop = Screen.height - padding - buttonHeight;
			if(GUI.Button(new Rect(buttonLeft, buttonTop, buttonWidth, buttonHeight), "Menu")) {
				Time.timeScale = 0.0f;
				showMenu = true;
				foreach(Player player in players) {
					player.Pause();
				}
			}
		}
		GUI.EndGroup();

	}

	public Player GetActivePlayer() {
		return activePlayer;
	}

	public bool Paused() {
		return showMenu;
	}
}
