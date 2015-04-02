using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthManager : MonoBehaviour {

	public int startingHealth = 100;
	private int currentHealth;
	public Slider healthSlider;
	bool isDead;
	bool damaged;
	//EnemyController enemyController;
	//PlayerController playerController;
	GameObject controller;

	// Use this for initialization
	void awake ()
	{


	}

	public void TakeDamage (int amount)
	{
		damaged = true;
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
		if(gameObject.layer == LayerMask.NameToLayer("Enemies"))
		{
			GetComponent<EnemyController> ().enabled = false;
		}
		else if(gameObject.layer == LayerMask.NameToLayer("Players"))
		{
			GetComponent<PlayerController> ().enabled = false;
		}
		Destroy (healthSlider.gameObject, 2f);
		Destroy (gameObject, 2f);

	}

	void Start () {
		if(gameObject.layer == LayerMask.NameToLayer("Enemies"))
		{
			controller = GetComponent<EnemyController> ().hbar;
		}
		else if(gameObject.layer == LayerMask.NameToLayer("Players"))
		{
			controller = GetComponent<PlayerController> ().hbar;
		}

		//enemyController = GetComponent<EnemyController> ();
		//temp = GameObject.Find (gameObject.name + " bar");
		//temp = enemyController.hbar;
		healthSlider =controller.GetComponent<Slider>();
		Debug.Log (healthSlider);
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
