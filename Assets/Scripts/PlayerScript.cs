using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Mirror;

public class PlayerScript : NetworkBehaviour {
	
	[SyncVar]
	public bool isTurn = false;
	
	[SyncVar]
	public int charNumber = -1;
		
	[SyncVar]
	public PlayerStat stats = new PlayerStat();
	
	public List<Sprite> sprites = new List<Sprite>();
	public List<PlayerStat> statList;
		
	private bool paused = false;//set to true when wrong input is given, pauses game for half a second before letting user submit input again
	
	private GameManager gm;
	private HealthBarScript hp;
	private GameObject[] players;
	private InputManagerScript inputManager;
		
	public override void OnStartServer() {
		base.OnStartServer();//calls the base class that has been overwritten by this
		//find the gamemanager object - going to be using this to manage turns
		gm = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<GameManager>();
		
		//moves the player so that the player models don't overlap
		transform.Translate(Vector3.right * GameObject.FindGameObjectsWithTag("Player").Length);
	}
		
	public override void OnStartClient() {
		base.OnStartClient();//calls the base class that has been overwritten by this
		
		//when player 2+ joins, the player 1 object he loads might already have a character selected - this loads that character on that object
		if (charNumber != -1) {
			SetChar(charNumber);
		}
		
		//healthbar above the character sprite
		hp = gameObject.GetComponentInChildren<HealthBarScript>();
		
		//anything which should only be triggered by the player who owns this player object will go under this
		if (!base.hasAuthority) {
			return;
		}
		
		inputManager = gameObject.GetComponent<InputManagerScript>();	
	}
	
	//sets the character sprite and stats
	[Command]
	public void CmdSetChar(int index) {
		SetChar(index);
		charNumber = index;
		RpcSetChar(index);
	}
	
	public void SetChar(int index) {
		gameObject.GetComponent<SpriteRenderer>().sprite = sprites[index];
		stats = statList[index-1];
	}
	
	[ClientRpc]
	void RpcSetChar(int index) {
		SetChar(index);
	}
	
	//
	//UPDATE FUNCTIONS
	//
	

	void FixedUpdate() {
		if (!isTurn || !base.hasAuthority || paused == true) {
			return;
		}

		Move();
	}
	
	void Move() {
		//gets movement in x and z axis by wasd
		Vector3 movement = inputManager.GetMovementInput();
		if (movement != Vector3.zero) {
			
			RaycastHit hitInfo;
			if (Physics.Raycast(transform.position, movement, out hitInfo, 1.1f)) {//check if player is going to move into wall or another player - if not, move him
				if (hitInfo.collider.tag == "Wall") {//if player moves into a wall, don't accept input
					paused = true;
					StartCoroutine(Unpause());
					return;
				}
			}
			else {
				transform.Translate(movement);
			}	
			
			//if within attack range after moving, attack another player
			if (Physics.Raycast(transform.position, movement, out hitInfo, stats.attackRange)) {
				if (hitInfo.collider.tag == "Player") {
					CmdCombat(hitInfo.collider.gameObject);
					isTurn = false;
					paused = true;
					StartCoroutine(Unpause());
					return;
				}
			}
			
			isTurn = false;
			paused = true;
			CmdSetNextPlayerTurn();
			StartCoroutine(Unpause());	
		}
	}
	
	[Command]
	void CmdSetNextPlayerTurn() {
		Debug.Log("Setting next player");
		gm.ChangePlayer();
	}
	
	[Command]
	void CmdCombat(GameObject obj) {
		PlayerScript player = obj.GetComponent<PlayerScript>();
		player.stats.health -= stats.damage;//deals damage to the other player
		float percent = (float)player.stats.health/player.stats.maxHealth;
		player.RpcUpdateHealthBar(percent);//updates the hp bar on all clients
		if (player.stats.health <=0) {
			player.RpcDeath();//sets the tombstone sprite for him, disables his collider
		}
		gm.ChangePlayer();
	}
	
	[TargetRpc]
	void TargetCheckDeath(NetworkConnection conn, int damage) {
		if (stats.health <= 0) {
			RpcDeath();
		}
		CmdSetNextPlayerTurn();
	}
	
	[ClientRpc]
	void RpcDeath() {
		gameObject.GetComponent<SpriteRenderer>().sprite = sprites[0];//have to run setchar?
		gameObject.GetComponent<BoxCollider>().enabled = false;
	}
	
	[ClientRpc]
	void RpcUpdateHealthBar(float percent) {
		hp.UpdateHealthBar(percent);
	}
	
	//doesnt let user spam invalid commands
	IEnumerator Unpause() {
		print("started pause");
		yield return new WaitForSeconds(0.1f);
		paused = false;
	}
		
	//
	//END OF GAME
	//
	
	[TargetRpc]
	public void TargetEndGame(bool win) {
		if (win) {
			SceneManager.LoadScene("WinScene");
		}
		else {
			SceneManager.LoadScene("LoseScene");
		}
	}
}