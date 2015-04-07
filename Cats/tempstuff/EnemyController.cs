using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BreadcrumbAi;

public class EnemyController : MonoBehaviour 
{
	
	public float speed;
	public int count;
	public float turnSpeed;
	private GameObject closestPlayer;
	//private float health;

	public GameObject healthBar;
	public GameObject hbar;

	public float attackCd = 0.6f;
	private float lastAttackTime;
	public int basicAttackDamage = 20;
	public float basicAttackForce = 3000f;
	public float grabAngle;
	public float grabRange;


	public float centerMassElevation = 1f;
	private RigidbodyConstraints normalConstraints;
	private float normalAngularDrag =2f;

	private bool isGrabbed;

	private Ai AiScript;



	public bool IsGrabbed 
	{
		get { return isGrabbed; }
		set {
			isGrabbed = value;
			if (value == true)
			{
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
				AiScript.enabled = false;
			}
			else
			{
				Reset ();
				AiScript.enabled = true;
			}
		}
	}


	//Rigidbody playerR;
	//Vector3 movement;
	
	
	void Awake (){
		lastAttackTime = attackCd;
		AiScript = GetComponent<Ai> ();
		normalConstraints = GetComponent<Rigidbody>().constraints;
		IsGrabbed = false;
		hbar = Instantiate (healthBar);
		hbar.name = gameObject.name + " bar";
		hbar.GetComponent<HealthBarManager> ().targetAgent = gameObject;
		count = 0;
	}

	void Update(){
		//if AiScript.attackState
		//if (AiScript.lifeState == Ai.LIFE_STATE.IsAlive) 
		//{
			if (AiScript.attackState ==Ai.ATTACK_STATE.CanAttackPlayer)
			{
				Debug.Log("attacking");
				Attack();
			}
	
		//{
	}

	public void Reset()
	{
		transform.position = new Vector3 (transform.position.x, centerMassElevation, transform.position.z);
		transform.rotation = Quaternion.identity;
		GetComponent<Rigidbody> ().angularDrag = normalAngularDrag;
		GetComponent<Rigidbody> ().constraints = normalConstraints;


	}

	Collider getAgentsInRange (string[] layernames)
	{
		int combinedLayers = 0;
		foreach (string layername in layernames) {
			combinedLayers += 1 << LayerMask.NameToLayer(layername);
		}
		//Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, grabRange,1 << LayerMask.NameToLayer(layername));
		Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, grabRange,combinedLayers);
		//Debug.Log (hitColliders.ToString ());
		if (enemiesInRange.Length != 0) {
			List<Collider> withinangleColliders = new List<Collider> ();
			
			foreach (Collider element in enemiesInRange) {
				Vector3 directionToTarget = -transform.position + element.transform.position;
				float angle = Vector3.Angle (directionToTarget, transform.forward);
				
				if (Mathf.Abs (angle) < grabAngle) {
					withinangleColliders.Add (element);
				}
			}
			Collider closest = withinangleColliders [0];
			foreach (Collider element in withinangleColliders) {
				if (Vector3.Distance (element.transform.position, transform.position) <= Vector3.Distance (closest.transform.position, transform.position)) {
					closest = element;	
				}
			}
			
			return closest;
		}
		{
			return null;
		}
		
	}

	void Attack ()
	{
		if (Time.time > lastAttackTime + attackCd) 
		{
			//playAnimation("Attack");
			
			
			Collider closestEnemyCollider = getAgentsInRange(new string[]{"Player"});
			closestEnemyCollider.gameObject.GetComponent<HealthManager>().TakeDamage(basicAttackDamage);
			closestEnemyCollider.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(closestEnemyCollider.transform.position - transform.position)*basicAttackForce);
			
			
			lastAttackTime = Time.time;
		}
	}

	void FixedUpdate()
	{
//		float forward = 0f;
//		float rotate = 0f;
//
//		if (IsGrabbed == true) {
//			GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
//
//		} 
//		else
//		{
//
//			GetComponent<Rigidbody>().constraints = normalConstraints;
//
//
//		
//		
//		//transform.position += transform.forward * forward * Time.deltaTime * speed;
//		//transform.RotateAround (transform.position, new Vector3 (0, 1, 0), rotate*turnspeed* Time.deltaTime);
//
//			GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
//			//int i = 0;
//			closestPlayer = null;
//			foreach (GameObject player in players)
//			{
//				if(closestPlayer == null || Vector3.Distance(player.transform.position, transform.position) < Vector3.Distance(closestPlayer.transform.position, transform.position))
//				{
//					closestPlayer = player;
//			
//				}
//			}
//
//			transform.rotation = Quaternion.Euler(closestPlayer.transform.position - transform.position);
//		
//			transform.LookAt (closestPlayer.transform);
//		
//			GetComponent<Rigidbody>().AddForce(transform.forward * speed * Time.deltaTime*1000);
//		//}
//		
//		//GetComponent<Rigidbody>().
//		}
		
	}
	void OnTriggerEnter(Collider other) {
		//Destroy(other.gameObject);
		/*if (other.gameObject.tag == "PickUp") {
			other.gameObject.SetActive(false);
			count = count +1;
		}*/
		
		
	}
}
