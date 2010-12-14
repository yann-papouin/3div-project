// Author: Sibrand Staessens

using UnityEngine;
using System.Collections;
using System;

/*
*	Script for moving the main camera smoothly towards a topview above a selected object.
*
*/

public class SmoothCameraScript : MonoBehaviour {
	// for interpolating to topview
	private  Vector3 cameraPositionBeforeTopView; 
	private  Quaternion cameraRotationBeforeTopView; 
		
	public float interpolTime = 3.0f;
	private Quaternion startInterpolRot, endInterpolRot;
	private Vector3 startInterpolPos, endInterpolPos;
	private float elapsedTime;
	private bool interpolToTopView;
	private ObjectScript objectToViewScript;
	
	void Start () {
		elapsedTime = 2*interpolTime;
		interpolToTopView = false;
	}
	
	void Update () {
		if(elapsedTime <= interpolTime){
			elapsedTime += Time.deltaTime;
			
			Camera.main.transform.position = Vector3.Lerp(startInterpolPos, endInterpolPos, elapsedTime/interpolTime);
			Camera.main.transform.rotation = Quaternion.Slerp(startInterpolRot, endInterpolRot, elapsedTime/interpolTime);		
		}
	}
	
	public void changeViewedObject(GameObject go){
		objectToViewScript = (ObjectScript) go.GetComponent("ObjectScript");	
	
		if(elapsedTime > interpolTime && !interpolToTopView){
			cameraPositionBeforeTopView = Camera.main.transform.position;
			cameraRotationBeforeTopView = Camera.main.transform.rotation;				
		}
		
		elapsedTime = 0.0f;
		interpolToTopView = true;
		startInterpolPos = Camera.main.transform.position;
		startInterpolRot = Camera.main.transform.rotation;
		
		Vector3 pos = objectToViewScript.transform.position;
		pos.y += objectToViewScript.topViewDistance;
		endInterpolPos = pos;			
			
		Transform rot = Camera.main.transform;
		rot.rotation = Quaternion.identity;	
		float angle = 0.0f;
		
		Vector3 rotp = new Vector3(90,0,0);
		rot.Rotate (rotp, Space.World);
		
		// make sure topDown en leftRight are correctly shown
		switch(objectToViewScript.localAxisLeftRight[0]){
			case 'X':
				angle = Vector3.Angle(rot.right, objectToViewScript.transform.right);
				break;
			case 'Y':
				angle = Vector3.Angle(rot.right, objectToViewScript.transform.up);
				break;
			case 'Z':
				angle = Vector3.Angle(rot.right, objectToViewScript.transform.forward);
				break;
		}		
				
		rotp = new Vector3(0,angle,0);
		rot.Rotate (rotp, Space.World);
		
		bool flip = false;
		switch(objectToViewScript.localAxisLeftRight[0]){
			case 'X':
				flip = Math.Abs(Vector3.Angle(rot.right, objectToViewScript.transform.right)) > 2.0f;
				break;
			case 'Y':
				flip = Math.Abs(Vector3.Angle(rot.right, objectToViewScript.transform.up)) > 2.0f;
				break;
			case 'Z':
				flip = Math.Abs(Vector3.Angle(rot.right, objectToViewScript.transform.forward)) > 2.0f;
				break;
		}
		if(flip){
			rot.Rotate (-2*rotp, Space.World);
		}
		
		endInterpolRot = rot.rotation;		
	}
	
	public void returnFromTopview(){	
		startInterpolPos = Camera.main.transform.position;
		startInterpolRot = Camera.main.transform.rotation;
		endInterpolPos = cameraPositionBeforeTopView;
		endInterpolRot = cameraRotationBeforeTopView;		
	
		interpolToTopView = false;
		elapsedTime = 0.0f;		
	}
}
