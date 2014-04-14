using UnityEngine;
using System.Collections;

public class AI {

	private Player player;
	private Soldier[] soldiers;
	int currentSoldier = 0;

	public AI(Player player, Soldier[] soldiers) {
		this.player = player;
		this.soldiers = soldiers;
		if(soldiers.Length <= 0) currentSoldier = -1;
	}

	public void UpdateSoldiers(Soldier[] soldiers) {
		this.soldiers = soldiers;
		if(soldiers.Length <= 0) {
			currentSoldier = -1;
		} else if(currentSoldier < soldiers.Length) {
			// some soldiers have died, so reset the current soldier
			currentSoldier = 0;
		}
	}

	public void ChooseAndMakeMove() {
		if(currentSoldier < 0) return;
		if(currentSoldier >= soldiers.Length) currentSoldier = 0;
		// tell current soldier to move forward a bit, iterating through the soldiers
		Soldier soldier = soldiers[currentSoldier];
		if(soldier) {
			Vector3 location = soldier.transform.position;
			location += 4 * soldier.transform.forward;
			soldier.SetDestination(location);
			player.StartMove(soldier);
		}
		currentSoldier++;
		if(currentSoldier >= soldiers.Length) currentSoldier = 0;
		//if this takes a long time we still want to be able to continue processing (e.g. animations, user input)
	}
}
