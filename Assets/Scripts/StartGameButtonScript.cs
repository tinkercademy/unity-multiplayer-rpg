using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class StartGameButtonScript : NetworkBehaviour
{
	private Button button;
	private GameManager gm;
	
	public override void OnStartClient() {
		gm = GameObject.Find("NetworkManager").GetComponent<GameManager>();
	}
	
    // Start is called before the first frame update
    void Start()
    {
        button = gameObject.GetComponent<Button>();
		button.onClick.AddListener(StartGame);
    }
	
	void StartGame() {
		//gm.StartGame();
	}
}
