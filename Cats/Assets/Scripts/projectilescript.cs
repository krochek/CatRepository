using UnityEngine;
using System.Collections;

public class projectilescript : MonoBehaviour {

	// Use this for initialization
	private bool isExplosive = true;
	private bool hasExploded = false;
	public float expRange = 15f;
	public float expForce = 1000f;
	public int projExpDamage = 1;

	void Start () {
		Destroy (gameObject, 4f);
	}



	void OnCollisionEnter(Collision collision) {
		if (isExplosive)
		{
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, expRange ,1 << LayerMask.NameToLayer("Enemies"));
			Debug.Log(hitColliders.Length);

			foreach (Collider col in hitColliders) {
				Debug.Log(col);
				//col.attachedRigidbody.AddForce(Vector3.Normalize(-transform.position + col.transform.position)*expForce, ForceMode.VelocityChange);
				col.attachedRigidbody.AddExplosionForce(expForce,transform.position, expRange);
				col.gameObject.GetComponent<HealthManager>().TakeDamage(projExpDamage);
				

			}
			Destroy(gameObject);

			//collision.contacts[0]();
		}


		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
