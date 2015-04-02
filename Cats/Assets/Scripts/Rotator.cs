using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour {

	private Vector3 offset1;
	private float rotspeed;

	void Start() {

		rotspeed = Random.Range (-5, 5);
		offset1 = transform.position;
		
	}
	// Update is called once per frame
	void Update () 
	{
		//transform.rot (new Vector3 (15, 30, 35) * Time.deltaTime);
		transform.RotateAround (transform.position, new Vector3 (0, 1, 0), 200* Time.deltaTime);
		transform.RotateAround (offset1 + new Vector3(0.75f,0,0), new Vector3 (0, 1, 0), 30* rotspeed  * Time.deltaTime);
	}
}
