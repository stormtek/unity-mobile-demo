using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {

	public GUISkin skin;
	public Texture2D healthyTexture, damagedTexture, criticalTexture;
	public Texture2D upArrow, downArrow, leftArrow, rightArrow;

	private GameManager gameManager;
	private Player activePlayer;
	private bool started = false;
	private GUIStyle selectedStyle = new GUIStyle();
	private GUIStyle targetStyle = new GUIStyle();
	private GUIStyle movementStyle = new GUIStyle();
	private GUIStyle buttonStyle = new GUIStyle();
	
	void Start () {
		gameManager = transform.root.GetComponent<GameManager>();
		selectedStyle.alignment = TextAnchor.MiddleLeft;
		selectedStyle.fontSize = 30;
		selectedStyle.normal.textColor = Color.white;
		targetStyle.alignment = TextAnchor.MiddleRight;
		targetStyle.fontSize = 30;
		targetStyle.normal.textColor = Color.white;
	}

	void OnGUI() {
		if(!activePlayer || !started) return;
		GUI.skin = skin;
		GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
		Soldier currentSelection = activePlayer.GetSelectedSoldier();
		Soldier currentTarget = activePlayer.GetSelectedEnemy();
		if(currentSelection) {
			selectedStyle.normal.background = null;
			selectedStyle.padding.top = 0;
			GUI.Label(new Rect(10, 30, Screen.width / 2 - 10, 40), currentSelection.GetDisplayName(), selectedStyle);
		}
			//Soldier currentTarget = currentSelection.GetTarget();
		if(currentTarget) {
			targetStyle.normal.background = null;
			targetStyle.padding.top = 0;
			GUI.Label(new Rect(Screen.width / 2, 30, Screen.width / 2 - 10, 40), currentTarget.GetDisplayName(), targetStyle);
		}
		float healthPercentage = 0.0f;
		if(currentSelection) {
			healthPercentage = currentSelection.GetHealthPercentage();
			if(healthPercentage > Resources.highSplit) selectedStyle.normal.background = healthyTexture;
			else if(healthPercentage > Resources.lowSplit) selectedStyle.normal.background = damagedTexture;
			else selectedStyle.normal.background = criticalTexture;
			selectedStyle.padding.top = -20;
			GUI.Label(new Rect(10, 10, 300 * healthPercentage, 20), "", selectedStyle);
		}
		if(currentTarget) {
			healthPercentage = currentTarget.GetHealthPercentage();
			if(healthPercentage > Resources.highSplit) targetStyle.normal.background = healthyTexture;
			else if(healthPercentage > Resources.lowSplit) targetStyle.normal.background = damagedTexture;
			else targetStyle.normal.background = criticalTexture;
			targetStyle.padding.top = -20;
			float targetHealthWidth = 300 * healthPercentage;
			GUI.Label(new Rect(Screen.width - targetHealthWidth - 10, 10, targetHealthWidth, 20), "", targetStyle);
		}
		if(currentSelection) {
			int padding = 20;
			int buttonWidth = 50;
			int topPos = 80;
			int leftPos = padding;
			buttonStyle.normal.background = criticalTexture;
			if(GUI.Button(new Rect(leftPos, topPos, buttonWidth, buttonWidth), "M", buttonStyle)) {

			}
			topPos += padding + buttonWidth;
			buttonStyle.normal.background = healthyTexture;
			if(GUI.Button(new Rect(leftPos, topPos, buttonWidth, buttonWidth), "A", buttonStyle)) {

			}
			topPos += padding + buttonWidth;
			buttonStyle.normal.background = damagedTexture;
			if(GUI.Button(new Rect(leftPos, topPos, buttonWidth, buttonWidth), "D", buttonStyle)) {

			}
			topPos += padding + buttonWidth;
			buttonStyle.normal.background = healthyTexture;
			if(GUI.Button(new Rect(leftPos, topPos, buttonWidth, buttonWidth), "C", buttonStyle)) {
				
			}
		}
		movementStyle.normal.background = upArrow;
		GUI.Box(Resources.topZone, "", movementStyle);
		movementStyle.normal.background = downArrow;
		GUI.Box(Resources.bottomZone, "", movementStyle);
		movementStyle.normal.background = leftArrow;
		GUI.Box(Resources.leftZone, "", movementStyle);
		movementStyle.normal.background = rightArrow;
		GUI.Box(Resources.rightZone, "", movementStyle);
		GUI.EndGroup();
	}

	public void SetActivePlayer(Player player) {
		activePlayer = player;
	}

	public void Begin() {
		started = true;
	}

	public void Pause() {
		started = false;
	}

	public void Finish() {
		started = false;
	}
}
