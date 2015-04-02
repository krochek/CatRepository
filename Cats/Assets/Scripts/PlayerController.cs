using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour 
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
	public int count;
	public float turnSpeed;
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
	//grabbing helpers
	public float centerMassElevation = 1f;
	private RigidbodyConstraints normalConstraints;
	public float normalAngularDrag = 20f;

	//for damage modelling and healthbar
	public GameObject healthBar;
	public GameObject hbar;


	private int enemiesLayer;
	private int playersLayer;




	public int basicAttackDamage = 20;

	private KeyCode[] keybinds;

	//Rigidbody playerR;
	//Vector3 movement;
	private bool isGrabbed;
	
	public bool IsGrabbed 
	{
		get { return isGrabbed; }
		set {
			isGrabbed = value;
			if (value)
			{
				GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
			}
			else
			{
				transform.position = new Vector3(transform.position.x, centerMassElevation, transform.position.z);
				GetComponent<Rigidbody>().constraints = normalConstraints;
			}
		}
	}

	void Awake()
	{
		hbar = Instantiate (healthBar);
		hbar.name = gameObject.name + " bar";
		hbar.GetComponent<HealthBarManager> ().targetAgent = gameObject;

		isGrabbed = false;
		normalConstraints = GetComponent<Rigidbody>().constraints;
		enemiesLayer = LayerMask.NameToLayer("Enemies");
		playersLayer = LayerMask.NameToLayer("Players");
	}

	void Start (){

		lastAttackTime = attackCd;

		switch (gameObject.name) 
		{
		case "Player1":
			//bindToPlayer();
			keybinds = new KeyCode[7]{KeyCode.D , KeyCode.A , KeyCode.W, KeyCode.S, KeyCode.V, KeyCode.B, KeyCode.C};
			break;

		case "Player2":
			keybinds = new KeyCode[7]{KeyCode.RightArrow , KeyCode.LeftArrow , KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.Keypad0,KeyCode.KeypadEnter, KeyCode.KeypadPeriod};
			break;
		}

		count = 0;
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
		//fixed12.connectedBody = GameObject.Find ("Player2").GetComponent<Rigidbody> ();

		//HingeJoint fixedP1P2 = gameObject.AddComponent<HingeJoint> ();
		//fixedP1P2.connectedBody = GameObject.Find ("Player2").GetComponent<Rigidbody>();

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

	void playAnimation(string animType)
	{

	}

	void Attack ()
	{
		if (Time.time > lastAttackTime + attackCd) 
		{
			playAnimation("Attack");

	
			Collider closestEnemyCollider = getAgentsInRange(new string[]{"Players","Enemies"});
			closestEnemyCollider.gameObject.GetComponent<HealthManager>().TakeDamage(basicAttackDamage);
			closestEnemyCollider.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.Normalize(closestEnemyCollider.transform.position - transform.position)*basicAttackForce);


			lastAttackTime = Time.time;
		}
	}

	void Grab (){

		Collider closestAgentCollider = getAgentsInRange (new string[2]{"Enemies", "Players"});
		if (closestAgentCollider) 
		{
			isGrabbing = true;
			grabbedAgent = closestAgentCollider.gameObject;

			if(grabbedAgent.layer == LayerMask.NameToLayer("Enemies"))
			{
				grabbedAgent.GetComponent<EnemyController>().IsGrabbed = true;
			}
			else if(grabbedAgent.layer == LayerMask.NameToLayer("Players"))
			{
				grabbedAgent.GetComponent<PlayerController>().IsGrabbed = true;
			}
			grabbingJoint = gameObject.AddComponent<FixedJoint>();
			grabbedAgent.transform.Translate(new Vector3(0, 1f, 0), Space.World);
			grabbingJoint.connectedBody = grabbedAgent.GetComponent<Rigidbody>();
		} 
		//LayerMask.NameToLayer("Enemies")
	}

	IEnumerator ReleaseConstraints (GameObject agentToRelease)
	{
		yield return new WaitForSeconds(3);

		if(agentToRelease.layer == LayerMask.NameToLayer("Enemies"))
		{
			agentToRelease.GetComponent<EnemyController>().IsGrabbed = false;
			agentToRelease.GetComponent<EnemyController>().Reset();
		}
		else if(agentToRelease.layer == LayerMask.NameToLayer("Players"))
		{
			agentToRelease.GetComponent<PlayerController>().IsGrabbed = false;
			agentToRelease.GetComponent<PlayerController>().Reset();

		}
	}

	void Reset()
	{
		transform.position = new Vector3 (transform.position.x, centerMassElevation, transform.position.z);
		transform.rotation = Quaternion.identity;
		GetComponent<Rigidbody> ().angularDrag = normalAngularDrag;
	}



	void Update(){
		if (IsGrabbed == true)
		{
			//GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
			
		} else
		{
			
		//	GetComponent<Rigidbody>().constraints = normalConstraints;
		//	transform.position = new Vector3 (transform.position.x, centerMassElevation, transform.position.z);


		
		
			if (isGrabbing) {
				if (Input.GetKeyUp (keybinds[4]) == true) {
					isGrabbing = false;
					Rigidbody enemyRigidbody = grabbingJoint.connectedBody;
					Destroy (grabbingJoint);
					enemyRigidbody.AddForce ((transform.forward + transform.up*0.2f)*throwSpeed, ForceMode.VelocityChange);
					enemyRigidbody.angularDrag = 0;
					StartCoroutine(ReleaseConstraints(grabbedAgent));
				
				}
				//	enemyRigidbody.AddForce(Vector3.transform.rotation.y, Space.World);
			} 
			else {
				if ( Input.GetKeyDown(keybinds[4]) == true )
				{
					Grab ();
				}
			}
			if (Input.GetKeyDown (keybinds [5]) == true) 
			{
				Shoot();
				
			}
			if (Input.GetKeyDown (keybinds [6]) == true) 
			{
				Attack();
			}
		}
	}

	void Shoot()
	{
		GameObject clone = Instantiate (projectile, transform.position + 4*Vector3.Normalize(transform.position - GameObject.Find("Player2").transform.position),Quaternion.identity) as GameObject;
		Rigidbody clonesrigid = clone.GetComponent<Rigidbody>();
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
		//shooting when connected to another player
		
		
		
		//transform.position += transform.forward * forward * Time.deltaTime * speed;
		//transform.RotateAround (transform.position, new Vector3 (0, 1, 0), rotate*turnspeed* Time.deltaTime);
		transform.Rotate(new Vector3 (0,1,0)*turnSpeed*rotate);
		
		//float moveHorizontal = Input.GetAxis ("Horizontal");
		//float moveVertical = Input.GetAxis ("Vertical");		
		//Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical); 
		if (forward != 0f) {
			GetComponent<Rigidbody>().AddForce(transform.forward*forward * speed * Time.deltaTime*1000);
		}
	}

	void MoveAbsolute ()
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
		//shooting when connected to another player
		
		//transform.LookAt (Vector3.Lerp(transform.position + transform.forward, transform.position + absDirection, 60 / Time.deltaTime));

		if(forward != 0f || rotate != 0f)
		{
			//var relativePoint = transform.InverseTransformPoint(otherTransform.position); if (relativePoint.x < 0.0) print ("Object is to the left"); else if (relativePoint.x > 0.0) print ("Object is to the right"); else print ("Object is directly ahead"); }
			Vector3 relativePoint;		
			relativePoint = transform.InverseTransformPoint(transform.position + new Vector3 (rotate, 0, forward));
			GetComponent<Rigidbody>().AddTorque(Vector3.up*relativePoint.x/Mathf.Abs(relativePoint.x) * turnSpeed * Time.deltaTime*1000*Vector3.Angle(transform.forward, new Vector3(rotate,0,forward)));
			GetComponent<Rigidbody>().AddForce(transform.forward * speed * Time.deltaTime*1000);


//			Vector3 absDirection = Vector3.Normalize(new Vector3 (rotate, 0, forward));
//			transform.LookAt (transform.position + absDirection);
//			GetComponent<Rigidbody>().AddForce(transform.forward * speed * Time.deltaTime*1000);
			//GetComponent<Rigidbody>().AddForce(Vector3.back * speed * Time.deltaTime*1000);
//			GetComponent<Rigidbody>().AddForce(Vector3.Normalize(forward * speed * Time.deltaTime*1000);
		}

	}

	void FixedUpdate()
	{
		if (IsGrabbed == false) {
			MoveAbsolute ();
		}

		//GetComponent<Rigidbody>().

	}
	void OnTriggerEnter(Collider other) {
		//Destroy(other.gameObject);
		if (other.gameObject.tag == "PickUp") {
			other.gameObject.SetActive(false);
			count = count +1;
		}


	}
}
