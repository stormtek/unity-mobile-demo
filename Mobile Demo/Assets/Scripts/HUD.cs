using UnityEngine;
using System.Collections;

public class HUD : MonoBehaviour {

	public GUISkin skin;
	public Texture2D healthyTexture, damagedTexture, criticalTexture;
	public Texture2D upArrow, downArrow, leftArrow, rightArrow;
	public Texture2D move, moveActive, moveUnavailable;
	public Texture2D attack, attackActive, attackUnavailable;
	public Texture2D defend, defendActive, defendUnavailable;
	public Texture2D cancel;

	private SoundManager soundManager;
	private Player humanPlayer;
	private bool started = false;
	private GUIStyle selectedStyle = new GUIStyle();
	private GUIStyle targetStyle = new GUIStyle();
	private GUIStyle movementStyle = new GUIStyle();
	private GUIStyle buttonStyle = new GUIStyle();

	private bool moveButtonActive = false, attackButtonActive = false, defendButtonActive = false;
	
	void Start () {
		soundManager = FindObjectOfType(typeof(SoundManager)) as SoundManager;
		selectedStyle.alignment = TextAnchor.MiddleLeft;
		selectedStyle.fontSize = 30;
		selectedStyle.normal.textColor = Color.white;
		targetStyle.alignment = TextAnchor.MiddleRight;
		targetStyle.fontSize = 30;
		targetStyle.normal.textColor = Color.white;
	}

	void OnGUI() {
		if(!humanPlayer || !started) return;
		GUI.skin = skin;
		GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height));
		Soldier currentSelection = humanPlayer.GetSelectedSoldier();
		Soldier currentTarget = humanPlayer.GetSelectedEnemy();
		// display name for the selected soldier belonging to the human player
		if(currentSelection) {
			selectedStyle.normal.background = null;
			selectedStyle.padding.top = 0;
			GUI.Label(new Rect(10, 30, Screen.width / 2 - 10, 40), currentSelection.GetDisplayName(), selectedStyle);
		}
		// display name for the selected soldier belonging to another player (enemy)
		if(currentTarget) {
			targetStyle.normal.background = null;
			targetStyle.padding.top = 0;
			GUI.Label(new Rect(Screen.width / 2, 30, Screen.width / 2 - 10, 40), currentTarget.GetDisplayName(), targetStyle);
		}
		float healthPercentage = 0.0f;
		// display health bar for the selected soldier belonging to the human player
		if(currentSelection) {
			healthPercentage = currentSelection.GetHealthPercentage();
			if(healthPercentage > Resources.highSplit) selectedStyle.normal.background = healthyTexture;
			else if(healthPercentage > Resources.lowSplit) selectedStyle.normal.background = damagedTexture;
			else selectedStyle.normal.background = criticalTexture;
			selectedStyle.padding.top = -20;
			GUI.Label(new Rect(10, 10, 300 * healthPercentage, 20), "", selectedStyle);
		}
		// display health bar for the selected soldier belonging to another player (enemy)
		if(currentTarget) {
			healthPercentage = currentTarget.GetHealthPercentage();
			if(healthPercentage > Resources.highSplit) targetStyle.normal.background = healthyTexture;
			else if(healthPercentage > Resources.lowSplit) targetStyle.normal.background = damagedTexture;
			else targetStyle.normal.background = criticalTexture;
			targetStyle.padding.top = -20;
			float targetHealthWidth = 300 * healthPercentage;
			GUI.Label(new Rect(Screen.width - targetHealthWidth - 10, 10, targetHealthWidth, 20), "", targetStyle);
		}
		// display options buttons for the selected soldier
		if(currentSelection && humanPlayer && !humanPlayer.NoMovesLeft() && !humanPlayer.MakingMove()) {
			if(humanPlayer.GetState() == Player.State.None) {
				moveButtonActive = false;
				defendButtonActive = false;
				attackButtonActive = false;
			}
			int padding = 20;
			int buttonWidth = 50;
			int topPos = 80;
			int leftPos = padding;
			// move
			if(currentSelection.CanMove()) {
				buttonStyle.normal.background = moveButtonActive ? moveActive : move;
			} else {
				buttonStyle.normal.background = moveUnavailable;
			}
			if(GUI.Button(new Rect(leftPos, topPos, buttonWidth, buttonWidth), "", buttonStyle)) {
				if(currentSelection.CanMove()) {
					if(moveButtonActive) {
						if(soundManager) soundManager.PlaySound("CancelClick");
						moveButtonActive = false;
						humanPlayer.SetState(Player.State.None);
					} else {
						if(soundManager) soundManager.PlaySound("ActionClick");
						moveButtonActive = true;
						if(attackButtonActive) attackButtonActive = false;
						if(defendButtonActive) defendButtonActive = false;
						humanPlayer.SetState(Player.State.Move);
					}
				}
			}
			topPos += padding + buttonWidth;
			// attack
			if(currentSelection.CanAttack()) {
				buttonStyle.normal.background = attackButtonActive ? attackActive : attack;
			} else {
				buttonStyle.normal.background = attackUnavailable;
			}
			if(GUI.Button(new Rect(leftPos, topPos, buttonWidth, buttonWidth), "", buttonStyle)) {
				if(currentSelection.CanAttack()) {
					if(attackButtonActive) {
						if(soundManager) soundManager.PlaySound("CancelClick");
						attackButtonActive = false;
						humanPlayer.SetState(Player.State.None);
					} else {
						if(soundManager) soundManager.PlaySound("ActionClick");
						attackButtonActive = true;
						if(moveButtonActive) moveButtonActive = false;
						if(defendButtonActive) defendButtonActive = false;
						humanPlayer.SetState(Player.State.Attack);
					}
				}
			}
			topPos += padding + buttonWidth;
			// defend
			if(currentSelection.CanDefend()) {
				buttonStyle.normal.background = defendButtonActive ? defendActive : defend;
			} else {
				buttonStyle.normal.background = defendUnavailable;
			}
			if(GUI.Button(new Rect(leftPos, topPos, buttonWidth, buttonWidth), "", buttonStyle)) {
				if(currentSelection.CanDefend()) {
					if(soundManager) soundManager.PlaySound("ActionClick");
					currentSelection.Defend();
					if(moveButtonActive) moveButtonActive = false;
					if(attackButtonActive) attackButtonActive = false;
					humanPlayer.StartMove(currentSelection);
				}
			}
			topPos += padding + buttonWidth;
			// cancel
			buttonStyle.normal.background = cancel;
			if(GUI.Button(new Rect(leftPos, topPos, buttonWidth, buttonWidth), "", buttonStyle)) {
				if(soundManager) soundManager.PlaySound("CancelClick");
				humanPlayer.DeselectSoldier();
				humanPlayer.SetState(Player.State.None);

			}
		} else {
			moveButtonActive = false;
			attackButtonActive = false;
			defendButtonActive = false;
			if(humanPlayer) humanPlayer.SetState(Player.State.None);
		}
		// display the movement control
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

	public void SetHumanPlayer(Player player) {
		humanPlayer = player;
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
