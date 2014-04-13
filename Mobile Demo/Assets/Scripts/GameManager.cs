using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public GUISkin skin;

	private Player[] players;
	private Player humanPlayer;
	private HUD hud;
	private UserInput userInput;
	private SoundManager soundManager;
	private TurnManager turnManager;
	private bool started = false, finished = false, showMenu = true;
	private string winningMessage = "";
	private float padding = 20;
	
	void Start () {
		players = FindObjectsOfType(typeof(Player)) as Player[];
		//ideally this would be from a player selection menu
		//but that is beyond what is needed for this demo
		foreach(Player player in players) {
			if(player.isHuman) humanPlayer = player;
		}
		hud = transform.root.GetComponent<HUD>();
		if(hud) hud.SetHumanPlayer(humanPlayer);
		userInput = transform.root.GetComponent<UserInput>();
		if(userInput) userInput.enabled = false;
		soundManager = FindObjectOfType(typeof(SoundManager)) as SoundManager;
		turnManager = transform.root.GetComponent<TurnManager>();
		if(turnManager) turnManager.SetPlayers(players);
	}

	void Update () {
		if(!humanPlayer) {
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
			started = false;
			finished = true;
			showMenu = true;
			undefeatedPlayers[0].AddWin();
			winningMessage = undefeatedPlayers[0].displayName + " Wins! ..... ";
			foreach(Player player in players) {
				winningMessage += player.displayName + ": " + player.GetNumberOfWins() + " ";
				player.Finish();
			}
			if(hud) hud.Finish();
			if(turnManager) turnManager.Finish();
		}
	}

	void OnGUI() {
		GUI.skin = skin;
		GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
		if(showMenu) DrawMenu();
	 	else DrawMenuButton();
		GUI.EndGroup();

	}

	private void DrawMenu() {
		float menuWidth = 300;
		float menuHeight = 200;
		float menuLeft = Screen.width / 2 - menuWidth / 2;
		float menuTop = Screen.height / 2 - menuHeight / 2;
		
		float buttonWidth = menuWidth - 2 * padding;
		float buttonHeight = 70;
		float buttonLeft = menuLeft + padding;
		float buttonTop = menuTop + padding;
		GUI.Box(new Rect(menuLeft, menuTop, menuWidth, menuHeight), "");
		string startButtonText = "New Game";
		if(started) startButtonText = "Resume";
		if(GUI.Button(new Rect(buttonLeft, buttonTop, buttonWidth, buttonHeight), startButtonText)) {
			if(soundManager) soundManager.PlaySound("ButtonClick");
			Time.timeScale = 1.0f;
			if(!started) {
				started = true;
				foreach(Player player in players) {
					player.Begin();
				}
				if(turnManager) turnManager.Begin();
			} else {
				foreach(Player player in players) {
					player.Resume();
				}
				if(turnManager) turnManager.Resume();
			}
			if(finished) finished = false;
			showMenu = false;
			if(hud) hud.Begin();
			if(userInput) userInput.enabled = true;
		}
		buttonTop += buttonHeight + padding;
		if(GUI.Button(new Rect(buttonLeft, buttonTop, buttonWidth, buttonHeight), "Exit")) {
			if(soundManager) soundManager.PlaySound("ButtonClick");
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
	}

	private void DrawMenuButton() {
		float buttonWidth = 250;
		float buttonHeight = 70;
		float buttonLeft = Screen.width - padding - buttonWidth;
		float buttonTop = Screen.height - padding - buttonHeight;
		if(GUI.Button(new Rect(buttonLeft, buttonTop, buttonWidth, buttonHeight), "Menu")) {
			if(soundManager) soundManager.PlaySound("ButtonClick");
			Time.timeScale = 0.0f;
			showMenu = true;
			foreach(Player player in players) {
				player.Pause();
			}
			if(hud) hud.Pause();
			if(turnManager) turnManager.Pause();
			if(userInput) userInput.enabled = false;
		}
		float labelWidth = 500;
		buttonLeft = buttonLeft - labelWidth - padding;
		GUI.Label(new Rect(buttonLeft, buttonTop, labelWidth, buttonHeight), turnManager.GetActivePlayerName());
	}
	
	public Player GetActivePlayer() {
		return humanPlayer;
	}

	public bool Paused() {
		return showMenu;
	}
}
