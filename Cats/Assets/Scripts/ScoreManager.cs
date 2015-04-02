using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

	public static int score;
	Text text;


	// Use this for initialization
	void Start () {
		text = GetComponent<Text> ();
		//score = 0;
	}
	
	// Update is called once per frame
	void Update () {
//		GameObject player = GameObject.Find("Player");
//		 
//
//
//
//		PlayerController playercontroller = player.GetComponent<PlayerController>();
//		//PlayerController playercontroller =  GetComponent<PlayerController>();
//		//Debug.Log (playercontroller.count);
//		text.text = "Score " + playercontroller.count;
	}
}
