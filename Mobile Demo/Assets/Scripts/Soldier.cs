using UnityEngine;
using System.Collections;

public class Soldier : MonoBehaviour {

	public string displayName = "Soldier";
	public float moveSpeed = 2, rotateSpeed = 75;
	public float weaponDamage = 0.35f;

	private Player owner;
	private WeaponBeam[] beams;
	private LOS los;
	private bool started = false, weaponBeamsOn = false;
	private bool selected = false, moving = false, rotating = false;
	private Vector3 destination = Resources.InvalidPosition;
	private Quaternion targetRotation;
	private Soldier target;
	private float healthPoints = 100, maxHealthPoints = 100;
	private int numKills = 0;
	private float range = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(!started) return;
		if(!target && weaponBeamsOn) TurnOffWeaponBeams();
		if(target) {
			if(selected) target.Select();
			else target.Deselect();
		}
		if(rotating) {
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
			Quaternion inverseTargetRotation = new Quaternion(-targetRotation.x,-targetRotation.y,-targetRotation.z,-targetRotation.w);
			if(transform.rotation == targetRotation || transform.rotation == inverseTargetRotation) {
				rotating = false;
				moving = true;
			}
		} else if(moving) {
			transform.position = Vector3.MoveTowards(transform.position, destination, moveSpeed * Time.deltaTime);
			if(transform.position == destination) {
				destination = Resources.InvalidPosition;
				moving = false;
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
	
	private bool TargetTooFarAway() {
		if(!target) return false;
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
		TurnOnWeaponBeams();
		if(target.Damage(weaponDamage)) {
			numKills += 1;
			if(owner) owner.AddKill();
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
		this.destination = destination;
		targetRotation = Quaternion.LookRotation(destination - transform.position);
		rotating = true;
		moving = false;
		TurnOffWeaponBeams();
		if(target) {
			target.Deselect();
			target = null;
		}
	}

	public void Attack(Soldier target) {
		this.target = target;
		if(TargetTooFarAway()) destination = GetAttackPosition();
		else destination = transform.position;
		targetRotation = Quaternion.LookRotation(target.transform.position - transform.position);
		rotating = true;
		moving = false;
	}

	public void Begin() {

		owner = transform.root.GetComponent<Player>();
		if(owner) SetColor(owner.teamColor);
		beams = GetComponentsInChildren<WeaponBeam>();
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

	public Soldier GetTarget() {
		return target;
	}

	public string GetDisplayName() {
		return displayName + " (Kills: " + numKills + ")";
	}
	
	public float GetHealthPercentage() {
		return healthPoints / maxHealthPoints;
	}

	public float getRange() {
		return range;
	}

}
