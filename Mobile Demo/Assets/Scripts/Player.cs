using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public bool isHuman = false;
	public string displayName = "Player";
	public Color teamColor, selectedColor;
	public Soldier soldier;

	private SpawnPoint spawnPoint;
	private bool started = false;
	private Soldier selectedSoldier;
	private int teamKils = 0, teamDeaths = 0;

	// Use this for initialization
	void Start () {
		spawnPoint = GetComponentInChildren<SpawnPoint>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Begin() {

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

	public bool IsDefeated() {
		if(!started) return false;
		Soldier[] soldiers = GetComponentsInChildren<Soldier>();
		return soldiers==null || soldiers.Length == 0;
	}

	public void HandleClick(GameObject hitObject, Vector3 hitPoint) {
		if(hitObject && hitObject.name != "Ground" && hitObject.name != "LOS") { //clicked on something that was not the ground plane
			Soldier soldier = hitObject.transform.parent.GetComponent<Soldier>();
			if(soldier) { //clicked on cube
				if(selectedSoldier) { //already have cube selected
					if(soldier != selectedSoldier) {
						if(soldier.IsControlledBy(this)) {
							selectedSoldier.Deselect();
							soldier.Select();
							selectedSoldier = soldier;
						} else {
							selectedSoldier.Attack(soldier);
						}
					}
				} else { // no cube selected
					if(soldier.IsControlledBy(this)) {
						soldier.Select();
						selectedSoldier = soldier;
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
		teamKils++;
	}

	public void AddDeath() {
		teamDeaths++;
	}

	public Soldier GetSelectedSoldier() {
		return selectedSoldier;
	}
}
