using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Mirror;

public class PlayerUIScript : NetworkBehaviour
{	
	[SyncVar]
	public bool isReady = false;
	
	[SyncVar]//later can remove this syncvar?
	public string playerName;
	
	[SyncVar]
	public int playerNo;//set by server when u join
	
	public Sprite readySprite;
	public Sprite notReadySprite;
	public InputField nameInputField;
	
	public Text turnOrderTextPrefab;
	public Image readyIndicatorImagePrefab;
	public Text readyIndicatorTextPrefab;
	public Text pickSpriteTextPrefab;
	public Button choiceButton;
	
	public bool chosenChar = false;//only edited on server
	
	private GameManager gm;
	private PlayerScript playerScript;
	
	private Transform canvasTransform;
	private Button readyButton;
	private InputField nameInput;
	private List<Button> choiceButtonList = new List<Button>();
	private List<Text> readyIndicatorTextList = new List<Text>();
	private List<Image> readyIndicatorImageList = new List<Image>();
	private List<Text> turnOrderTextList = new List<Text>();
	private GameObject[] players;
	
	
	public override void OnStartServer() {
		base.OnStartServer();
		gm = GameObject.FindGameObjectWithTag("NetworkManager").GetComponent<GameManager>();
	}
	
	public override void OnStartClient() {
		base.OnStartClient();
		
		if (!base.hasAuthority) {
			return;
		}
		
		canvasTransform = GameObject.FindGameObjectWithTag("MainCanvas").transform;
		playerScript = gameObject.GetComponent<PlayerScript>();
		
		CmdCreateReadyIndicators();
		
		readyButton = GameObject.FindGameObjectWithTag("ReadyButton").GetComponent<Button>();
		readyButton.onClick.AddListener(CmdPlayerReady);
		
		players = GameObject.FindGameObjectsWithTag("Player");
		CmdSetPlayerNo(players.Length);	//sets his player number based on when he joined the server
		CmdSetName("Player " + players.Length.ToString());
		
		CreateNameInputField();
		CreateCharChoiceButtons();
	}
	
	//Ready Indicators display each player's name, and if they are ready or not, on every client
	//this is called whenever a new client joins
	//it creates a new ready indicator for that player on all existing clients
	//it also creates all the existing ready indicators for that new player
	[Command]
	private void CmdCreateReadyIndicators() {
		GameObject[] connectedPlayers = gm.GetPlayers();
		TargetCreateReadyIndicators(connectedPlayers);//creating all the existing ready indicators with the array of all players
		
		GameObject lastPlayer = connectedPlayers[connectedPlayers.Length-1];
		Array.Resize(ref connectedPlayers, connectedPlayers.Length-1);
		for (int i=0; i<connectedPlayers.Length;i++) {
			PlayerUIScript playerUI = connectedPlayers[i].GetComponent<PlayerUIScript>();
			playerUI.TargetCreateReadyIndicators(new GameObject[]{lastPlayer});//adding the new player to all existing clients
		}
	}
	
	[TargetRpc]
	public void TargetCreateReadyIndicators(GameObject[] connectedPlayers) {
		for (int i=0; i<connectedPlayers.Length; i++) {
			PlayerUIScript playerUIScript = connectedPlayers[i].GetComponent<PlayerUIScript>();
			AddReadyIndicators(playerUIScript);
		}
	}
	
	//generates a text and image object for a player, adds it to the list of texts and images that exist
	private void AddReadyIndicators(PlayerUIScript playerUIScript) {
		Text readyText = Instantiate(readyIndicatorTextPrefab, canvasTransform);
		Image readyImage = Instantiate(readyIndicatorImagePrefab, canvasTransform);
		
		Vector2 textPosition = readyText.GetComponent<RectTransform>().anchoredPosition;
		Vector2 imagePosition = readyText.GetComponent<RectTransform>().anchoredPosition;
		textPosition.y -= 40*readyIndicatorTextList.Count;
		imagePosition.y -= 40*readyIndicatorImageList.Count;
		readyText.GetComponent<RectTransform>().anchoredPosition = textPosition;
		readyImage.GetComponent<RectTransform>().anchoredPosition = imagePosition;
		readyText.text = string.IsNullOrEmpty(playerUIScript.playerName) ? ("Player " + players.Length.ToString()) : playerUIScript.playerName;//in case playername hasn't updated from server to client
		print("generating " + playerUIScript.playerName);
		readyImage.sprite = playerUIScript.isReady?readySprite:notReadySprite;
		readyIndicatorTextList.Add(readyText);
		readyIndicatorImageList.Add(readyImage);
	}

	//when someone changes name/ready status, modify their ready indicator
	[TargetRpc]
	private void TargetUpdateReadyIndicators(int index, string name, bool ready) {
		readyIndicatorTextList[index].text = name;
		readyIndicatorImageList[index].sprite = ready?readySprite:notReadySprite;
	}
	
	//destroyed when the game starts
	[TargetRpc]
	public void TargetDestroyReadyIndicators() {
		foreach (Text text in readyIndicatorTextList) {
			Destroy(text.gameObject);
		}
		foreach (Image image in readyIndicatorImageList) {
			Destroy(image.gameObject);
		}
	}
	
	//when a player clicks the ready button, this is called
	//change his ready indicator to indicate the change of status
	//checks if all players are ready - if they are, start the game
	[Command]
	void CmdPlayerReady() {
		if (gm.GetGameStarted()) {
			return;
		}
		if (chosenChar == false) {
			TargetPickSpriteText();
			return;
		}
		isReady ^= true;
		
		TargetChangeReadyButton(isReady);
		
		foreach (GameObject player in gm.GetPlayers()) {
			player.GetComponent<PlayerUIScript>().TargetUpdateReadyIndicators(playerNo-1, playerName, isReady);
		}
		
		gm.CheckGameStart();
	}
	
	//if player tries to click ready without picking a character, this triggers
	[TargetRpc]
	public void TargetPickSpriteText() {
		StartCoroutine(PickSpriteText());
	}
	
	IEnumerator PickSpriteText() {
		Text pickSpriteText = Instantiate(pickSpriteTextPrefab, canvasTransform);
		yield return new WaitForSeconds(0.8f);
		Destroy(pickSpriteText.gameObject);
	}
	
	[TargetRpc]
	public void TargetChangeReadyButton(bool change) {
		readyButton.GetComponentInChildren<Text>().text = change ? "READY!!" : "NOT READY!!";
	}

	[Command]
	private void CmdSetPlayerNo(int no) {
		playerNo=no;
	}
	
	[Command]
	private void CmdSetName(string name) {
		playerName = name;
	}
	
	private void CreateNameInputField() {
		 nameInput = Instantiate(nameInputField, canvasTransform);
		 nameInput.onEndEdit.AddListener(delegate {CmdChangeName(nameInput.text);});
	}
	
	
	[Command]
	private void CmdChangeName(string name) {
		playerName = name;
		foreach (GameObject player in gm.GetPlayers()) {
			player.GetComponent<PlayerUIScript>().TargetUpdateReadyIndicators(playerNo-1, name, isReady);
		}
	}
	
	//creates options for character based on the number of sprites/characters available for him to choose
	private void CreateCharChoiceButtons() {
		for (int i=1;i<playerScript.sprites.Count;i++) {
			Button choice = Instantiate(choiceButton, canvasTransform);
			Vector2 buttonPosition = choice.GetComponent<RectTransform>().anchoredPosition;
			buttonPosition.x += 40*i;
			choice.GetComponent<RectTransform>().anchoredPosition = buttonPosition; 
			choice.GetComponentInChildren<Text>().text = (i).ToString();
			int temp = i;//keeps memory address to assign to setsprite, i=0 is the tombstone sprite
			choice.onClick.AddListener(delegate {playerScript.CmdSetChar(temp);});
			choice.onClick.AddListener(SetChar);
			
			
			//creating a tooltip to display the stats when he hovers over the button
			EventTrigger trigger = choice.GetComponent<EventTrigger>();			
			
			EventTrigger.Entry entry = new EventTrigger.Entry();
			entry.eventID = EventTriggerType.PointerEnter;
			PlayerStat ps = playerScript.statList[temp-1];
			string tooltipString = "Name: " + ps.name + "\nHealth: " + ps.health + "\nDamage: " + ps.damage + "\nAttack Range: " + ps.attackRange;
			entry.callback.AddListener(delegate {TooltipScript.ShowTooltip_Static(tooltipString);});
			
			EventTrigger.Entry entry2 = new EventTrigger.Entry();
			entry2.eventID = EventTriggerType.PointerExit;
			entry2.callback.AddListener(delegate {TooltipScript.HideTooltip_Static();});
			
			trigger.triggers.Add(entry);
			trigger.triggers.Add(entry2);
			
			//keep list of choicebuttons to destroy them when the game starts
			choiceButtonList.Add(choice);
		}
	}
	
	//once he has chosen a sprite, chosenchar is true so that he is allowed to click ready
	private void SetChar() {
		chosenChar = true;
	}
	
	//creates turn order texts to displays the list of players once the game starts - the active player will have his name bolded
	[TargetRpc]
	public void TargetSetTurnOrderText(string[] textList, int index) {
		if (!base.hasAuthority) {
			return;
		}
		print(playerName);
		for (int i=0;i<textList.Length;i++) {
			Text turnOrderText = Instantiate(turnOrderTextPrefab, canvasTransform);
			turnOrderText.text = textList[i];
			Vector2 textPosition = turnOrderText.GetComponent<RectTransform>().anchoredPosition;
			textPosition.y -= 40*i;
			turnOrderText.GetComponent<RectTransform>().anchoredPosition = textPosition; 
			
			if (i == index) {
				turnOrderText.fontStyle = FontStyle.Bold;
			}
			
			turnOrderTextList.Add(turnOrderText);
		}
	}
	
	//when active player changes
	[TargetRpc]
	public void TargetChangeTurnOrderText(int prev, int index) {
		turnOrderTextList[prev].fontStyle = FontStyle.Normal;
		turnOrderTextList[index].fontStyle = FontStyle.Bold;
	}
	
	//when player dies, remove his turn order text
	[TargetRpc]
	public void TargetDestroyTurnOrderText(int index) {
		Text text = turnOrderTextList[index];
		turnOrderTextList.RemoveAt(index);
		Destroy(text.gameObject);
	}
	
	void OnDestroy() {
		foreach (Text turnOrderText in turnOrderTextList) {
			Destroy(turnOrderText);
		}
	}
	
	[TargetRpc]
	public void TargetDestroyReadyButton() {
		Destroy(GameObject.FindGameObjectWithTag("ReadyButton"));
	}
	
	[TargetRpc]
	public void TargetDestroyNameInput() {
		Destroy(nameInput.gameObject);
	}
	
	[TargetRpc]
	public void TargetDestroyChoiceButtons() {
		foreach (Button button in choiceButtonList) {
			Destroy(button.gameObject);
		}
	}
	
}
