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
	private StackScript stackScript;
	private MoveScript moveScript;
	private SmoothCameraScript smoothCameraScript;
	
	
	//last object selected
	private GameObject lastGameObjectHit;
	
	// Use this for initialization
	void Start () {
		//Set ref script
		raycastscript = gameObject.GetComponent("RayCastScript") as RayCastScript;
		rotateScript = gameObject.GetComponent("RotateScript") as RotateScript;
		scaleScript = gameObject.GetComponent("ScaleScript") as ScaleScript;
		stackScript = gameObject.GetComponent("StackScript") as StackScript;
		moveScript = gameObject.GetComponent("MoveScript") as MoveScript;
		smoothCameraScript = gameObject.GetComponent("SmoothCameraScript") as SmoothCameraScript;


		
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
		updateNavigation();
		updateStackingManipulation();
				
		if (Input.GetButton("Fire1")){
			lastGameObjectHit = raycastscript.getTargetObjects(Input.mousePosition, playerCam.camera);
			if (lastGameObjectHit != rotateScript.clone){
				rotateScript.selectedObject = lastGameObjectHit;
				rotateScript.SetDrawFeedback(true);
				
				scaleScript.selectedObject = lastGameObjectHit;
				scaleScript.SetDrawFeedback(true);	

			}
			
		}
		//rotate
		if (Input.GetKey("-"))
			rotateScript.RotateLeft();
		if (Input.GetKey("="))
			rotateScript.RotateRight();
		//scale
		if (Input.GetKey("["))
			scaleScript.ScaleKleiner();
		if (Input.GetKey("]"))
			scaleScript.ScaleGroter();
		if (Input.GetKey("p"))
			scaleScript.volgendeAs();
		if (Input.GetKeyUp("p"))
			scaleScript.releaseLock();
			
		//camera
		if (Input.GetKeyUp (";")){
			smoothCameraScript.getNextCameraLokatie();
		}

		if (Input.GetKeyUp ("'")){
			smoothCameraScript.getVorigCameraLokatie();
		}
		
		if (Input.GetKey("/")){
			smoothCameraScript.GaNaarVorigePositie();			
		}
		
		//camera uitzoomen naar top view
		if (Input.GetKey(".")){
			smoothCameraScript.GaNaarTopView();	
		}
		
			
		//Set the gui shizzle
		Vector3 mousePos= Input.mousePosition;
		float mouseX = mousePos.x/Screen.width;
		float mouseY = mousePos.y/Screen.height;
		//Debug.Log(mouseX + "\t" + mouseY);
		//screenpointer.transform.position = new Vector3(Screen.width/2, Screen.height/2, 0);
		screenpointer.transform.position = new Vector3(mouseX, mouseY, 0);
		//screenpointer.transform.position = mouseY;
		Rect cursloc = new Rect(mousePos.x, Screen.height - mousePos.y, cursorImage.width, cursorImage.height);
		//GUI.Label(cursloc, cursorImage);
	}
	
	private void updateNavigation(){
		if (Input.GetKey("left") && !stackScript.isActive)
			moveCameraLeft();
		if (Input.GetKey("right") && !stackScript.isActive)
			moveCameraRight();
		if (Input.GetKey("up") && !stackScript.isActive)
			moveCameraForward();
		if (Input.GetKey("down") && !stackScript.isActive)
			moveCameraBackward();
		if (Input.GetKey("g") && !stackScript.isActive)
			rotateCameraLeft();
		if (Input.GetKey("h") && !stackScript.isActive)
			rotateCameraRight();
	}
	
	private void updateStackingManipulation(){
		if (Input.GetKeyUp ("t")){
				TestScript script = (TestScript) GameObject.Find("InputController").GetComponent("TestScript");
				script.testStack(); // place an object -> skip selection step
		}	
			
		if(stackScript.isActive){
			if(stackScript.gridModus){
				if (Input.GetKeyUp("left"))
					stackScript.goToNextAvailablePositionLeft();	
				if (Input.GetKeyUp("right"))
					stackScript.goToNextAvailablePositionRight();
				if (Input.GetKeyUp("up"))
					stackScript.goToNextAvailablePositionTop();
				if (Input.GetKeyUp("down"))
					stackScript.goToNextAvailablePositionDown();
			}else{
				if (Input.GetKey("left"))
					stackScript.goToLeft();	
				if (Input.GetKey("right"))
					stackScript.goToRight();
				if (Input.GetKey("up"))
					stackScript.goToTop();
				if (Input.GetKey("down"))
					stackScript.goToBottom();
			}
			
				
			if (Input.GetKeyUp ("y") && stackScript.gridModus)
				stackScript.goToNextAvailablePosition(); // scrolling  left-right, bottom-up
			if (Input.GetKeyUp ("u"))
				stackScript.goToNextPossibleStackedObject(); // scrolling between possible object
			if (Input.GetKeyUp ("i"))
				stackScript.Abort(); // abort this manipulation
			if (Input.GetKeyUp ("o"))
				stackScript.End(); // end this manipulation
		}		
	}
	
	//COPYPASTA
	private void moveCameraLeft()
	{
		Debug.Log("Move camera left");	
		
		Vector3 cameraRelative = playerCam.transform.TransformDirection (-moveStep,0,0);		
		cameraRelative.y = 0;
		playerCam.transform.localPosition += cameraRelative;
		
		smoothCameraScript.eigenLokatie = playerCam.transform.position;
		
	}
	
	private void moveCameraRight()
	{
		Debug.Log("Move camera right");
		
		Vector3 cameraRelative = playerCam.transform.TransformDirection (moveStep,0,0);
		cameraRelative.y = 0;		
		playerCam.transform.localPosition += cameraRelative;
		
		smoothCameraScript.eigenLokatie = playerCam.transform.position;

	}
	
	private void moveCameraForward()
	{
		Debug.Log("Move camera forward");
		
		Vector3 cameraRelative = playerCam.transform.TransformDirection (0,0,moveStep);	
		cameraRelative.y = 0;
		playerCam.transform.localPosition += cameraRelative;
		
		smoothCameraScript.eigenLokatie = playerCam.transform.position;

	}
	
	private void moveCameraBackward()
	{
		Debug.Log("Move camera backward");
		
		Vector3 cameraRelative = playerCam.transform.TransformDirection (0,0,-moveStep);	
		cameraRelative.y = 0;		
		playerCam.transform.localPosition += cameraRelative;
		
		smoothCameraScript.eigenLokatie = playerCam.transform.position;

	}
	
	private void rotateCameraLeft()
	{
		Debug.Log("Rotate camera left");
		
		playerCam.transform.Rotate(0,-rotateStep,0);
		
		smoothCameraScript.eigenLookAt = playerCam.transform.rotation;
	}
	
	private void rotateCameraUp()
	{
		Debug.Log("Rotate camera up");
		
		playerCam.transform.Rotate(rotateStep,0,0);
		
		smoothCameraScript.eigenLookAt = playerCam.transform.rotation;
	}
	
		private void rotateCameraDown()
	{
		Debug.Log("Rotate camera down");
		
		playerCam.transform.Rotate(-rotateStep,0,0);
		
		smoothCameraScript.eigenLookAt = playerCam.transform.rotation;
	}
	
	private void rotateCameraRight()
	{
		Debug.Log("Rotate camera right");
		
		playerCam.transform.Rotate(0,rotateStep,0);
		
		smoothCameraScript.eigenLookAt = playerCam.transform.rotation;
	}
}
