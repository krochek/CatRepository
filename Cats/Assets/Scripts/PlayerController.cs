using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : AgentController
{
/*
  Co-op Mechanics:

	Implemented:
	projectile at the direction of the vector between players.
	


	Everything else:
	(1) laser beam between players.
	(2) intersecting Laser beam (Where the lasers cross there is either explosion either a new laser beam with the a direction of the average of the players)
	(3) Yarn ball that a player can roll at a direction. As it rolls it unrevals and decreases in radius. A player can catch the ball and throw it again. 
	(4) Explosion when players collide.
	
*/




	public float speed;
	public float turnSpeed;
	public float maxSpeed;

	public float grabRange;
	private bool isGrabbing;
	private FixedJoint grabbingJoint;
	private GameObject grabbedAgent;
	public float throwSpeed ;
	public float grabAngle;

	public GameObject projectile;
	public float projectileSpeed;

	public float attackCd = 0.6f;
	private float lastAttackTime;

	public float basicAttackForce = 3000f;
	public int basicAttackDamage = 20;


	//grabbing helpers
	public float PlayerCenterMassElevation = 1f;
//	private RigidbodyConstraints normalConstraints;
	public float PlayerNormalAngularDrag = 20f;

	// for collision explosion between players
	public float betweenPlayerExpSpeed = 10;
	public float betweenPlayerExpRadius = 20;
	public int expDamage = 35;



	public AudioClip ExplosionSound;
	private AudioSource source;


	



	private KeyCode[] keybinds;



	void Awake()
	{
		GetComponent<Rigidbody> ().maxAngularVelocity = 20;

		justThrown = false;
		hbar = Instantiate (healthBar);
		hbar.name = gameObject.name + " bar";
		hbar.GetComponent<HealthBarManager> ().targetAgent = gameObject;
		normalConstraints = GetComponent<Rigidbody>().constraints;
		IsNotConstrained = false;
		source = GetComponent<AudioSource>();

	}

	void Start (){

		lastAttackTime = attackCd;

		switch (gameObject.name) 
		{
		case "Player1":
			//bindToPlayer();
			//keybinds = new KeyCode[7]{KeyCode.D , KeyCode.A , KeyCode.W, KeyCode.S, KeyCode.V, KeyCode.B, KeyCode.C};
			keybinds = new KeyCode[7]{KeyCode.D , KeyCode.A , KeyCode.W, KeyCode.S, KeyCode.Joystick4Button0, KeyCode.B, KeyCode.C};
			break;

		case "Player2":
			//keybinds = new KeyCode[7]{KeyCode.RightArrow , KeyCode.LeftArrow , KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.Keypad0,KeyCode.KeypadEnter, KeyCode.KeypadPeriod};
			keybinds = new KeyCode[7]{KeyCode.RightArrow , KeyCode.LeftArrow , KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.Joystick2Button0,KeyCode.KeypadEnter, KeyCode.KeypadPeriod};
			break;
		}

		isGrabbing = false;
	}

	void bindToPlayer()
	{
		Mesh mesh = new Mesh ();

		Vector3[] vertices = new Vector3[]
		{
			new Vector3( 3, 0,  1),
			new Vector3( 3, 0, -1),
			new Vector3(-3, 0,  1),
			new Vector3(-3, 0, -1),
		};
		
		Vector2[] uv = new Vector2[]
		{
			new Vector2(1, 1),
			new Vector2(1, 0),
			new Vector2(0, 1),
			new Vector2(0, 0),
		};
		
		int[] triangles = new int[]
		{
			0, 1, 2,
			2, 1, 3,
		};
		
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;

		GameObject axe1 = new GameObject ();
		axe1.name = "Axe1";
		axe1.AddComponent<MeshFilter> ();
		axe1.GetComponent<MeshFilter>().mesh = mesh;
		axe1.AddComponent<MeshRenderer> ();
		axe1.AddComponent<BoxCollider> ();
		axe1.transform.position = transform.position;
		//axe1.transform.Translate (Vector3.up * 2);

		HingeJoint hinge1 = gameObject.AddComponent<HingeJoint> ();
		hinge1.axis = Vector3.up;
		axe1.AddComponent<Rigidbody> ();
		hinge1.connectedBody = axe1.GetComponent<Rigidbody>();

		GameObject axe2 = new GameObject ();
		axe2.name = "Axe2";
		axe2.AddComponent<MeshFilter> ();
		axe2.GetComponent<MeshFilter>().mesh = mesh;
		axe2.AddComponent<MeshRenderer> ();
		axe2.AddComponent<BoxCollider> ();
		axe2.transform.position = GameObject.Find("Player2").transform.position;
		//axe2.transform.Translate (Vector3.up * 2);

		HingeJoint hinge2 = GameObject.Find("Player2").AddComponent<HingeJoint> ();
		hinge2.axis = Vector3.up;
		axe2.AddComponent<Rigidbody> ();
		hinge2.connectedBody = axe2.GetComponent<Rigidbody>();

		FixedJoint fixed12 = axe1.AddComponent<FixedJoint> ();
		fixed12.connectedBody = axe2.GetComponent<Rigidbody> ();


	}






	void PlayerAsProjectile (Collision collision)
	{
		if (justThrown == true)
		{
			Explosion(collision.contacts[0].point,betweenPlayerExpSpeed*2f,betweenPlayerExpRadius,"Player","Enemy",expDamage);
			//	StartCoroutine(slowTime(0.7f,0.25f));
			justThrown = false;
			
		}
	}

	void OnCollisionEnter(Collision collision) {

		PlayerAsProjectile (collision);

		if(Time.time > gameObject.GetComponentInParent<PlayerManager>().lastCollisionTime + gameObject.GetComponentInParent<PlayerManager>().collisionCd)
		{
			foreach (ContactPoint contact in collision.contacts) 
			{
				//Debug.Log("contact");
			
				if (contact.otherCollider.gameObject.layer == LayerMask.NameToLayer("Player")) 
				{
					//	Debug.Log("Boom");
					Explosion(contact.point,betweenPlayerExpSpeed,betweenPlayerExpRadius,"Player","Enemy",expDamage);
					source.PlayOneShot(ExplosionSound,0.75f);
					//	StartCoroutine(slowTime(0.7f,0.25f));


				}
				gameObject.GetComponentInParent<PlayerManager>().lastCollisionTime = Time.time;
			}
		}
	}

	protected override void  Reset()
	{
		transform.position = new Vector3 (transform.position.x, PlayerCenterMassElevation, transform.position.z);
		transform.rotation = Quaternion.identity;
		GetComponent<Rigidbody> ().angularDrag = PlayerNormalAngularDrag;
	}

	void playAnimation(string animType)
	{

	}

	void Attack ()
	{
		if (Time.time > lastAttackTime + attackCd) 
		{
			Collider closestEnemyCollider = getClosestAgentInCone(new string[]{"Player","Enemy"},grabRange,grabAngle);
			if (closestEnemyCollider != null)
			{
				playAnimation("Attack");

		

				closestEnemyCollider.gameObject.GetComponent<HealthManager>().TakeDamage(basicAttackDamage);
				closestEnemyCollider.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(closestEnemyCollider.transform.position - transform.position)*basicAttackForce);


				lastAttackTime = Time.time;
			}
		}
	}



	void Grab (){

		Collider closestAgentCollider = getClosestAgentInCone (new string[2]{"Enemy", "Player"},grabRange,grabAngle);
		if (closestAgentCollider != null)
		{
			isGrabbing = true;
			grabbedAgent = closestAgentCollider.gameObject;
			grabbedAgent.GetComponent<AgentController>().IsNotConstrained = true;


			grabbingJoint = gameObject.AddComponent<FixedJoint>();
			grabbedAgent.transform.position = transform.position + 2f*transform.forward + 2f*transform.up;
			//grabbedAgent.transform.Translate(new Vector3(0, 1f, 0), Space.World);
			grabbingJoint.connectedBody = grabbedAgent.GetComponent<Rigidbody>();
		} 

	}


	void releaseGrab()
	{
		isGrabbing = false;
		Rigidbody enemyRigidbody = grabbingJoint.connectedBody;
		Destroy (grabbingJoint);
		enemyRigidbody.AddForce ((transform.forward + transform.up*0.2f)*throwSpeed, ForceMode.VelocityChange);
		enemyRigidbody.angularDrag = 0;
		enemyRigidbody.constraints = RigidbodyConstraints.None;
		enemyRigidbody.gameObject.GetComponent<AgentController>().justThrown = true;
		//enemyRigidbody.gameObject.GetComponent<AgentController>().lastReleasedTime = Time.time;
		StartCoroutine(ReleaseConstraints(enemyRigidbody.gameObject));
	}




	void Update(){
		if (IsNotConstrained == true)
		{
						
		} else
		{
			
		
			if (isGrabbing) {
				Debug.DrawRay(transform.position, transform.forward*5);
				//if (Input.GetKeyUp (keybinds[4]) == true) {
				if (gameObject.name == "Player1" && Input.GetAxisRaw ("Joy 1 A") == 0)
				{
					releaseGrab();
				}
				else if (gameObject.name == "Player2" && Input.GetAxisRaw ("Joy 2 A") == 0)
				{
					releaseGrab();
				}

			} 
			else {
				/*if ( Input.GetKeyDown(keybinds[4]) == true )
				{
					Grab ();
				}*/
				if (gameObject.name == "Player1" && Input.GetAxisRaw ("Joy 1 A") == 1)
				{
					Grab ();
				}
				else if (gameObject.name == "Player2" && Input.GetAxisRaw ("Joy 2 A") == 1)
				{
					Grab ();
				}
				/*if (gameObject.name == "Player1" && Input.GetAxisRaw ("Joy 1 A") == 1)
				{
					Grab ();
				}
				else if (gameObject.name == "Player2" && Input.GetAxisRaw ("Joy 2 A") == 1) 
				{
					Grab ();
				}*/

			}
			if (Input.GetKeyDown (keybinds [5]) == true) 
			{
				Shoot();
				
			}

			// Attack

			if (gameObject.name == "Player1" && Input.GetAxisRaw ("Joy 1 B") == 1)
			{
				Attack();
			}
			else if (gameObject.name == "Player2" && Input.GetAxisRaw ("Joy 2 B") == 1)
			{
				Attack();
			}
			/*
			if (Input.GetKeyDown (keybinds [6]) == true) 
			{

			}*/
		}
	}

	void Shoot()
	{
		GameObject clone = Instantiate (projectile, transform.position + 4*Vector3.Normalize(transform.position - GameObject.Find("Player2").transform.position),Quaternion.identity) as GameObject;
		Rigidbody clonesrigid = clone.GetComponent<Rigidbody>();
		clone.name = "Bullet";
		clonesrigid.AddForce(Vector3.Normalize(transform.position - GameObject.Find("Player2").transform.position)*50, ForceMode.VelocityChange);
	}

	void MoveRelative ()
	{
		float forward = 0f;
		float rotate = 0f;
		if (Input.GetKey (keybinds [2]) == true) {
			forward = 1f;
		}
		if (Input.GetKey (keybinds [3]) == true) {
			forward = -1f;
		}
		if (Input.GetKey (keybinds [0]) == true) {
			rotate = 1f;
		}
		if (Input.GetKey (keybinds [1]) == true) {
			rotate = -1f;
		}
		transform.Rotate(new Vector3 (0,1,0)*turnSpeed*rotate);
		if (forward != 0f) {
			GetComponent<Rigidbody>().AddForce(transform.forward*forward * speed * Time.deltaTime*1000);
		}
	}

	void MoveAbsolute ()
	{
		float forward = 0f;
		float rotate = 0f;
		/*if (Input.GetKey (keybinds [2]) == true) {
			forward = 1f;
		}
		if (Input.GetKey (keybinds [3]) == true) {
			forward = -1f;
		}
		if (Input.GetKey (keybinds [0]) == true) {
			rotate = 1f;
		}
		if (Input.GetKey (keybinds [1]) == true) {
			rotate = -1f;
		}*/

		if (gameObject.name == "Player1")
		{
			forward = Input.GetAxisRaw ("Joy 1 Y");
			rotate = Input.GetAxisRaw ("Joy 1 X");
		}
		else if (gameObject.name == "Player2") 
		{
			forward = Input.GetAxisRaw ("Joy 2 Y");
			rotate = Input.GetAxisRaw ("Joy 2 X");
		}
		//shooting when connected to another player
		
		//transform.LookAt (Vector3.Lerp(transform.position + transform.forward, transform.position + absDirection, 60 / Time.deltaTime));

		if(forward != 0f || rotate != 0f)
		{
			//var relativePoint = transform.InverseTransformPoint(otherTransform.position); if (relativePoint.x < 0.0) print ("Object is to the left"); else if (relativePoint.x > 0.0) print ("Object is to the right"); else print ("Object is directly ahead"); }
			Vector3 relativePoint;		
			relativePoint = transform.InverseTransformPoint(transform.position + new Vector3 (rotate, 0, forward));
			if (relativePoint.x != 0)
			{
				GetComponent<Rigidbody>().AddTorque(Vector3.up*relativePoint.x/Mathf.Abs(relativePoint.x) * turnSpeed * Time.deltaTime*Vector3.Angle(transform.forward, new Vector3(rotate,0,forward)), ForceMode.VelocityChange);
				//GetComponent<Rigidbody>().AddTorque(Vector3.up*relativePoint.x/Mathf.Abs(relativePoint.x)*turnSpeed , ForceMode.VelocityChange);
			}
			if (GetComponent<Rigidbody>().velocity.magnitude <= maxSpeed)
			{
				GetComponent<Rigidbody>().AddForce(new Vector3(rotate,0,forward)*speed*Time.deltaTime,ForceMode.VelocityChange);
			}
			else
			{
				GetComponent<Rigidbody>().velocity = GetComponent<Rigidbody>().velocity.normalized*maxSpeed;
			}
			//}

//			Vector3 absDirection = Vector3.Normalize(new Vector3 (rotate, 0, forward));
//			transform.LookAt (transform.position + absDirection);
//			GetComponent<Rigidbody>().AddForce(transform.forward * speed * Time.deltaTime*1000);
			//GetComponent<Rigidbody>().AddForce(Vector3.back * speed * Time.deltaTime*1000);
//			GetComponent<Rigidbody>().AddForce(Vector3.Normalize(forward * speed * Time.deltaTime*1000);
		}

	}

	void FixedUpdate()
	{
		if (IsNotConstrained == false) {
			MoveAbsolute ();
		}

		//GetComponent<Rigidbody>().

	}

}
