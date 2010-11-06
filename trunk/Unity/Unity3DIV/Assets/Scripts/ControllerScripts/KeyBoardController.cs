using UnityEngine;
using System.Collections;

public class KeyBoardController : MonoBehaviour {

	public GameObject playerCam; 
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis("Horizontal") < 0 )
			moveCameraLeft();
		else if (Input.GetAxis("Horizontal") > 0)
			moveCameraRight();
		
		if (Input.GetAxis("Vertical") > 0)
			zoomCameraOut();
	}
	
	//COPYPASTA
	private void moveCameraLeft()
	{
		Debug.Log("Move camera links");
	}
	private void moveCameraRight()
	{
		Debug.Log("Move camera rechts");
	}
	private void zoomCameraOut() //Jens heeft dit al gedaan in testscene. Code in comments
	{
		Debug.Log("Zoom camera uit/Swap camera");
		/*
			originalCam.camera.enabled = false;
			camToSwitch.camera.enabled = true;
		*/
	}
}
