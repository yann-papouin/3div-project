using UnityEngine;
using System.Collections;

public class KeyBoardController : MonoBehaviour {

	public GameObject playerCam; 
	public float moveStep = 0.5f;
	public float rotateStep = 2.0f;
	
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
		
		if (Input.GetKey ("left"))
			moveCameraLeft();
		if (Input.GetKey ("right"))
			moveCameraRight();
		if (Input.GetKey ("up"))
			moveCameraForward();
		if (Input.GetKey ("down"))
			moveCameraBackward();
		if (Input.GetKey ("g"))
			rotateCameraLeft();
		if (Input.GetKey ("h"))
			rotateCameraRight();
	}
	
	//COPYPASTA
	private void moveCameraLeft()
	{
		Debug.Log("Move camera left");	
		
		Vector3 cameraRelative = playerCam.transform.TransformDirection (-moveStep,0,0);		
		cameraRelative.y = 0;
		playerCam.transform.localPosition += cameraRelative;
	}
	
	private void moveCameraRight()
	{
		Debug.Log("Move camera right");
		
		Vector3 cameraRelative = playerCam.transform.TransformDirection (moveStep,0,0);
		cameraRelative.y = 0;		
		playerCam.transform.localPosition += cameraRelative;
	}
	
	private void moveCameraForward()
	{
		Debug.Log("Move camera forward");
		
		Vector3 cameraRelative = playerCam.transform.TransformDirection (0,0,moveStep);	
		cameraRelative.y = 0;
		playerCam.transform.localPosition += cameraRelative;
	}
	
	private void moveCameraBackward()
	{
		Debug.Log("Move camera backward");
		
		Vector3 cameraRelative = playerCam.transform.TransformDirection (0,0,-moveStep);	
		cameraRelative.y = 0;		
		playerCam.transform.localPosition += cameraRelative;
	}
	
	private void rotateCameraLeft()
	{
		Debug.Log("Rotate camera left");
		
		playerCam.transform.Rotate(0,-rotateStep,0);
	}
	
	private void rotateCameraRight()
	{
		Debug.Log("Rotate camera right");
		
		playerCam.transform.Rotate(0,rotateStep,0);
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
