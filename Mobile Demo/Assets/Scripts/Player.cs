using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public enum State {
		Attack, Move, Defend, None
	}

	public bool isHuman = false;
	public string displayName = "Player";
	public Color teamColor, selectedColor;
	public Soldier soldier;

	private SpawnPoint spawnPoint;
	private bool started = false;
	private Soldier selectedSoldier, selectedEnemySoldier, activeSoldier;
	private int teamKills = 0, teamDeaths = 0, wins = 0;
	private SoundManager soundManager;
	private State currentState = State.None;
	private int numMoves = 0;
	private bool makingMove = false;

	private AI aiController;

	// Use this for initialization
	void Start () {
		spawnPoint = GetComponentInChildren<SpawnPoint>();
		soundManager = FindObjectOfType(typeof(SoundManager)) as SoundManager;
	}
	
	// Update is called once per frame
	void Update () {
		if(!started) return;
		if(!isHuman && aiController != null && !NoMovesLeft() && !makingMove) {
			aiController.ChooseAndMakeMove();
		}
		if(makingMove) MakeMove();
	}

	public void Begin() {
		Soldier[] existingSoldiers = GetComponentsInChildren<Soldier>();
		foreach(Soldier oldSoldier in existingSoldiers) Destroy(oldSoldier);
		SpawnPosition[] points = spawnPoint.GetComponentsInChildren<SpawnPosition>();
		Soldier[] newSoldiers = new Soldier[points.Length];
		for(int i=0; i<points.Length; i++) {
			SpawnPosition position = points[i];
			Soldier newSoldier = (Soldier)Instantiate(soldier, position.transform.position, position.transform.rotation);
			newSoldier.transform.parent = this.transform;
			newSoldier.Begin();
			newSoldiers[i] = newSoldier;
		}
		if(!isHuman) aiController = new AI(this, newSoldiers);
		started = true;
	}

	public void Pause() {
		started = false;
		foreach(Soldier soldier in GetComponentsInChildren<Soldier>()) {
			soldier.Pause();
		}
	}

	public void Finish() {
		started = false;
		foreach(Soldier soldier in GetComponentsInChildren<Soldier>()) {
			soldier.Finish();
		}
	}

	public void Resume() {
		started = true;
		foreach (Soldier soldier in GetComponentsInChildren<Soldier>()) {
			soldier.Resume();
		}
	}

	public bool IsDefeated() {
		if(!started) return false;
		Soldier[] soldiers = GetComponentsInChildren<Soldier>();
		return soldiers==null || soldiers.Length == 0;
	}

	public void HandleClick(GameObject hitObject, Vector3 hitPoint) {
		if(hitObject && hitObject.name != "Ground" && hitObject.name != "LOS") { //clicked on something that was not the ground plane
			Soldier clickedSoldier = hitObject.transform.parent.GetComponent<Soldier>();
			if(clickedSoldier) { //clicked on soldier
				if(clickedSoldier.IsControlledBy(this)) { // soldier is controlled by player
					bool selectSoldier = false, deselectSoldier  = false;
					if(selectedSoldier) { //already have soldier selected
						if(clickedSoldier == selectedSoldier) { // clicked on selected soldier
							if(selectedSoldier.IsSelected()) {
								deselectSoldier = true;
							} else {
								selectSoldier = true;
							}
						} else { // clicked on another soldier
							deselectSoldier = true;
							selectSoldier = true;
						}
					} else { // no soldier selected
						selectSoldier = true;
					}
					if(deselectSoldier) DeselectSoldier();
					if(selectSoldier) {
						clickedSoldier.Select();
						selectedSoldier = clickedSoldier;
					}
				} else { // soldier is controlled by another player
					bool selectEnemy = false, deselectEnemy = false;;
					if(selectedEnemySoldier) { // already have enemy soldier selected
						if(clickedSoldier == selectedEnemySoldier) { // clicked on selected enemy soldier
							if(selectedEnemySoldier.IsSelected()) {
								deselectEnemy = true;
							} else { // 
								selectEnemy = true;
							}
						} else { // clicked on another enemy soldier
							deselectEnemy = true;
							selectEnemy = true;
						}
					} else { // clicked on an enemy soldier
						selectEnemy = true;
					}
					if(deselectEnemy) {
						selectedEnemySoldier.Deselect();
						selectedEnemySoldier = null;
					}
					if(selectedSoldier) {
						if(currentState == State.Attack) {
							if(selectedSoldier.Attack(clickedSoldier)) {
								selectEnemy = true;
								StartMove(selectedSoldier);
							}
						} else {
							//prompt user to click on attack first somehow?
						}
					}
					if(selectEnemy) {
						selectedEnemySoldier = clickedSoldier;
						selectedEnemySoldier.Select();

					}
				}

			}
		} else if(selectedSoldier) {
			if(hitPoint != Resources.InvalidPosition) {
				if(currentState == State.Move) {
					float distance = Vector3.Distance(hitPoint, selectedSoldier.transform.position);
					if(distance < selectedSoldier.GetRange()) {
						selectedSoldier.SetDestination(new Vector3(hitPoint.x, 0, hitPoint.z));
						StartMove(selectedSoldier);
					}
				} else {
					// prompt user to click on move first somehow?
				}
			}
		}
	}

	private void MakeMove() {
		// do nothing until the active soldier is finished, then end the move
		if(!activeSoldier.IsActive()) {
			numMoves--;
			makingMove = false;
			activeSoldier = null;
		}
	}

	public void StartMove(Soldier soldier) {
		if(!makingMove) {
			makingMove = true;
			currentState = State.None;
			activeSoldier = soldier;
		}
	}

	public bool MakingMove() {
		return makingMove;
	}

	public void AddKill() {
		teamKills++;
	}

	public void AddDeath() {
		teamDeaths++;
		if(!isHuman && aiController != null) {
			Soldier[] soldiers = GetComponentsInChildren<Soldier>();
			aiController.UpdateSoldiers(soldiers);
		}
	}

	public void AddWin() {
		wins++;
	}

	public Soldier GetSelectedSoldier() {
		return selectedSoldier;
	}

	public Soldier GetSelectedEnemy() {
		return selectedEnemySoldier;
	}

	public int GetNumberOfWins() {
		return wins;
	}

	public void DeselectSoldier() {
		selectedSoldier.Deselect();
		selectedSoldier = null;
		currentState = State.None;
	}

	public void PlaySound(string soundName) {
		if(soundManager) soundManager.PlaySound(soundName);
	}

	public void StopSound(string soundName) {
		if(soundManager) soundManager.StopSound(soundName);
	}

	public bool IsPlayingSound(string soundName) {
		if(soundManager) return soundManager.IsPlayingSound(soundName);
		return false;
	}

	public string GetDisplayName() {
		if(currentState == State.None) return displayName;
		else return displayName + " (" + currentState + ")";
	}

	public void SetState(State newState) {
		currentState = newState;
	}

	public State GetState() {
		return currentState;
	}

	public void StartTurn() {
		//this should be retrieved from TurnManager? GameManager? one of these anyway ...
		numMoves = 2;
		Soldier[] soldiers = GetComponentsInChildren<Soldier>();
		foreach(Soldier soldier in soldiers) soldier.StartTurn();
	}

	public void EndTurn() {
		currentState = State.None;
	}

	public bool NoMovesLeft() {
		return numMoves == 0 && !makingMove;
	}
}
