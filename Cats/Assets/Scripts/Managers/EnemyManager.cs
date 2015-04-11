using UnityEngine;
using System.Collections;


public class EnemyManager : MonoBehaviour
{
	//
	private float numEnemies;
	private bool gameOver;

	//public PlayerHealth playerHealth;       // Reference to the player's heatlh.
	public GameObject rangedEnemy;                // The enemy prefab to be spawned.
	public GameObject meleeEnemy;
	
	public float spawnTime = 3f;            // How long between each spawn.
	public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.
	public float maxEnemies;


	public float NumEnemies
	{
		get { return numEnemies; }
		set {
			numEnemies = value;
			}
	}

	public bool GameOver
	{
		get { return gameOver; }
		set {
			gameOver = value;
			if (value == true)
			{
				
			}
			else
			{
				
			}
		}
	}

	void Start ()
	{
		// Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
		NumEnemies = 0;
		gameOver = false;
		StartCoroutine(Spawn());
	}
	
	
	public IEnumerator Spawn ()
	{
		// If the player has no health left...
		while (gameOver == false) {
			while (NumEnemies < maxEnemies) {
				int i = Random.Range (0, spawnPoints.Length);
				int j = Random.Range (0, 2);
				if (j==0) 
				{
					Instantiate (meleeEnemy, spawnPoints [i].position, spawnPoints [i].rotation);
				}
				else{
					Instantiate (rangedEnemy, spawnPoints [i].position, spawnPoints [i].rotation);
				}
				NumEnemies += 1;
				yield return new WaitForSeconds(3);

				// ... exit the function.

			}
			yield return new WaitForSeconds(3);
		}


	}
}