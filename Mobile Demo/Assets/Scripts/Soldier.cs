using UnityEngine;
using System.Collections;

public class Soldier : MonoBehaviour {

	private Player owner;
	private WeaponBeam[] beams;
	private bool started = false, weaponBeamsOn = false;
	private bool selected = false;

	// Use this for initialization
	void Start () {
		owner = transform.root.GetComponent<Player>();
		if(owner) SetColor(owner.teamColor);
		beams = GetComponentsInChildren<WeaponBeam>();
		Hide();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	private void SetColor(Color color) {
		Element[] childObjects = transform.GetComponentsInChildren<Element>();
		foreach(Element childObject in childObjects) childObject.renderer.material.color = color;
	}

	private void Show() {
		foreach(Renderer renderer in transform.GetComponentsInChildren<Renderer>()) {
			renderer.enabled = true;
		}
	}
	
	private void Hide() {
		foreach(Renderer renderer in transform.GetComponentsInChildren<Renderer>()) {
			renderer.enabled = false;
		}
	}

	private void TurnOnWeaponBeams() {
		weaponBeamsOn = true;
		foreach(WeaponBeam beam in beams) {
			beam.transform.renderer.enabled = true;
		}
	}
	
	private void TurnOffWeaponBeams() {
		weaponBeamsOn = false;
		foreach(WeaponBeam beam in beams) {
			beam.transform.renderer.enabled = false;
		}
	}

	public bool IsControlledBy(Player player) {
		return owner == player;
	}

	public void Select() {
		selected = true;
		SetColor(owner.selectedColor);
	}
	
	public void Deselect() {
		selected = false;
		if(owner) SetColor(owner.teamColor);
	}

	public bool IsSelected() {
		return selected;
	}

	public void SetDestination(Vector3 destination) {

	}

	public void Attack(Soldier target) {

	}

	public void Begin() {
		started = true;
		Show();
		TurnOffWeaponBeams();
	}

	public void Pause() {
		started = false;
	}

	public void Finish() {
		started = false;
		Hide();
	}
}
