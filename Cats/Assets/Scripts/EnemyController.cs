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
	public enum EnemyType {Melee, Ranged, Special};
	public EnemyType enemyType;
//	public GameObject healthBar;
//	public GameObject hbar;

	public float attackCd = 1.5f;
	private float lastAttackTime =0;
	public int basicAttackDamage = 20;
	public float meleeAttackRange = 3;
	public float basicAttackForce = 3000f;
	public float grabAngle;
	public float grabRange;

	//For ranged attack
	public float rangedAttackRange = 90;
	public GameObject projectile;
	public float projectileSpeed;
	public float throwAngle = 45;

	public float EnemyCenterMassElevation = 1f;
	public float EnemyNormalAngularDrag =2f;

	private Ai AiScript;

	public GameObject attackIndicator;
	private GameObject _attackIndicator;

	public float projectileStartDist = 1.5f;


	
	void Awake ()
	{
		_attackIndicator = Instantiate (attackIndicator,transform.position + Vector3.up*2, transform.rotation) as GameObject;
		_attackIndicator.transform.parent = gameObject.transform;
		_attackIndicator.SetActive (false);
		//_attackIndicator.transform.position += Vector3.up * 2;
		lastAttackTime = - attackCd;
		GetComponent<Rigidbody> ().maxAngularVelocity = 20;


		AiScript = GetComponent<Ai> ();
		normalConstraints = GetComponent<Rigidbody>().constraints;
		IsNotConstrained = false;
		hbar = Instantiate (healthBar);
		hbar.name = gameObject.name + " bar";
		hbar.GetComponent<HealthBarManager> ().targetAgent = gameObject;
	}

	void Update(){
		//if AiScript.attackState
		//if (AiScript.lifeState == Ai.LIFE_STATE.IsAlive) 
		//{
		if (AiScript.attackState == Ai.ATTACK_STATE.CanAttackPlayer && base.IsNotConstrained == false)
		{
			if (enemyType == EnemyType.Ranged)
			{
				Debug.Log("Ranged attacking");
				BallisticAttack();
			}
			else 
			{
				Debug.Log("Melee attacking");
				StartCoroutine(Attack());
			}

		}
	

		//{
	}

	public override bool IsNotConstrained 
	{

		get { return base.isNotConstrained; }
		set {
			base.isNotConstrained = value;
			gameObject.GetComponent<Ai>().enabled = !value;
			if (value == true)
			{
				lastGrabbedTime = Time.time;
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
			}
			else
			{
				transform.position = new Vector3(transform.position.x, centerMassElevation, transform.position.z);
				transform.rotation = Quaternion.identity;
				GetComponent<Rigidbody>().constraints = normalConstraints;
				
			}
		}
	}

	protected override void Reset()
	{
		//transform.position = new Vector3 (transform.position.x, EnemyCenterMassElevation, transform.position.z);
		//transform.rotation = Quaternion.identity;
		IsNotConstrained = false;
		GetComponent<Rigidbody> ().angularDrag = EnemyNormalAngularDrag;
		//GetComponent<Ai> ().enabled = true;
	}




	private IEnumerator Attack ()
	{
		if (Time.time > lastAttackTime + attackCd ) 
		{
			//playAnimation("Attack");
			_attackIndicator.SetActive (true);
			yield return new WaitForSeconds(0.5f);
			_attackIndicator.SetActive (false);
			if(IsNotConstrained == false)
			{
			
				Debug.Log("melee Attacking ...");
				Collider closestEnemyCollider = getClosestAgentInCone(new string[]{"Player"}, meleeAttackRange,90);
				if (closestEnemyCollider != null)
				{
					closestEnemyCollider.gameObject.GetComponent<HealthManager>().TakeDamage(basicAttackDamage);
					closestEnemyCollider.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(closestEnemyCollider.transform.position - transform.position)*basicAttackForce);
					lastAttackTime = Time.time;
				}
			}
		}

	}

	void BallisticAttack ()
	{
		if (Time.time > lastAttackTime + attackCd) 
		{
			Collider closestEnemyCollider = getClosestAgentInCone(new string[]{"Player"}, rangedAttackRange,90);
			if(closestEnemyCollider != null)
			{

				GameObject ballisticProjectile = Instantiate (projectile, transform.position - projectileStartDist*Vector3.Normalize(transform.position - closestEnemyCollider.gameObject.transform.position),Quaternion.identity) as GameObject;
				if(ballisticProjectile != null)
				{
					ballisticProjectile.name = "Bullet";
					float distance = -projectileStartDist + Vector3.Distance(transform.position,closestEnemyCollider.gameObject.transform.position);
					float throwSpeed = Mathf.Sqrt(Physics.gravity.magnitude*distance/(2*Mathf.Sin (Mathf.Deg2Rad*throwAngle)*Mathf.Cos(Mathf.Deg2Rad*throwAngle)));
					Vector3	throwVector = Vector3.RotateTowards(Vector3.Normalize(transform.position - closestEnemyCollider.gameObject.transform.position),Vector3.down, throwAngle*Mathf.PI/180, 0.0f );
					
					ballisticProjectile.GetComponent<Rigidbody>().AddForce(-throwVector*throwSpeed, ForceMode.VelocityChange);
					
					lastAttackTime = Time.time;
				}
			}
		}
	}

	void RangedRayAttack ()
	{
		if (Time.time > lastAttackTime + attackCd) 
		{
			//playAnimation("Attack");
			
			
			Collider closestEnemyCollider = getClosestAgentInCone(new string[]{"Player"}, rangedAttackRange,90);
			
			GameObject clone = Instantiate (projectile, transform.position - 3*Vector3.Normalize(transform.position - closestEnemyCollider.gameObject.transform.position),Quaternion.identity) as GameObject;
			Rigidbody clonesrigid = clone.GetComponent<Rigidbody>();
			clone.name = "Bullet";
			clonesrigid.AddForce(-Vector3.Normalize(transform.position - closestEnemyCollider.gameObject.transform.position + Vector3.up)*50, ForceMode.VelocityChange);
			
			//			closestEnemyCollider.gameObject.GetComponent<HealthManager>().TakeDamage(basicAttackDamage);
			//			closestEnemyCollider.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(closestEnemyCollider.transform.position - transform.position)*basicAttackForce);
			
			
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
