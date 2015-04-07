using UnityEngine;
using System.Collections;

public class Cameracontroller : MonoBehaviour {

	//public GameObject player;
	private Vector3 offset;
	private GameObject[] players;
	public float scaleFactor;
	private float largestDistance;
	private Camera playerCamera;

	// Use this for initialization
	void Start () {
		players = GameObject.FindGameObjectsWithTag ("Player");
		offset = transform.position;
		playerCamera = Camera.main;

	
	}
	
	// Update is called once per frame
	void LateUpdate () {
		players = GameObject.FindGameObjectsWithTag ("Player");
		if (players.Length > 0) 
		{
			Vector3 avgPosition = new Vector3 ();

			foreach (GameObject player in players) 
			{
				avgPosition += player.transform.position;
			}
			avgPosition = avgPosition / players.Length;
			largestDistance = 0;
			float temp;
			foreach (GameObject player in players) 
			{
				temp = Vector3.Distance(avgPosition,player.transform.position);
				if (temp >= largestDistance )
				{
					largestDistance = temp;
				}
			}


			transform.position = avgPosition + offset;
			playerCamera.orthographicSize = 12.5f + largestDistance*2*scaleFactor;
		}
	}
}
