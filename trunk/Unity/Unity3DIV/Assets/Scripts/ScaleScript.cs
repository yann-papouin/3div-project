using UnityEngine;
using System.Collections;

public class ScaleScript : MonoBehaviour {
	public GameObject selectedObject;
	public float scaleStep = 0.001f;
	
	public GameObject scaleVisualObjectPrefab;
	public GameObject clone;
	private bool drawFeedback = false;
	private string gekozenAs = "";
	
	// Use this for initialization
	void Start () {
		clone = (GameObject) Instantiate(scaleVisualObjectPrefab, new Vector3 (0,0,0) , Quaternion.identity);
		clone.active = false;
		clone.transform.localScale = new Vector3(1,1,1);
	}
	
	// Update is called once per frame
	void Update () {
		if (drawFeedback){
			if (gekozenAs == "x"){
					
			} else if (gekozenAs == "y"){
					
			} else if (gekozenAs == "z") {
				
			}	
		}
	}
	
		public void SetDrawFeedback(bool hasDrawFeedback, string gekozenAs){
				
		if (hasDrawFeedback){
			// schaleren
			Vector3 grootte = new Vector3(1,1,1);
			float schaal;
			if (gekozenAs == "x"){
				schaal = selectedObject.renderer.bounds.size.x / clone.renderer.bounds.size.x;
				grootte.x = schaal;	
				grootte.y = 0.05f;
				grootte.z = (selectedObject.renderer.bounds.size.z / clone.renderer.bounds.size.z) / 5;			
			} else if (gekozenAs == "y"){
				schaal = selectedObject.renderer.bounds.size.y / clone.renderer.bounds.size.y;	
				grootte.x = (selectedObject.renderer.bounds.size.x / clone.renderer.bounds.size.x)/5;
				grootte.y = schaal;	
				grootte.z = (selectedObject.renderer.bounds.size.z / clone.renderer.bounds.size.z) / 5;		
			} else if (gekozenAs == "z") {
				schaal = selectedObject.renderer.bounds.size.z / clone.renderer.bounds.size.z;	
				grootte.x =	(selectedObject.renderer.bounds.size.x / clone.renderer.bounds.size.x)/5;
				grootte.y = 0.05f;
				grootte.z =  schaal;
			}	
			clone.transform.localScale = grootte;
			
			
			//roteren
			clone.transform.rotation = selectedObject.transform.rotation;
			
			//positioneren
			clone.transform.position = selectedObject.transform.position;
					
		} else {
			clone.transform.localScale = new Vector3(1,1,1);	
		}
		
		clone.active = hasDrawFeedback;
		drawFeedback = hasDrawFeedback;

	
	}
	
	public void ScaleXBigger(){
		selectedObject.transform.localScale += new Vector3 (scaleStep,0,0);
		clone.transform.localScale += new Vector3 (scaleStep,0,0);

	}
	
	public void ScaleYBigger(){
		selectedObject.transform.localScale += new Vector3 (0,scaleStep,0);
		clone.transform.localScale += new Vector3 (0,scaleStep,0);
	}
	
	public void ScaleZBigger(){
		selectedObject.transform.localScale += new Vector3 (0,0,scaleStep);
		clone.transform.localScale += new Vector3 (0,0,scaleStep);
	}
	
		
	public void ScaleXSmaller(){
		selectedObject.transform.localScale += new Vector3 (-scaleStep,0,0);
		clone.transform.localScale += new Vector3 (-scaleStep,0,0);
	}
	
	public void ScaleYSmaller(){
		selectedObject.transform.localScale += new Vector3 (0,-scaleStep,0);
		clone.transform.localScale += new Vector3 (0,-scaleStep,0);
	}
	
	public void ScaleZSmaller(){
		selectedObject.transform.localScale += new Vector3 (0,0,-scaleStep);
		clone.transform.localScale += new Vector3 (0,0,-scaleStep);
	}
}
