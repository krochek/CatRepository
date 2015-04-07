using UnityEngine;
using System.Collections;

public class WallBuilder : MonoBehaviour {

	public Transform brick;
	public int gridWidth=4;
	public int gridDepth=4;
	public int gridHeight=4;
	public float scaleOfCube=1f;

	void Start() {
		for (int y = 0; y < gridHeight; y=y+2) 
		{
			for (int z = 0; z < gridDepth; z=z+2) 
			{
				for (int x = 0; x < gridWidth; x=x+2) 
				{
					Instantiate (brick, new Vector3 (x*scaleOfCube/2, y*scaleOfCube/2, z*scaleOfCube/2), Quaternion.identity);
				}
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
