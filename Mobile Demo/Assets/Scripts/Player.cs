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
	private Soldier selectedSoldier, selectedEnemySoldier;
	private int teamKills = 0, teamDeaths = 0, wins = 0;
	private SoundManager soundManager;
	private State currentState = State.None;

	// Use this for initialization
	void Start () {
		spawnPoint = GetComponentInChildren<SpawnPoint>();
		soundManager = FindObjectOfType(typeof(SoundManager)) as SoundManager;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Begin() {
		Soldier[] existingSoldiers = GetComponentsInChildren<Soldier>();
		foreach(Soldier oldSoldier in existingSoldiers) Destroy(oldSoldier);
		SpawnPosition[] points = spawnPoint.GetComponentsInChildren<SpawnPosition>();
		foreach(SpawnPosition position in points) {
			Soldier newSoldier = (Soldier)Instantiate(soldier, position.transform.position, position.transform.rotation);
			newSoldier.transform.parent = this.transform;
			newSoldier.Begin();
		}
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
							if(currentState == State.Defend) {
								Debug.Log("tell selected soldier to enter defense mode for a turn");
							} else {
								if(selectedSoldier.IsSelected()) {
									deselectSoldier = true;
								} else {
									selectSoldier = true;
								}
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
						if(currentState == State.Defend) {
							Debug.Log("Tell newly selected soldier to enter defense mode for a turn");
						}
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
					if(distance < selectedSoldier.getRange()) {
						selectedSoldier.SetDestination(new Vector3(hitPoint.x, 0, hitPoint.z));
					}
				} else {
					// prompt user to click on move first somehow?
				}
			}
		}
	}

	public void AddKill() {
		teamKills++;
	}

	public void AddDeath() {
		teamDeaths++;
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
		if(currentState != State.Defend) currentState = State.None;
	}

	public void PlaySound(string soundName) {
		if(soundManager) soundManager.PlaySound(soundName);
	}

	public void StopSound(string soundName) {
		if(soundManager) soundManager.StopSound(soundName);
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
}
