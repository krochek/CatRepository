using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using BreadcrumbAi;


public class AgentController : MonoBehaviour {



	public float centerMassElevation = 1f;
	protected RigidbodyConstraints normalConstraints;
	public float normalAngularDrag = 0.3f;

	public GameObject healthBar;
	public GameObject hbar;

	protected bool isNotConstrained;
	//public float lastReleasedTime = 0;
	public float lastGrabbedTime = 0;

	//for after throw explosion on landing or hitting anything.
	public bool justThrown;
	
//	public AgentController()
//	{
//
//	}

	public virtual bool IsNotConstrained 
	{
		get { return isNotConstrained; }
		set {
			isNotConstrained = value;
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

	protected Collider getClosestAgentInCone (string[] layernames, float range, float angle)
	{
		int combinedLayers = 0;
		foreach (string layername in layernames) {
			combinedLayers += 1 << LayerMask.NameToLayer (layername);
		}
		//Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, grabRange,1 << LayerMask.NameToLayer(layername));
		Collider[] enemiesInRange = Physics.OverlapSphere (transform.position, range, combinedLayers);
		//Debug.Log (hitColliders.ToString ());
		if (enemiesInRange.Length != 0) {
			List<Collider> withinangleColliders = new List<Collider> ();
			
			foreach (Collider element in enemiesInRange) {
				Vector3 directionToTarget = -transform.position + element.transform.position;
				float tempangle = Vector3.Angle (directionToTarget, transform.forward);
				
				if (Mathf.Abs (tempangle) < angle) {
					withinangleColliders.Add (element);
				}
			}
			if (withinangleColliders.Count > 0) {
				Collider closest = withinangleColliders [0];
				foreach (Collider element in withinangleColliders) {
					if (Vector3.Distance (element.transform.position, transform.position) <= Vector3.Distance (closest.transform.position, transform.position)) {
						closest = element;	
					}
				}
				
				
				return closest;
			} else {
				return null;
			}
		} else {
			return null;
		}
	}

	protected void Explosion(Vector3 expPoint, float expForce, float expRadius, string excludedLayer, string damageLayer, int damage)
	{
		Collider[] explodees = Physics.OverlapSphere (expPoint, expRadius);
		foreach (Collider explodee in explodees) 
		{
			if (explodee.gameObject.layer != LayerMask.NameToLayer(excludedLayer) && explodee.attachedRigidbody != null)
			{
				if (explodee.gameObject.layer == LayerMask.NameToLayer("Enemy"))
				    {
						explodee.gameObject.GetComponent<AgentController>().IsNotConstrained = true;
						explodee.GetComponent<Ai>().enabled = false;
						explodee.attachedRigidbody.angularDrag = 0;
						StartCoroutine(ReleaseConstraints(explodee.gameObject));
					}

				//This is just code I played with to make the explosion more random. This is not a the way to do this.
				//Random doesn't work well in proximity to each call. creates one true random and then semi copies of it.
				explodee.attachedRigidbody.AddExplosionForce(expForce,expPoint,expRadius,10f, ForceMode.VelocityChange);
				Vector3 randomVector = Random.onUnitSphere;
				Vector3 crossVector = Vector3.Normalize( Vector3.Cross(expPoint-explodee.transform.position, Vector3.up));
				crossVector = crossVector +randomVector*0.5f;
				explodee.attachedRigidbody.AddTorque(crossVector*200*Random.Range(0.5f,4.5f), ForceMode.VelocityChange);
				if (explodee.gameObject.layer == LayerMask.NameToLayer(damageLayer))
				{
					explodee.gameObject.GetComponent<HealthManager>().TakeDamage(damage);
				}
			}
			
		}
		
	}

	protected virtual void Reset()
	{
		transform.position = new Vector3 (transform.position.x, centerMassElevation, transform.position.z);
		transform.rotation = Quaternion.identity;
		GetComponent<Rigidbody> ().angularDrag = normalAngularDrag;
	}

	protected IEnumerator ReleaseConstraints (GameObject agentToRelease)
	{
	//	Debug.Log (agentToRelease.gameObject.GetComponent<AgentController> ().lastReleasedTime);
		yield return new WaitForSeconds(3);

		if (agentToRelease != null && (Time.time - agentToRelease.gameObject.GetComponent<AgentController>().lastGrabbedTime) > 3f) 
		{ 

		//	Debug.Log (Time.time - agentToRelease.gameObject.GetComponent<AgentController>().lastReleasedTime);
			agentToRelease.GetComponent<AgentController> ().IsNotConstrained = false;
			agentToRelease.GetComponent<AgentController> ().Reset ();
		}

	}
	public IEnumerator slowTime(float sec, float rate)
	{
		yield return new WaitForSeconds (0.1f);
		Time.timeScale = rate;
		Time.fixedDeltaTime = 0.02f * Time.timeScale;
		yield return new WaitForSeconds(sec);
		Time.timeScale = 1.0f;
		Time.fixedDeltaTime = 0.02f;
		
	}

	void Awake() 
	{

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
