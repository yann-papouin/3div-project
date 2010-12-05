using UnityEngine;
using System.Collections;

public class RotateScript : MonoBehaviour {
	public GameObject selectedObject;
	//hoeveel graden/step
	public float rotateStep = 0.5f;
	
	public GameObject rotateVisualObjectPrefab;
	public GameObject clone;
	
	public bool drawFeedback;
	
	// Use this for initialization
	void Start () {
		clone = (GameObject) Instantiate(rotateVisualObjectPrefab, new Vector3 (0,0,0) , Quaternion.identity);
		clone.active = false;
		clone.transform.localScale = new Vector3(1,1,1);	

	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void SetDrawFeedback(bool hasDrawFeedback){
		drawFeedback = hasDrawFeedback;
		
		if (hasDrawFeedback){
			//schaleren
			Vector3 grootte;
			float schaal;
			if (selectedObject.renderer.bounds.size.x > selectedObject.renderer.bounds.size.z){
				schaal = selectedObject.renderer.bounds.size.x / clone.renderer.bounds.size.x;
			} else {
				schaal = selectedObject.renderer.bounds.size.z / clone.renderer.bounds.size.z;
			}
			grootte.x = schaal;
			grootte.z = schaal;	
			grootte.y = 0.05f;
	
			clone.transform.localScale = grootte;
	
				
		//positioneren
		Vector3 positie = selectedObject.transform.position;
		positie.y = 0.55f;
		clone.transform.position = positie;
		} else {
			clone.transform.localScale = new Vector3(1,1,1);	
		}
		
		clone.active = hasDrawFeedback;
	
	}
		
	
	//links roteren
	public void RotateLeft(){
		selectedObject.transform.Rotate(Vector3.up, -rotateStep, Space.World);
		clone.transform.Rotate(Vector3.up, -rotateStep, Space.World);

	}
	
	//rechts roteren
	public void RotateRight(){
		selectedObject.transform.Rotate(Vector3.up, rotateStep, Space.World);
		clone.transform.Rotate(Vector3.up, rotateStep, Space.World);
	}
	
	public void DrawcirkelRondObject(){
			
	}
}
