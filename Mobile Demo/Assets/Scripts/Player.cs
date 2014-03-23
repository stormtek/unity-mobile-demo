using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	public bool isHuman = false;
	public string displayName = "Player";
	public Color teamColor, selectedColor;

	private bool started = false;
	private Soldier selectedSoldier;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Begin() {
		started = true;
		foreach(Soldier soldier in GetComponentsInChildren<Soldier>()) {
			soldier.Begin();
		}
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
		return false;
	}

	public void HandleClick(GameObject hitObject, Vector3 hitPoint) {
		if(hitObject && hitObject.name != "Ground") { //clicked on something that was not the ground plane
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
				selectedSoldier.SetDestination(new Vector3(hitPoint.x, 0, hitPoint.z));
			}
		}
	}

	public void AddKill() {

	}

	public void AddDeath() {

	}
}
