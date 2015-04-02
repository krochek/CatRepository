using UnityEngine;
using System.Collections;

public class OnEnviornmentCollision : MonoBehaviour 


{
	public float explRadius = 5f;

	void OnCollisionEnter (Collision col)
	{
		//Debug.Log (Random.value);
		if(col.gameObject.tag == "InteractiveEnv")
		{
			Collider[] hitColliders = Physics.OverlapSphere(transform.position, explRadius,1 << LayerMask.NameToLayer("IntEnv")); 
			int i = 0;
			while( i < hitColliders.Length)
			{
				Destroy(hitColliders[i].gameObject);
				i++;
			}
		}

	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
