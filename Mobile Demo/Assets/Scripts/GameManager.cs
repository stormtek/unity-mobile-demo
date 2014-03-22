using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

	private Player[] players;

	// Use this for initialization
	void Start () {
		players = FindObjectsOfType(typeof(Player)) as Player[];
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
