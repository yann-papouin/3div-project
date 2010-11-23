using UnityEngine;
using System.Collections;

public class KeyBoardController : MonoBehaviour {

	public GameObject playerCam; 
	public float moveStep = 0.5f;
	public float rotateStep = 2.0f;
	
	
	//Similar to wiimote texture
	public Texture2D cursorImage;
	
	//Gui elementen op muis/wiimote
	public GUITexture baseGuiTexture;
	private GUITexture screenpointer;
	
	//scripts
	private RayCastScript raycastscript;
	private RotateScript rotateScript;
	private ScaleScript scaleScript;
	
	//last object selected
	private GameObject lastGameObjectHit;
	// Use this for initialization
	void Start () {
		//Set ref script
		raycastscript = gameObject.GetComponent("RayCastScript") as RayCastScript;
		rotateScript = gameObject.GetComponent("RotateScript") as RotateScript;
		scaleScript = gameObject.GetComponent("ScaleScript") as ScaleScript;
		
		//Turn off mouse pointer and set the cursorImage
		screenpointer = (GUITexture)Instantiate(baseGuiTexture);
		Screen.showCursor = false;
		screenpointer.texture = cursorImage;
		screenpointer.color = Color.red;
		screenpointer.pixelInset = new Rect(10,10,10,10);
		screenpointer.transform.localScale -= new Vector3(1, 1, 0);
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
		if (Input.GetKey ("t")){
			TestScript script = (TestScript) GameObject.Find("InputController").GetComponent("TestScript");
			script.test();
		}
		
		/*if (Input.GetKey ("o"))
			rotateCameraUp();
		if (Input.GetKey ("l"))
			rotateCameraDown();*/
		if (Input.GetButton("Fire1")){
			lastGameObjectHit = raycastscript.getTargetObjects(Input.mousePosition, playerCam.camera);
			rotateScript.selectedObject = lastGameObjectHit;
			scaleScript.selectedObject = lastGameObjectHit;
		}
		//rotate
		if (Input.GetKey("-"))
			rotateScript.RotateLeft();
		if (Input.GetKey("="))
			rotateScript.RotateRight();
		//scale
		if (Input.GetKey("["))
			scaleScript.ScaleXSmaller();
		if (Input.GetKey("]"))
			scaleScript.ScaleXBigger();
				
		
		//Set the gui shizzle
		Vector3 mousePos= Input.mousePosition;
		float mouseX = mousePos.x/Screen.width;
		float mouseY = mousePos.y/Screen.height;
		//Debug.Log(mouseX + "\t" + mouseY);
		//screenpointer.transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
		screenpointer.transform.position = new Vector3(mouseX, mouseY, 0);
		//screenpointer.transform.position = mouseY;
		//Rect cursloc = new Rect(mousePos.x, Screen.height - mousePos.y, cursorImage.width, cursorImage.height);
		//GUI.Label(cursloc, cursorImage);
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
	
	private void rotateCameraUp()
	{
		Debug.Log("Rotate camera up");
		
		playerCam.transform.Rotate(rotateStep,0,0);
	}
	
		private void rotateCameraDown()
	{
		Debug.Log("Rotate camera down");
		
		playerCam.transform.Rotate(-rotateStep,0,0);
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
