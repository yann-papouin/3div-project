using UnityEngine;
using System.Collections;

public class CameraViewScript : MonoBehaviour {
	private ArrayList vasteCameraLokaties;
	private ArrayList vasteCameraLookAts;
	private bool wachtOpKeyUp = false;
	
	public Vector3 eigenLokatie = new Vector3(0,0,0);
	public Quaternion eigenLookAt = Quaternion.identity;
	
	public Vector3 uitZoomLocatie = new Vector3(11, 60, 10);
	public Vector3 uitZoomLookAt = new Vector3(11, 10, 10);
	
	private int index = -1;
	
	// Use this for initialization
	void Start () {
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
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public Vector3 getNextCameraLokatie(){
		if(wachtOpKeyUp == false){
			index = (index+1) % (vasteCameraLokaties.Count);
			wachtOpKeyUp = true;
		}
		return (Vector3)vasteCameraLokaties[index];
		
	}
	
	public Vector3 getNextCameraLookAt(){
		return (Vector3)vasteCameraLookAts[index];
	}
	
	public Vector3 getVorigCameraLokatie(){
		if(wachtOpKeyUp == false){
			index = index -1;
			if (index < 0){
				index = vasteCameraLokaties.Count - 1;
			}
			wachtOpKeyUp = true;
		}
		return (Vector3)vasteCameraLokaties[index];
		
	}
	
	public Vector3 getVorigCameraLookAt(){
		return (Vector3)vasteCameraLookAts[index];
	}
	
	public void releaseLock(){
		wachtOpKeyUp = false;
	}
		
}
