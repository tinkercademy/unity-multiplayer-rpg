using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerScriptFake : NetworkBehaviour
{
	
	
	/*
	public GameObject trash;
	
	//private GameManager gm;
	private bool isTurn = false;

	[SyncVar]
	public int turnNumber;
	
	public int initiative;
	private Button button;
	private GameObject[] players;
	
    // Start is called before the first frame update
    void Start()
    {
		//DebugConsole.Log("a");
		//Debug.Log("start");
    }
	
	public override void OnStartClient() {
		base.OnStartClient();//calls the base class that has been overwritten by this
		if (base.hasAuthority) {
			Debug.Log("adding button listener");
			button = GameObject.Find("Button").GetComponent<Button>();
			button.onClick.AddListener(CmdStartGame);
			CmdSetTurnNumber(GameObject.FindGameObjectsWithTag("Player").Length);
			GameObject.Find("turnorder").GetComponent<Text>().text = turnNumber.ToString();	
			Debug.Log("turn number: " + turnNumber.ToString());
		}
	}
	
	
	//how to fix the turn orders so all clients are synced
	//i guess enable turnorder as a syncvar, then make the clients send to server to adjust the turnorder when they join
	
	
	[Command]
	void CmdSetTurnNumber(int tempTurnNumber) {
		turnNumber = tempTurnNumber;
	}
	
	[Command]
	void CmdStartGame() {
		Instantiate(trash);
		GameObject[] tempPlayers = new GameObject[GameObject.FindGameObjectsWithTag("Player").Length];
		foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
			tempPlayers[player.GetComponent<PlayerScript>().turnNumber-1] = player;//when 2nd player joins after host, turn order of 2nd player is not set to 2
		}
		for (int i=0; i<tempPlayers.Length;i++) {
			TargetStartGame(tempPlayers[i].GetComponent<NetworkIdentity>().connectionToClient, tempPlayers);
		}
		print("start: " + tempPlayers[0].GetComponent<PlayerScript>().turnNumber.ToString());
		TargetChangeTurn(tempPlayers[0].GetComponent<NetworkIdentity>().connectionToClient);
	}
	
	[TargetRpc]
	void TargetStartGame(NetworkConnection conn, GameObject[] tempPlayers) {
		players = tempPlayers;
		Debug.Log("players set" + players.Length.ToString());
	}

    // Update is called once per frame
    void Update()
    {
		float xMovement = Input.GetAxisRaw("Horizontal");
		float zMovement = Input.GetAxisRaw("Vertical");
		if (isTurn && (xMovement != 0 || zMovement != 0) && base.hasAuthority) {
			isTurn = false;
			CmdMovePlayer(xMovement, zMovement);
			CmdChangeTurn(turnNumber);
		}
    }
	
	private NetworkIdentity GetNextPlayer(int numberToGet) {
		if (players.Length == numberToGet) {
			return players[0].GetComponent<NetworkIdentity>();
		}
		else {
			return players[numberToGet].GetComponent<NetworkIdentity>();
		}
	}
	
	[Command]
	private void CmdMovePlayer(float xMovement, float zMovement) {
		transform.Translate(xMovement, 0, zMovement);
	}
	
	[Command]
	private void CmdChangeTurn(int no) {
		NetworkIdentity target = GetNextPlayer(no);
		TargetChangeTurn(target.connectionToClient);
	}
	
	[TargetRpc]
	public void TargetChangeTurn(NetworkConnection conn) {
		Debug.Log("ITS MY TURN" + turnNumber.ToString());
		isTurn = true;
	}

	[TargetRpc]
	public void TargetRollInitiative(NetworkConnection conn) {
		initiative = Random.Range(1,20);
	}*/
}
