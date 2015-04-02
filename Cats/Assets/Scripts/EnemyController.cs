using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour 
{
	
	public float speed;
	public int count;
	public float turnSpeed;
	private GameObject closestPlayer;
	//private float health;

	public GameObject healthBar;

	public GameObject hbar;

	public float centerMassElevation = 1f;
	private RigidbodyConstraints normalConstraints;
	private float normalAngularDrag =2f;

	private bool isGrabbed;

	public bool IsGrabbed 
	{
		get { return isGrabbed; }
		set {
			isGrabbed = value;
			if (value == false)
			{
				transform.position = new Vector3(transform.position.x, centerMassElevation, transform.position.z);
			}
		}
	}


	//Rigidbody playerR;
	//Vector3 movement;
	
	
	void Awake (){
		IsGrabbed = false;
		hbar = Instantiate (healthBar);
		hbar.name = gameObject.name + " bar";
		hbar.GetComponent<HealthBarManager> ().targetAgent = gameObject;
		count = 0;
		normalConstraints = GetComponent<Rigidbody>().constraints;
	}

	void Update(){

	}

	public void Reset()
	{
		transform.position = new Vector3 (transform.position.x, centerMassElevation, transform.position.z);
		transform.rotation = Quaternion.identity;
		GetComponent<Rigidbody> ().angularDrag = normalAngularDrag;
	}

	
	void FixedUpdate()
	{
		float forward = 0f;
		float rotate = 0f;

		if (IsGrabbed == true) {
			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;

		} 
		else
		{

			GetComponent<Rigidbody>().constraints = normalConstraints;


		
		
		//transform.position += transform.forward * forward * Time.deltaTime * speed;
		//transform.RotateAround (transform.position, new Vector3 (0, 1, 0), rotate*turnspeed* Time.deltaTime);

			GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
			//int i = 0;
			closestPlayer = null;
			foreach (GameObject player in players)
			{
				if(closestPlayer == null || Vector3.Distance(player.transform.position, transform.position) < Vector3.Distance(closestPlayer.transform.position, transform.position))
				{
					closestPlayer = player;
			
				}
			}

			transform.rotation = Quaternion.Euler(closestPlayer.transform.position - transform.position);
		
			transform.LookAt (closestPlayer.transform);
		
			GetComponent<Rigidbody>().AddForce(transform.forward * speed * Time.deltaTime*1000);
		//}
		
		//GetComponent<Rigidbody>().
		}
		
	}
	void OnTriggerEnter(Collider other) {
		//Destroy(other.gameObject);
		/*if (other.gameObject.tag == "PickUp") {
			other.gameObject.SetActive(false);
			count = count +1;
		}*/
		
		
	}
}
