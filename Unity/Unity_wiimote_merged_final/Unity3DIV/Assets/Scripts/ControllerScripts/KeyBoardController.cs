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
	private VerwijderScript verwijderScript;
	private SelectieScript selectieScript;
	
	private bool moveToSelectedRoom = false;
	
	
	private bool changeParent = false;	
	private bool returnToCamShowAfterManipulation = false;
	//last object selected
	private GameObject selectieScript.lastGameObjectHit, lastHooveredGameObject;
	// Navigation + selection, Camera show, Choosing manipulation, Moving + Parent Changing, Deleting, Rotating, Scaling, Stacking
	private enum Modi{NAV_SEL, CAM_SHO, CHO_MAN, DEL, MOV, ROT, SCA, STA}; 
	private Modi modus;
	
	// Use this for initialization
	void Start () {
		//selectieScript.lastGameObjectHit = null;
		lastHooveredGameObject = null;
	
		//Set ref script
		raycastscript = gameObject.GetComponent("RayCastScript") as RayCastScript;
		rotateScript = gameObject.GetComponent("RotateScript") as RotateScript;
		scaleScript = gameObject.GetComponent("ScaleScript") as ScaleScript;
		stackScript = gameObject.GetComponent("StackScript") as StackScript;
		moveScript = gameObject.GetComponent("MoveScript") as MoveScript;
		smoothCameraScript = gameObject.GetComponent("SmoothCameraScript") as SmoothCameraScript;
		verwijderScript = gameObject.GetComponent("VerwijderScript") as VerwijderScript;
		selectieScript = gameObject.GetComponent("SelectieScript") as SelectieScript;
		selectieScript.setSelectionmodeOn();
		selectieScript.playerCam = playerCam;
		
		//Turn off mouse pointer and set the cursorImage
		screenpointer = (GUITexture)Instantiate(baseGuiTexture);
		Screen.showCursor = false; 
		screenpointer.texture = cursorImage;
		screenpointer.color = Color.red;
		screenpointer.pixelInset = new Rect(-5, -5,10,10);
		screenpointer.transform.localScale -= new Vector3(1, 1, 0);
		
		modus = Modi.NAV_SEL;
	}
	
	// Update is called once per frame
	void Update () {
		//Laat hier temp staan OVERGEZET
		selectieScript.updateSelection(Input.mousePosition.x, Input.mousePosition.y);
		smoothCameraScript.UpdateCamera();
	
		//check of we een nieuwe kamer geselecteerd hebben vanuit topview en navigeer daarnaar
		/* OVERZETTEN */
		if(moveToSelectedRoom){
			smoothCameraScript.goToRoom(selectieScript.lastGameObjectHit.transform.position);
			modus = Modi.NAV_SEL;
		}
		
		/*OVERGEZET*/
		// aborting modus
		if (Input.GetKeyUp("q")){			
			Debug.Log("Abort");
			rotateScript.End();
			scaleScript.End();
			stackScript.Abort();
			moveScript.End();
			
			if(returnToCamShowAfterManipulation)
				modus = Modi.CAM_SHO;
			else {
				smoothCameraScript.GaNaarVorigePositie();		
				modus = Modi.NAV_SEL;
			}
				
			deselectAll();		
		} 
		// go to moving/changing parent modus
		else if(selectieScript.lastGameObjectHit && modus == Modi.CHO_MAN && Input.GetButtonUp("Fire1")) {
			Debug.Log(selectieScript.getButtonOnPosition(Input.mousePosition.x, Input.mousePosition.y));
			if (selectieScript.getButtonOnPosition(Input.mousePosition.x, Input.mousePosition.y) == "Move"){		
				Debug.Log("Move change parent");	
				rotateScript.End();
				scaleScript.End();
				stackScript.End();
			
				modus = Modi.MOV;
				moveScript.Begin(selectieScript.lastGameObjectHit);	
			}
			else if (selectieScript.getButtonOnPosition(Input.mousePosition.x, Input.mousePosition.y) == "Rotate"){
				Debug.Log("rotate");
				scaleScript.End();
				stackScript.End();
				moveScript.End();
				smoothCameraScript.GaNaarVorigePositie();			
							
				modus = Modi.ROT;
				rotateScript.Begin(selectieScript.lastGameObjectHit);
			}
			else if (selectieScript.getButtonOnPosition(Input.mousePosition.x, Input.mousePosition.y) == "Add"){
				Debug.Log("stack");
				rotateScript.End();
				scaleScript.End();
				moveScript.End();			
				
				modus = Modi.STA;	
				stackScript.Begin(selectieScript.lastGameObjectHit);
			}
			else if (selectieScript.getButtonOnPosition(Input.mousePosition.x, Input.mousePosition.y) == "Delete"){
				Debug.Log("remove");
				rotateScript.End();
				scaleScript.End();
				stackScript.End();
				moveScript.End();			
			
				modus = Modi.DEL;	
			}
			else if (selectieScript.getButtonOnPosition(Input.mousePosition.x, Input.mousePosition.y) == "Scale"){
				Debug.Log("scale");
				rotateScript.End();
				stackScript.End();
				moveScript.End();
				smoothCameraScript.GaNaarVorigePositie();			
				
				modus = Modi.SCA;		
				scaleScript.Begin(selectieScript.lastGameObjectHit);
			}
		// go to scale modus
		} else if(selectieScript.lastGameObjectHit && modus == Modi.CHO_MAN && Input.GetKeyUp("e")){		
			Debug.Log("scale");
			rotateScript.End();
			stackScript.End();
			moveScript.End();
			smoothCameraScript.GaNaarVorigePositie();			
				
			modus = Modi.SCA;		
			scaleScript.Begin(selectieScript.lastGameObjectHit);
		// go to rotate modus		
		}  else if(selectieScript.lastGameObjectHit && modus == Modi.CHO_MAN && Input.GetKeyUp("r")){		
			Debug.Log("rotate");
			scaleScript.End();
			stackScript.End();
			moveScript.End();
			smoothCameraScript.GaNaarVorigePositie();			
						
			modus = Modi.ROT;
			rotateScript.Begin(selectieScript.lastGameObjectHit);
		// go to stacking modus
		} else if(selectieScript.lastGameObjectHit && modus == Modi.CHO_MAN && Input.GetKeyUp("t")){		
			Debug.Log("stack");
			rotateScript.End();
			scaleScript.End();
			moveScript.End();			
			
			modus = Modi.STA;	
			stackScript.Begin(selectieScript.lastGameObjectHit);
		// go to removing status
		} else if(selectieScript.lastGameObjectHit && modus == Modi.CHO_MAN && Input.GetKeyUp("y")){	
			Debug.Log("remove");
			rotateScript.End();
			scaleScript.End();
			stackScript.End();
			moveScript.End();			
			
			modus = Modi.DEL;	
		// go to slideshow modus
		} else if(Input.GetKeyUp("u")){			
			Debug.Log("slideshow");
			rotateScript.End();
			scaleScript.End();
			stackScript.End();
			moveScript.End();			
			
			deselectAll();
			smoothCameraScript.getNextCameraLokatie();
			modus = Modi.CAM_SHO;	
		} 
		
		//Select an object
		if (modus == Modi.CHO_MAN || modus == Modi.NAV_SEL){
			if(selectieScript.isActive && Input.GetButtonUp("Fire1")){
				if(selectieScript.GUIShown)
				{
					selectieScript.hideUI();
					selectieScript.GUIShown = false;
					modus = Modi.NAV_SEL;
				}
				else
				{
					ObjectScript script = (ObjectScript)selectieScript.lastGameObjectHit.GetComponent("ObjectScript");
					string[] choices={ "Geen keuzes"}; //wordt overreden indien niet null
					if (script != null){
						choices = script.getObjectPossibilities();
					}
					selectieScript.showUi(Input.mousePosition.x, Input.mousePosition.y, choices);
					selectieScript.GUIShown = true;
					modus = Modi.CHO_MAN;
				}
			}
			/*
			selectieScript.fireEvent = true; //Vuur een clickevent af in selectie
			//TODO: if object selected?
			if(selectieScript.lastGameObjectHit != null){
				Debug.Log("Naar manipulatiemode");
				modus = Modi.CHO_MAN;
				}*/
		}
		
		//DEEL PER DEEL DOEN -> KNOP PER KNOP IN WIIMOTE | STAAT "DONE" NA UPDATEFUNCT. INDIEN GEIMPLEMENTEERD
		switch(modus){
			case Modi.NAV_SEL: /* DONE  */
				selectieScript.setSelectionmodeOn();
				updateNavigation();
				break;
			case Modi.CHO_MAN: /* DONE  */
				selectieScript.setSelectionmodeOn();
				updateManipulationChoosing();
				break;
			case Modi.MOV: /* DONE - TOTEST */ 
				selectieScript.setSelectionmodeOff();
				updateMovingManipulation();
				selectieScript.guiscript.hideGui();
				break;
			case Modi.ROT: /* DONE - TOTEST */
				selectieScript.setSelectionmodeOff();
				updateRotatingManipulation();
				selectieScript.guiscript.hideGui();
				break;
			case Modi.SCA: /* DONE - TOTEST */ 
				selectieScript.setSelectionmodeOff();
				updateScalingManipulation();
				selectieScript.guiscript.hideGui();
				break;
			case Modi.STA: /* DONE - TOTEST */
				selectieScript.setSelectionmodeOff();
				updateStackingManipulation();
				selectieScript.guiscript.hideGui();
				break;
			case Modi.DEL: /* DONE - TOTEST */
				selectieScript.setSelectionmodeOff();
				updateDeletingManipulation();
				selectieScript.guiscript.hideGui();
				break;
			case Modi.CAM_SHO: /* DONE |MERGED WITH NAV_SEL | TOTEST */
				//selectieScript.setSelectionmodeOn();
				updateCameraShow();
				//selectieScript.guiscript.hideGui();
				break;
		}
		
		//Set the gui shizzle
		Vector3 mousePos= Input.mousePosition;
		float mouseX = mousePos.x/Screen.width;
		float mouseY = mousePos.y/Screen.height;
		screenpointer.transform.position = new Vector3(mouseX, mouseY, 0);
		Rect cursloc = new Rect(mousePos.x, Screen.height - mousePos.y, cursorImage.width, cursorImage.height);
	}
	
	void OnGUI(){
		GUI.Label(new Rect(25, 25, 100, 30), modus.ToString());
	}
	
	private void checkForNewSelection(){
	
	//selectionScript.setSelectionmodeOn();
	//selectieScript.lastGameObjectHit = selectieScript.selectieScript.lastGameObjectHit;
	
	/*
		if(lastHooveredGameObject && (ObjectScript) lastHooveredGameObject.GetComponent("ObjectScript"))
			((ObjectScript) lastHooveredGameObject.GetComponent("ObjectScript")).hooverThis(false);
	
		GameObject go = raycastscript.getTargetObjects(Input.mousePosition, playerCam.camera);
		if (go != null){			
			lastHooveredGameObject = go;
			if(((ObjectScript) go.GetComponent("ObjectScript")))
				((ObjectScript) go.GetComponent("ObjectScript")).hooverThis(true);			
		}
	
		if (Input.GetButton("Fire1")){ //click 
			if (go != null){
				modus = Modi.CHO_MAN;
				if(selectieScript.lastGameObjectHit && ((ObjectScript) selectieScript.lastGameObjectHit.GetComponent("ObjectScript")))
					((ObjectScript) selectieScript.lastGameObjectHit.GetComponent("ObjectScript")).selectThis(false);	
				selectieScript.lastGameObjectHit = go;
				if(selectieScript.lastGameObjectHit && ((ObjectScript) selectieScript.lastGameObjectHit.GetComponent("ObjectScript"))){
					((ObjectScript) selectieScript.lastGameObjectHit.GetComponent("ObjectScript")).hooverThis(false);
					((ObjectScript) selectieScript.lastGameObjectHit.GetComponent("ObjectScript")).selectThis(true);	
				}
			}
		}*/
	}
	
	private void deselectAll(){
	/*
		if(lastHooveredGameObject && (ObjectScript) lastHooveredGameObject.GetComponent("ObjectScript"))
			((ObjectScript) lastHooveredGameObject.GetComponent("ObjectScript")).hooverThis(false);
		lastHooveredGameObject = null;
		if(selectieScript.lastGameObjectHit && (ObjectScript) selectieScript.lastGameObjectHit.GetComponent("ObjectScript"))
			((ObjectScript) selectieScript.lastGameObjectHit.GetComponent("ObjectScript")).selectThis(false);	
		selectieScript.lastGameObjectHit = null;
		*/
		//selectieScript.lastGameObjectHit = null;
	}
	
	/*DONE*/
	private void updateNavigation(){
		if (Input.GetKey("left"))
			moveCameraLeft();
		if (Input.GetKey("right"))
			moveCameraRight();
		if (Input.GetKey("up"))
			moveCameraForward();
		if (Input.GetKey("down"))
			moveCameraBackward();
		if (Input.GetKey("g"))
			rotateCameraLeft();
		if (Input.GetKey("h"))
			rotateCameraRight();
			
		//checkForNewSelection();
	}
	
	private void updateManipulationChoosing(){			
		//checkForNewSelection();
		
		// TODO CHECK 2D MENU SELECTION
	}
	
	private void updateMovingManipulation(){
		if(moveScript.gridModus){
			if (Input.GetKeyUp("left")){
				moveScript.goToNextAvailablePositionLeft();	
				}
			if (Input.GetKeyUp("right"))
				moveScript.goToNextAvailablePositionRight();
			if (Input.GetKeyUp("up"))
				moveScript.goToNextAvailablePositionTop();
			if (Input.GetKeyUp("down"))
				moveScript.goToNextAvailablePositionDown();
		}else{
			if (Input.GetKey("left"))
				moveScript.goToLeft();	
			if (Input.GetKey("right"))
				moveScript.goToRight();
			if (Input.GetKey("up"))
				moveScript.goToTop();
			if (Input.GetKey("down"))
				moveScript.goToBottom();
		}			
		if (Input.GetKeyUp ("i") && moveScript.gridModus) /* WIIMOTE | NOT IMPLEMENTED */
			moveScript.goToNextAvailablePosition(); // scrolling  left-right, bottom-up
		if (Input.GetKeyUp ("o")){ /* IMPLEMENTED */
			moveScript.End(); // end this manipulation
			modus = Modi.CHO_MAN;
			
		}
		
		if(lastHooveredGameObject && (ObjectScript) lastHooveredGameObject.GetComponent("ObjectScript"))
			((ObjectScript) lastHooveredGameObject.GetComponent("ObjectScript")).hooverThis(false);
	
		GameObject newParent = raycastscript.getTargetObjects(Input.mousePosition, playerCam.camera);
		if(newParent != null){		/* TODO: Not implemented -> Needed? */	
			lastHooveredGameObject = newParent;
			if(((ObjectScript) newParent.GetComponent("ObjectScript")))
				((ObjectScript) newParent.GetComponent("ObjectScript")).hooverThis(true);			
		
			if (Input.GetButton("Fire1")) //click 
				moveScript.changeStackParent(selectieScript.lastGameObjectHit, newParent);
		}
		else
			lastHooveredGameObject = null;
	}
	
	private void updateRotatingManipulation(){
		if (Input.GetKey("left"))
			rotateScript.RotateLeft();
		if (Input.GetKey("right"))
			rotateScript.RotateRight();
		if (Input.GetKeyUp("o")){
			rotateScript.End(); // end this manipulation
			modus = Modi.CHO_MAN;
		}
		
		checkForNewSelection();
	}
	
	private void updateScalingManipulation(){
		if (Input.GetKey("down"))
			scaleScript.ScaleKleiner();
		if (Input.GetKey("up"))
			scaleScript.ScaleGroter();
		if (Input.GetKey("p"))
			scaleScript.volgendeAs();
		if (Input.GetKeyUp("p"))
			scaleScript.releaseLock();
		if (Input.GetKeyUp("o")){
			scaleScript.End(); // end this manipulation
			modus = Modi.CHO_MAN;
		}		
		
		checkForNewSelection();
	}
	
	private void updateStackingManipulation(){
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
				
			if (Input.GetKeyUp ("k") && stackScript.gridModus) //TODO: NOT IMPLEMENTED
				stackScript.goToNextAvailablePosition(); // scrolling  left-right, bottom-up
			if (Input.GetKeyUp ("i"))
				stackScript.goToNextPossibleStackedObject(); // scrolling between possible object
			if (Input.GetKeyUp ("o")){
				stackScript.End(); // end this manipulation
				modus = Modi.CHO_MAN;
			}
		}	
	}
	
	private void updateDeletingManipulation(){
		verwijderScript.DeleteObject(selectieScript.lastGameObjectHit);
		selectieScript.lastGameObjectHit = null;	
		modus = Modi.NAV_SEL;
		checkForNewSelection();
	}
	
	private void updateCameraShow(){

		if (Input.GetKeyUp ("right")){
			smoothCameraScript.getNextCameraLokatie();
		}

		if (Input.GetKeyUp ("left")){
			smoothCameraScript.getVorigCameraLokatie();
		}

		if (Input.GetKeyUp("up")){
			smoothCameraScript.GaNaarTopView();
		}

		if(Input.GetKeyUp("down")){
			smoothCameraScript.returnFromTopview2();
		}

		checkForNewSelection();

		if(selectieScript.lastGameObjectHit != null && selectieScript.lastGameObjectHit.tag == "vloer"){
			moveToSelectedRoom = true;
			returnToCamShowAfterManipulation = false;
		}

		else if(selectieScript.lastGameObjectHit != null && selectieScript.lastGameObjectHit.tag != "vloer"){
			moveToSelectedRoom = false;
			returnToCamShowAfterManipulation = true;
		}

		else if(selectieScript.lastGameObjectHit == null){
			returnToCamShowAfterManipulation = false;
			moveToSelectedRoom = false;
		}

}
	
	//COPYPASTA
	private void moveCameraLeft()
	{		
		Vector3 cameraRelative = playerCam.transform.TransformDirection (-moveStep,0,0);	
		bool collision = raycastscript.getWall(playerCam.transform.position, cameraRelative);
		if(!collision){
			cameraRelative.y = 0;		
			playerCam.transform.localPosition += cameraRelative;		
			smoothCameraScript.eigenLokatie = playerCam.transform.position;
		}	
	}	
	private void moveCameraRight()
	{		
		Vector3 cameraRelative = playerCam.transform.TransformDirection (moveStep,0,0);
		bool collision = raycastscript.getWall(playerCam.transform.position, cameraRelative);
		if(!collision){
			cameraRelative.y = 0;		
			playerCam.transform.localPosition += cameraRelative;		
			smoothCameraScript.eigenLokatie = playerCam.transform.position;
		}
	}	
	private void moveCameraForward()
	{		
		Vector3 cameraRelative = playerCam.transform.TransformDirection (0,0,moveStep);	
		bool collision = raycastscript.getWall(playerCam.transform.position, cameraRelative);
		if(!collision){
			cameraRelative.y = 0;		
			playerCam.transform.localPosition += cameraRelative;		
			smoothCameraScript.eigenLokatie = playerCam.transform.position;
		}
	}	
	private void moveCameraBackward()
	{
		Vector3 cameraRelative = playerCam.transform.TransformDirection (0,0,-moveStep);	
		bool collision = raycastscript.getWall(playerCam.transform.position, cameraRelative);
		if(!collision){
			cameraRelative.y = 0;		
			playerCam.transform.localPosition += cameraRelative;		
			smoothCameraScript.eigenLokatie = playerCam.transform.position;
		}
	}	
	private void rotateCameraLeft()
	{
		playerCam.transform.Rotate(0,-rotateStep,0);		
		smoothCameraScript.eigenLookAt = playerCam.transform.rotation;
	}	
	private void rotateCameraUp()
	{
		playerCam.transform.Rotate(rotateStep,0,0);		
		smoothCameraScript.eigenLookAt = playerCam.transform.rotation;
	}	
	private void rotateCameraDown()
	{		
		playerCam.transform.Rotate(-rotateStep,0,0);		
		smoothCameraScript.eigenLookAt = playerCam.transform.rotation;
	}	
	private void rotateCameraRight()
	{		
		playerCam.transform.Rotate(0,rotateStep,0);		
		smoothCameraScript.eigenLookAt = playerCam.transform.rotation;
	}
	private void rotateCameraLeft90Degrees(){
		Debug.Log("Rotate camera left");
		playerCam.transform.Rotate(0,-90,0);
	}
	private void rotateCameraRight90Degrees(){
		Debug.Log("Rotate camera right");
		playerCam.transform.Rotate(0,90,0);
	}
}
