using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBarManager : MonoBehaviour {

	private Vector2 fixedPos;
	private Vector2 targetScreenPos;
	private Vector2 screenSize;
	private Vector2 canvasSize;
	private Camera playerCamera;
	public GameObject targetAgent;
	private GameObject screen;
	private string str;
	private Slider slider;


	// Use this for initialization
	void Start () {
		screen = GameObject.Find ("Canvas");
		str = gameObject.name;
		//targetAgent = GameObject.Find(str = str.Remove(str.Length - 4));
		playerCamera = Camera.main;
		transform.SetParent(screen.transform, false);
		//gameObject.GetComponent<Slider> ();
		//Debug.Log (targetAgent);

	}

	// Update is called once per frame
	void Update () 
	{
		canvasSize.x = screen.GetComponent<RectTransform>().rect.width;
		canvasSize.y = screen.GetComponent<RectTransform>().rect.height;
		//Debug.Log (canvasSize);

		targetScreenPos = playerCamera.WorldToScreenPoint (targetAgent.transform.position + Vector3.up*2);
		fixedPos.y = targetScreenPos.y - canvasSize.y;
		fixedPos.x = targetScreenPos.x - canvasSize.x;

		GetComponent<RectTransform> ().anchoredPosition = fixedPos;





//		fixedPos.x = (targetScreenPos.x / screenSize.x) * canvasSize.x;
//		fixedPos.y = -(canvasSize.y - (targetScreenPos.y / screenSize.y) * canvasSize.y);
//		GetComponent(RectTransform).anchoredPosition = fixedPos;*/
	}
}
