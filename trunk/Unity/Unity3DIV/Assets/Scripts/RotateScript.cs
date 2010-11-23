using UnityEngine;
using System.Collections;

public class RotateScript : MonoBehaviour {
	public GameObject selectedObject;
	//hoeveel graden/step
	public float rotateStep = 0.5f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	//links roteren
	public void RotateLeft(){
		//selectedObject.transform.rotation = Quaternion.AngleAxis(-rotateStep, Vector3.up);
		selectedObject.transform.Rotate(Vector3.up, -rotateStep, Space.World);
		
	}
	
	//rechts roteren
	public void RotateRight(){
		//selectedObject.transform.rotation = Quaternion.AngleAxis(rotateStep, Vector3.up);
		selectedObject.transform.Rotate(Vector3.up, rotateStep, Space.World);

	}
}
