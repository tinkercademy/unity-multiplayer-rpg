using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkManager
{
	public List<Sprite> sprites = new List<Sprite>();
	
	private bool gameStarted = false;
	private int indexCurrentPlayer = 0;
	private List<PlayerScript> players = new List<PlayerScript>();
	private List<PlayerUIScript> playersUI = new List<PlayerUIScript>();
	
	//whenever turn changes, call this
	public void ChangePlayer() {
		players[indexCurrentPlayer].isTurn = false;//set the player who has taken his turn to false
		int indexPreviousPlayer = indexCurrentPlayer;
		
		//check if any player has died, doesn't account for current player dying on his turn
		for (int i = players.Count-1; i>=0; i--) {
			if (players[i].stats.health <= 0) {
				GameEnd(players[i], false);
				players.RemoveAt(i);
				
				if (players.Count == indexCurrentPlayer) {//if last player in list kills first player
					indexCurrentPlayer--;
				}
				
				//remove turn order text
				DestroyTurnOrderText(i);
			}
		}
		
		if (players.Count == 1) {
			GameEnd(players[0], true);
			return;
		}
		
		if (players.Count == indexCurrentPlayer+1) {//adjust the current player
			indexCurrentPlayer = 0;
		}
		else {
			indexCurrentPlayer++;
		}
		players[indexCurrentPlayer].isTurn = true;//set the new current player's turn to true
		ChangeTurnOrderText(indexPreviousPlayer, indexCurrentPlayer);//breaks the text thing
	}
	
	//sets which screen to show player - win or loss
	private void GameEnd(PlayerScript player, bool win) {
		player.TargetEndGame(win);
	}
	
	
	public bool GetGameStarted() {
		return gameStarted;
	}
	
	//if all players ready, can start game
	public void CheckGameStart() {
		foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
			PlayerUIScript playerUIScript = player.GetComponent<PlayerUIScript>();
			if (!playerUIScript.isReady) {
				return;
			}
		}
		StartGame();
	}
	
	
	//randomises player order, creates turn order texts and sets turn to first player
	public void StartGame() {
		gameStarted = true;
		
		//randomising the turn order
		List<PlayerScript> tempPlayers = new List<PlayerScript>();
		List<PlayerUIScript> tempPlayersUI = new List<PlayerUIScript>();
		foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
			tempPlayers.Add(player.GetComponent<PlayerScript>());
			tempPlayersUI.Add(player.GetComponent<PlayerUIScript>());
		}
		while (tempPlayers.Count != 0) {
			int i = Random.Range(0,tempPlayers.Count);
			players.Add(tempPlayers[i]);
			playersUI.Add(tempPlayersUI[i]);
			tempPlayers.RemoveAt(i);
			tempPlayersUI.RemoveAt(i);
		}
	
		SetTurnOrderText(0);
		DestroyReadyButtonsAndIndicators();
		
		//setting it to the first player's turn
		players[0].isTurn = true;
	}
	
	//function to set the turnordertext in each playerscript
	private void SetTurnOrderText(int index) {
		string[] turnOrderText = new string[players.Count];
		for (int i=0;i<players.Count;i++) {
			turnOrderText[i] = ((i+1).ToString() + "\t" + playersUI[i].playerName);
		}
		foreach (PlayerUIScript playerUI in playersUI) {
			playerUI.TargetSetTurnOrderText(turnOrderText, index);//have to make it call on client
		}
	}
	
	//destroy UI after game starts
	private void DestroyReadyButtonsAndIndicators() {
		foreach (PlayerUIScript playerUI in playersUI) {
			playerUI.TargetDestroyReadyButton();
			playerUI.TargetDestroyReadyIndicators();
			playerUI.TargetDestroyChoiceButtons();
			playerUI.TargetDestroyNameInput();
		}
	}
	
	private void ChangeTurnOrderText(int prev, int index) {
		foreach (PlayerUIScript playerUI in playersUI) {
			playerUI.TargetChangeTurnOrderText(prev, index);//have to make it call on client as well
		}
	}
	
	private void DestroyTurnOrderText(int index) {
		foreach (PlayerUIScript playerUI in playersUI) {
			playerUI.TargetDestroyTurnOrderText(index);
		}
	}
	
	public GameObject[] GetPlayers() {
		return GameObject.FindGameObjectsWithTag("Player");
	}
}
