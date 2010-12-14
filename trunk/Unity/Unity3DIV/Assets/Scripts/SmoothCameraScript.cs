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
	
	private ArrayList vasteCameraLokaties;
	private ArrayList vasteCameraLookAts;
	private bool wachtOpKeyUp = false;
	
	public Vector3 eigenLokatie = new Vector3(0,0,0);
	public Quaternion eigenLookAt = Quaternion.identity;
	
	public Vector3 uitZoomLocatie = new Vector3(11, 60, 10);
	
	private int index = -1;
	
	void Start () {
		elapsedTime = 2*interpolTime;
		interpolToTopView = false;
		
		vasteCameraLokaties = new ArrayList();
		vasteCameraLookAts = new ArrayList();
		
		//Kamer 1 hoeken
		vasteCameraLokaties.Add(new Vector3(14, 7, 10));
		vasteCameraLookAts.Add(new Vector3(-10, 7, -4));
		
		vasteCameraLokaties.Add(new Vector3(13, 7, -10));
		vasteCameraLookAts.Add(new Vector3(-10, 7, 9));
		
		vasteCameraLokaties.Add(new Vector3(-13, 7, -10));
		vasteCameraLookAts.Add(new Vector3(14, 7, 10));
				
		vasteCameraLokaties.Add(new Vector3(-13, 7, 10));
		vasteCameraLookAts.Add(new Vector3(13, 7, -10));
		
//		//Kamer 1 midden
//		vasteCameraLokaties.Add(new Vector3(0, 7, 0));
//		vasteCameraLookAts.Add(new Vector3(-10, 7, -4));
//
//		vasteCameraLokaties.Add(new Vector3(0, 7, 0));
//		vasteCameraLookAts.Add(new Vector3(-10, 7, 9));
//		
//		vasteCameraLokaties.Add(new Vector3(0, 7, 0));
//		vasteCameraLookAts.Add(new Vector3(14, 7, 10));
//
//		vasteCameraLokaties.Add(new Vector3(0, 7, 0));
//		vasteCameraLookAts.Add(new Vector3(13, 7, -10));
		
		//Kamer 2 hoeken
		vasteCameraLokaties.Add(new Vector3(-13, 7, 14));
		vasteCameraLookAts.Add(new Vector3(32, 7, 32));
		
		vasteCameraLokaties.Add(new Vector3(-13, 7, 32));
		vasteCameraLookAts.Add(new Vector3(32, 7, 14));
		
		vasteCameraLokaties.Add(new Vector3(32, 7, 32));
		vasteCameraLookAts.Add(new Vector3(-13, 7, 14));
				
		vasteCameraLokaties.Add(new Vector3(32, 7, 14));
		vasteCameraLookAts.Add(new Vector3(-13, 7, 32));
				
		//Kamer 3 hoeken
		vasteCameraLokaties.Add(new Vector3(32, 7, 11));
		vasteCameraLookAts.Add(new Vector3(18, 7, -10));
				
		vasteCameraLokaties.Add(new Vector3(32, 7, -10));
		vasteCameraLookAts.Add(new Vector3(18, 7, 10));
		
		vasteCameraLokaties.Add(new Vector3(18, 7, -10));
		vasteCameraLookAts.Add(new Vector3(32, 7, 11));
		
		vasteCameraLokaties.Add(new Vector3(18, 7, 10));
		vasteCameraLookAts.Add(new Vector3(32, 7, -10));	
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
	
	public void getNextCameraLokatie(){
		startInterpolPos = Camera.main.transform.position;
		startInterpolRot = Camera.main.transform.rotation;
				
		index = (index+1) % (vasteCameraLokaties.Count);
		
		Transform t = Camera.main.transform;
		t.position = (Vector3)vasteCameraLokaties[index];
		t.LookAt((Vector3)vasteCameraLookAts[index]);
		
		endInterpolRot = t.rotation;

		endInterpolPos = (Vector3)vasteCameraLokaties[index];
		elapsedTime = 0.0f;	
		
	}
	

	public void getVorigCameraLokatie(){
		startInterpolPos = Camera.main.transform.position;
		startInterpolRot = Camera.main.transform.rotation;

			index = index -1;
			if (index < 0){
				index = vasteCameraLokaties.Count - 1;
			}
			
		Transform t = Camera.main.transform;
		t.position = (Vector3)vasteCameraLokaties[index];
		t.LookAt((Vector3)vasteCameraLookAts[index]);
		
		endInterpolRot = t.rotation;

		endInterpolPos = (Vector3)vasteCameraLokaties[index];
		elapsedTime = 0.0f;	
		
	}
	
	public void GaNaarVorigePositie(){
		startInterpolPos = Camera.main.transform.position;
		startInterpolRot = Camera.main.transform.rotation;

		endInterpolPos = eigenLokatie;
		endInterpolRot = eigenLookAt;
		elapsedTime = 0.0f;	
	}
	
	public void GaNaarTopView(){
		startInterpolPos = Camera.main.transform.position;
		startInterpolRot = Camera.main.transform.rotation;
		
		Transform rot = Camera.main.transform;
		rot.rotation = Quaternion.identity;	
		float angle = 0.0f;
		
		Vector3 rotp = new Vector3(90,0,0);
		rot.Rotate (rotp, Space.World);

		endInterpolRot = rot.rotation;		
		endInterpolPos = uitZoomLocatie;

		
		elapsedTime = 0.0f;	
	}
	
}
