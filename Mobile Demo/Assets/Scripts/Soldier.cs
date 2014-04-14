using UnityEngine;
using System.Collections;

public class Soldier : MonoBehaviour {

	public string displayName = "Soldier";
	public float moveSpeed = 2, rotateSpeed = 75;
	public float weaponDamage = 0.35f;

	private Player owner;
	private WeaponBeam[] beams;
	private Shield shield;
	private LOS los;
	private bool started = false, weaponBeamsOn = false;
	private bool selected = false, moving = false, rotating = false;
	private Vector3 destination = Resources.InvalidPosition;
	private Quaternion targetRotation;
	private Soldier target;
	private float healthPoints = 100, maxHealthPoints = 100;
	private int numKills = 0;
	private float range = 0.0f;
	private bool currentlyActive = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(!started) return;
		if(!target && weaponBeamsOn) TurnOffWeaponBeams();
		/*if(target) {
			if(selected) target.Select();
			else target.Deselect();
		}*/
		if(rotating) {
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
			Quaternion inverseTargetRotation = new Quaternion(-targetRotation.x,-targetRotation.y,-targetRotation.z,-targetRotation.w);
			if(transform.rotation == targetRotation || transform.rotation == inverseTargetRotation) {
				rotating = false;
				moving = true;
				if(owner) owner.PlaySound("Footsteps");
			}
		} else if(moving) {
			transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
			if(transform.position == destination) {
				destination = Resources.InvalidPosition;
				moving = false;
				if(owner) owner.StopSound("Footsteps");
				currentlyActive = false;
			}
		} else if(target) {
			MakeAttack();
		}
	}

	private void SetColor(Color color) {
		Element[] childObjects = transform.GetComponentsInChildren<Element>();
		foreach(Element childObject in childObjects) childObject.renderer.material.color = color;
	}

	private void Show() {
		foreach(Renderer renderer in transform.GetComponentsInChildren<Renderer>()) {
			if(renderer.tag != "LOS") {
				renderer.enabled = true;
			}
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

	private void ShowShield() {
		if(shield) shield.transform.renderer.enabled = true;
	}

	private void HideShield() {
		if(shield) shield.transform.renderer.enabled = false;
	}
	
	private bool TargetTooFarAway(Soldier target) {
		if(!target) return true;
		return Mathf.Abs((target.transform.position - transform.position).magnitude) > range;
	}
	
	private Vector3 GetAttackPosition() {
		Vector3 targetLocation = target.transform.position;
		Vector3 direction = targetLocation - transform.position;
		float targetDistance = direction.magnitude;
		float distanceToTravel = targetDistance - (0.9f * range);
		return Vector3.Lerp(transform.position, targetLocation, distanceToTravel / targetDistance);
	}
	
	private void MakeAttack() {
		if(owner) owner.PlaySound("Attack");
		TurnOnWeaponBeams();
		if(target.Damage(weaponDamage)) {
			numKills += 1;
			if(owner) {
				owner.AddKill();
				owner.StopSound("Attack");
				currentlyActive = false;
			}
		}
	}

	public bool IsControlledBy(Player player) {
		return owner == player;
	}

	public void Select() {
		selected = true;
		SetColor(owner.selectedColor);
		if(los) los.renderer.enabled = true;
	}
	
	public void Deselect() {
		selected = false;
		if(owner) SetColor(owner.teamColor);
		if(los) los.renderer.enabled = false;
	}

	public bool IsSelected() {
		return selected;
	}

	public void SetDestination(Vector3 destination) {
		currentlyActive = true;
		this.destination = destination;
		targetRotation = Quaternion.LookRotation(destination - transform.position);
		rotating = true;
		moving = false;
		if(owner) owner.StopSound("Footsteps");
		TurnOffWeaponBeams();
		if(target) {
			target = null;
			if(owner) owner.StopSound("Attack");
		}
	}

	public bool Attack(Soldier target) {
		if(TargetTooFarAway(target)) return false;
		currentlyActive = true;
		this.target = target;
		destination = transform.position;
		targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
		rotating = true;
		moving = false;
		owner.StopSound("Footsteps");
		return true;
	}

	public void Begin() {
		owner = transform.root.GetComponent<Player>();
		if(owner) SetColor(owner.teamColor);
		beams = GetComponentsInChildren<WeaponBeam>();
		shield = GetComponentInChildren<Shield>();
		los = GetComponentInChildren<LOS>();
		if(los) {
			//we are assuming the LOS object is a plane which has a base size 10x larger than a cube
			//we are also assuming that this is square rather than rectangular
			float losScale = los.transform.localScale.x;
			range = 10.0f / 2 * losScale;
		}
		started = true;
	}

	public void Pause() {
		started = false;
	}

	public void Resume() {
		started = true;
	}

	public void Finish() {
		started = false;
		Hide();
	}

	public bool Damage(float damage) {
		healthPoints -= damage;
		if(healthPoints < 0) {
			if(owner) owner.AddDeath();
			Destroy(gameObject);
			return true;
		}
		return false;
	}

	public void Defend() {
		ShowShield();
	}

	public Soldier GetTarget() {
		return target;
	}

	public string GetDisplayName() {
		return displayName + " (Kills: " + numKills + ")";
	}
	
	public float GetHealthPercentage() {
		return healthPoints / maxHealthPoints;
	}

	public float GetRange() {
		return range;
	}

	public bool IsActive() {
		return currentlyActive;
	}
}
