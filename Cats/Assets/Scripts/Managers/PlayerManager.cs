using UnityEngine;
using System.Collections;

public class PlayerManager : MonoBehaviour {

	public float collisionCd = 5.0f;
	public float lastCollisionTime;
	// Use this for initialization
	void Awake ()
	{
		lastCollisionTime = -collisionCd;
	}


	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
