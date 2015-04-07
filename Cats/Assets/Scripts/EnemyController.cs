using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BreadcrumbAi;

public class EnemyController : AgentController 
{
	
	public float speed;
//	public int count;
	public float turnSpeed;
	private GameObject closestPlayer;
	//private float health;

//	public GameObject healthBar;
//	public GameObject hbar;

	public float attackCd = 0.6f;
	private float lastAttackTime;
	public int basicAttackDamage = 20;
	public float basicAttackForce = 3000f;
	public float grabAngle;
	public float grabRange;


	public float EnemyCenterMassElevation = 1f;
	public float EnemyNormalAngularDrag =2f;

	private Ai AiScript;




	
	void Awake (){
		lastAttackTime = attackCd;
		AiScript = GetComponent<Ai> ();
		normalConstraints = GetComponent<Rigidbody>().constraints;
		IsGrabbed = false;
		hbar = Instantiate (healthBar);
		hbar.name = gameObject.name + " bar";
		hbar.GetComponent<HealthBarManager> ().targetAgent = gameObject;
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

	protected override void Reset()
	{
		transform.position = new Vector3 (transform.position.x, EnemyCenterMassElevation, transform.position.z);
		transform.rotation = Quaternion.identity;
		GetComponent<Rigidbody> ().angularDrag = EnemyNormalAngularDrag;
		GetComponent<Ai> ().enabled = true;
	}




	void Attack ()
	{
		if (Time.time > lastAttackTime + attackCd) 
		{
			//playAnimation("Attack");
			
			
			Collider closestEnemyCollider = getAgentsInRange(new string[]{"Player"}, grabRange,grabAngle);
			closestEnemyCollider.gameObject.GetComponent<HealthManager>().TakeDamage(basicAttackDamage);
			closestEnemyCollider.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(closestEnemyCollider.transform.position - transform.position)*basicAttackForce);
			
			
			lastAttackTime = Time.time;
		}
	}

	void FixedUpdate()
	{

	}
	//void OnTriggerEnter(Collider other) {
		//Destroy(other.gameObject);
		/*if (other.gameObject.tag == "PickUp") {
			other.gameObject.SetActive(false);
			count = count +1;
		}*/
		
		

}
