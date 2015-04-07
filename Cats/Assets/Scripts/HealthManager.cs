using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthManager : MonoBehaviour {

	public int startingHealth = 100;
	private int currentHealth;
	public Slider healthSlider;
	bool isDead;
	//bool damaged;
	//EnemyController enemyController;
	//PlayerController playerController;
	GameObject controller;

	// Use this for initialization
	void awake ()
	{


	}

	public void TakeDamage (int amount)
	{
		//damaged = true;
		currentHealth -= amount;
		healthSlider.value = currentHealth;
		if (currentHealth <=0 && isDead == false ) 
		{
			Death();
		}

	}

	void Death ()
	{
		isDead = true;
		if(gameObject.layer == LayerMask.NameToLayer("Enemy"))
		{
			GetComponent<EnemyController> ().enabled = false;
		}
		else if(gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			GetComponent<PlayerController> ().enabled = false;
		}
		Destroy (healthSlider.gameObject, 2f);
		Destroy (gameObject, 2f);

	}

	void Start () {
		if(gameObject.layer == LayerMask.NameToLayer("Enemy"))
		{
			controller = GetComponent<EnemyController> ().hbar;
		}
		else if(gameObject.layer == LayerMask.NameToLayer("Player"))
		{
			controller = GetComponent<PlayerController> ().hbar;
		}

		//enemyController = GetComponent<EnemyController> ();
		//temp = GameObject.Find (gameObject.name + " bar");
		//temp = enemyController.hbar;
		//Debug.Log ("we got this far");
		healthSlider = controller.GetComponent<Slider>();
		//Debug.Log (healthSlider);
		currentHealth = startingHealth;
		healthSlider.value = startingHealth;
//		Debug.Log (gameObject.name);
//		temp = GameObject.Find (gameObject.name + " bar");
//		healthSlider =temp.GetComponent<Slider>();
//		healthSlider.value = startingHealth;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
