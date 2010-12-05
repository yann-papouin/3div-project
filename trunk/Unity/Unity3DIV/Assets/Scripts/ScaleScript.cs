using UnityEngine;
using System.Collections;

public class ScaleScript : MonoBehaviour {
	public GameObject selectedObject;
	public float scaleStep = 0.001f;
	
	public GameObject scaleVisualObjectPrefab;
	public GameObject clone;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void ScaleXBigger(){
		selectedObject.transform.localScale = selectedObject.transform.localScale + new Vector3 (scaleStep,0,0);
	}
	
	public void ScaleYBigger(){
		selectedObject.transform.localScale = selectedObject.transform.localScale + new Vector3 (0,scaleStep,0);
	}
	
	public void ScaleZBigger(){
		selectedObject.transform.localScale = selectedObject.transform.localScale + new Vector3 (0,0,scaleStep);
	}
	
		
	public void ScaleXSmaller(){
		selectedObject.transform.localScale = selectedObject.transform.localScale + new Vector3 (-scaleStep,0,0);
	}
	
	public void ScaleYSmaller(){
		selectedObject.transform.localScale = selectedObject.transform.localScale + new Vector3 (0,-scaleStep,0);
	}
	
	public void ScaleZSmaller(){
		selectedObject.transform.localScale = selectedObject.transform.localScale + new Vector3 (0,0,-scaleStep);
	}
}
