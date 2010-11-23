using UnityEngine;
using System.Collections;

public class ScaleScript : MonoBehaviour {
	public GameObject selectedObject;
	public float scaleStep = 0.1f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void ScaleXBigger(){
		//selectedObject.transform.localScale += Vector3(scaleStep,0,0);
		//selectedObject.transform.lossyScale.x = 1 + scaleStep;
	}
	
	public void ScaleYBigger(){
		//selectedObject.transform.Scale(0,scaleStep,0);
	}
	
	public void ScaleZBigger(){
		//selectedObject.transform.Scale(0,0,scaleStep);
	}
	
		
	public void ScaleXSmaller(){
		//selectedObject.transform.Scale(-scaleStep,0,0);
		//selectedObject.transform.lossyScale.x = 1 - scaleStep;
	}
	
	public void ScaleYSmaller(){
		//selectedObject.transform.Scale(0,-scaleStep,0);
	}
	
	public void ScaleZSmaller(){
		//selectedObject.transform.Scale(0,0,-scaleStep);
	}
}
