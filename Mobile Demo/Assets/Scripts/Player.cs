using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public bool isHuman = false;
	public string displayName = "Player";
	public Color teamColor, selectedColor;
	public Soldier soldier;

	private SpawnPoint spawnPoint;
	private bool started = false;
	private Soldier selectedSoldier, selectedEnemySoldier;
	private int teamKills = 0, teamDeaths = 0, wins = 0;

	// Use this for initialization
	void Start () {
		spawnPoint = GetComponentInChildren<SpawnPoint>();
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
				if(clickedSoldier.IsControlledBy(this)) {
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
					if(deselectSoldier) {
						selectedSoldier.Deselect();
						selectedSoldier = null;
					}
					if(selectSoldier) {
						clickedSoldier.Select();
						selectedSoldier = clickedSoldier;
					}
				} else {
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
						if(selectedSoldier.Attack(clickedSoldier)) {
							selectEnemy = true;
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
				float distance = Vector3.Distance(hitPoint, selectedSoldier.transform.position);
				if(distance < selectedSoldier.getRange()) {
					selectedSoldier.SetDestination(new Vector3(hitPoint.x, 0, hitPoint.z));
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
}
