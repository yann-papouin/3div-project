using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class WiiController : MonoBehaviour {
	//DLLImports: handjes af
	[DllImport ("UniWii")]
	private static extern void wiimote_start();
	[DllImport ("UniWii")]
	private static extern void wiimote_stop();
	[DllImport ("UniWii")]
	private static extern int wiimote_count();
	[DllImport ("UniWii")]
	private static extern byte wiimote_getAccX(int which);
	[DllImport ("UniWii")]
	private static extern byte wiimote_getAccY(int which);
	[DllImport ("UniWii")]
	private static extern byte wiimote_getAccZ(int which);
	[DllImport ("UniWii")]
	private static extern float wiimote_getIrX(int which);
	[DllImport ("UniWii")]
	private static extern float wiimote_getIrY(int which);
	[DllImport ("UniWii")]
	private static extern float wiimote_getRoll(int which);
	[DllImport ("UniWii")]
	private static extern float wiimote_getPitch(int which);
	[DllImport ("UniWii")]
	private static extern float wiimote_getYaw(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonA(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonB(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonUp(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonLeft(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonRight(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonDown(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButton1(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButton2(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonPlus(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonMinus(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonHome(int which);
	[DllImport ("UniWii")]
	private static extern byte wiimote_getNunchuckStickX(int which);
	[DllImport ("UniWii")]
	private static extern byte wiimote_getNunchuckStickY(int which);
	[DllImport ("UniWii")]
	private static extern byte wiimote_getNunchuckAccX(int which);
	[DllImport ("UniWii")]
	private static extern byte wiimote_getNunchuckAccZ(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonNunchuckC(int which);
	[DllImport ("UniWii")]
	private static extern bool wiimote_getButtonNunchuckZ(int which);

	//Wiimote
	private int lightIndicatorCount; 
	private bool[] isExpansion;
	private string display;
	private Vector3 vec;
	private Vector3 oldVec;
	private int wiimoteCount;
	//wiicoords
	private int x;
	private int y;
	private int z;
	private float ir_x;
	private float ir_y;
	private float temp_x;
	private float temp_y;
	private float roll;
	private float pitch;
	private float yaw;
	//Nunchuck
	private float nx; 
	private float ny;
	private float nsx;
	private float nsy;
	
	//Public van wiimote
	public float sensitivity = 8.0f;
	public float pitchFudge = 30.0f;
	public float rollFudge = 50.0f;
	public bool showDebugGUI = true;
	
	//Gui
	public Texture2D cursorImage; 
	public GUITexture baseGuiTexture;
	private GUITexture screenpointer;
	
	//Scripts
	private RayCastScript raycastscript;
	private RotateScript rotateScript;
	private ScaleScript scaleScript;
	private StackScript stackScript;
	private MoveScript moveScript;
	private SmoothCameraScript smoothCameraScript;
	private VerwijderScript verwijderScript;
	private SelectieScript selectieScript;	

	//Shared tussen keyboard en wiimote
	public GameObject playerCam; 
	public float moveStep = 0.5f;
	public float rotateStep = 2.0f;
	private bool moveToSelectedRoom = false;
	private bool changeParent = false;
	private bool returnToCamShowAfterManipulation = false;
	private enum Modi{NAV_SEL, CAM_SHO, CHO_MAN, DEL, MOV, ROT, SCA, STA}; //TODO: Cam_sho = nav_sel
	private Modi modus; //Huidige modus

	private bool topview = false;
	// Use this for initialization
	void Start () {
		//Init controllers
		wiimote_start();
		
		//Turn off mouse pointer and set the cursorImage
		screenpointer = (GUITexture)Instantiate(baseGuiTexture);
		Screen.showCursor = false;
		screenpointer.texture = cursorImage;
		screenpointer.color = Color.red;
		screenpointer.pixelInset = new Rect(10,10,10,10);
		screenpointer.transform.localScale -= new Vector3(1,1,0);
		
		//Set ref scripts
		raycastscript = gameObject.GetComponent("RayCastScript") as RayCastScript;
		rotateScript = gameObject.GetComponent("RotateScript") as RotateScript;
		scaleScript = gameObject.GetComponent("ScaleScript") as ScaleScript;
		stackScript = gameObject.GetComponent("StackScript") as StackScript;
		moveScript = gameObject.GetComponent("MoveScript") as MoveScript;
		smoothCameraScript = gameObject.GetComponent("SmoothCameraScript") as SmoothCameraScript;
		verwijderScript = gameObject.GetComponent("VerwijderScript") as VerwijderScript;
		selectieScript = gameObject.GetComponent("SelectieScript") as SelectieScript;
		
		//Set initial values
		selectieScript.setSelectionmodeOn();
		selectieScript.playerCam = playerCam;
		modus = Modi.NAV_SEL;
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	/*Buttonlocks */
	private bool rollpulse = false;
	private bool buttonpulse = false;
	private	bool minbuttonpulse = false;
	private	bool plusbuttonpulse = false;
	private bool button1pulse = false;
	private bool button2pulse = false;
	private bool padbuttonpulse = false;
	private	bool homebuttonpulse = false;

	void FixedUpdate(){
		wiimoteCount = wiimote_count();
		if (wiimoteCount > 0)
		{
			//Nota: alleen 1 wiimote is rekening mee gehouden, 2x privates nodig voor 2e of afwisselend behandelen
			x = wiimote_getAccX(0);
			y = wiimote_getAccY(0);
			z = wiimote_getAccZ(0);
			roll = Mathf.Round(wiimote_getRoll(0) + rollFudge);
			pitch = Mathf.Round(wiimote_getPitch(0) + pitchFudge);
			yaw = Mathf.Round(wiimote_getYaw(0));
			
			//Orientation vectors				
			if (!float.IsNaN(roll) && !float.IsNaN(pitch)) {
					vec = new Vector3(pitch, 0 , -1 * roll);
/*					vec = new Vector3(pitch, yaw , -1 * roll);*/
					vec = Vector3.Lerp(oldVec, vec, Time.deltaTime * sensitivity);
					oldVec = vec;
				}

			/* ir Gui  */
			ir_x = wiimote_getIrX(0);
			ir_y = wiimote_getIrY(0);
			//Fuck off basic zooi die er al was, Jens coming through
			if ( (ir_x != -100) && (ir_y != -100) ) {
				temp_x = (ir_x + 1)/2;
				temp_y = (ir_y + 1)/2;
				screenpointer.transform.position = new Vector3(temp_x, temp_y, 0);
			}
			
			//Updaten van muispointers
			selectieScript.updateSelection(temp_x * Screen.width, temp_y * Screen.height);
			smoothCameraScript.UpdateCamera();
			
			//wanneer we gemerkt hebben dat de gebruiker in topview een vloer heeft geselecteerd, zetten we hem eerst terug op de grond voor iets anders te doen
			//de camera gaat in de smoothcamerascript de gebruiker terug in een draaiende beweging in het midden van de geselecteerde kamer zetten
			if(moveToSelectedRoom){
				smoothCameraScript.goToRoom(selectieScript.lastGameObjectHit.transform.position);
				modus = Modi.NAV_SEL;
				topview = false;
			}
			//Hier komen alle eigen manipulaties
			
			/*Klikken op A en B */ //TODO:single - pulse - klik :/
			if (wiimote_getButtonA(0) && wiimote_getButtonB(0) && !buttonpulse)
			{
				buttonpulse = true;
				switch(modus){
					//TOONT HET MENU in selectie of manipulatiemode
					case Modi.NAV_SEL: //In selectiemodus: klikken = selecteren
							if(selectieScript.GUIShown)
							{
								selectieScript.hideUI();
								selectieScript.GUIShown = false;
								modus = Modi.NAV_SEL;
							}
							else{
								ObjectScript script = (ObjectScript)selectieScript.lastGameObjectHit.GetComponent("ObjectScript");
								string[] choices = {"Geen keuzes"};
								if (script != null)
									choices = script.getObjectPossibilities();
								selectieScript.showUi(temp_x * Screen.width, temp_y * Screen.height, choices);
								selectieScript.GUIShown = true;
								modus = Modi.CHO_MAN;
							}
						break;
					case Modi.CHO_MAN:
						//Zelfde als selectie -> terug naar nav gaan
						if(selectieScript.GUIShown)
						{
							selectieScript.hideUI();
							selectieScript.GUIShown = false;
							modus = Modi.NAV_SEL;
						}
						else{
							ObjectScript script = (ObjectScript)selectieScript.lastGameObjectHit.GetComponent("ObjectScript");
							string[] choices = {"Geen keuzes"};
							if (script != null)
								choices = script.getObjectPossibilities();
							selectieScript.showUi(temp_x * Screen.width, temp_y * Screen.height, choices);
							selectieScript.GUIShown = true;
							modus = Modi.CHO_MAN;
						}
						break;
					case Modi.MOV:
						GameObject newParent = raycastscript.getTargetObjects(new Vector3(temp_x * Screen.width, temp_y * Screen.height, 0), playerCam.camera);
						if (newParent != null && newParent != selectieScript.lastGameObjectHit){
							moveScript.changeStackParent(selectieScript.lastGameObjectHit, newParent);
						}
						break;
				}
			}else if (wiimote_getButtonA(0)) //Alleen A ingedrukt = menu kiezen
			{
				//buttonpulse = true;
				if (selectieScript.lastGameObjectHit && modus == Modi.CHO_MAN){
					//Recalculate the gui pixel values from ir values
					float realvalX = temp_x * Screen.width;
					float realvalY = temp_y * Screen.height;
					Debug.Log(selectieScript.getButtonOnPosition(realvalX, realvalY));
					if (selectieScript.getButtonOnPosition(realvalX, realvalY) == "Move"){		
						Debug.Log("Move change parent");	
						rotateScript.End();
						scaleScript.End();
						stackScript.End();					
						modus = Modi.MOV;
						moveScript.Begin(selectieScript.lastGameObjectHit);	
					}
					else if (selectieScript.getButtonOnPosition(realvalX, realvalY) == "Rotate"){
						Debug.Log("rotate");
						scaleScript.End();
						stackScript.End();
						moveScript.End();
						smoothCameraScript.GaNaarVorigePositie();												
						modus = Modi.ROT;
						rotateScript.Begin(selectieScript.lastGameObjectHit);
					}
					else if (selectieScript.getButtonOnPosition(realvalX, realvalY) == "Add"){
						Debug.Log("stack");
						rotateScript.End();
						scaleScript.End();
						moveScript.End();								
						modus = Modi.STA;	
						stackScript.Begin(selectieScript.lastGameObjectHit);
					}
					else if (selectieScript.getButtonOnPosition(realvalX, realvalY) == "Delete"){
						Debug.Log("remove");
						rotateScript.End();
						scaleScript.End();
						stackScript.End();
						moveScript.End();								
						//modus = Modi.DEL;	//Niet nodig: alles gebeurt hier
						verwijderScript.DeleteObject(selectieScript.lastGameObjectHit);
						selectieScript.lastGameObjectHit = null;
						selectieScript.hideUI();
						selectieScript.GUIShown = false;
						modus = Modi.NAV_SEL;
					}
					else if (selectieScript.getButtonOnPosition(realvalX, realvalY) == "Scale"){
						Debug.Log("scale");
						rotateScript.End();
						stackScript.End();
						moveScript.End();
						smoothCameraScript.GaNaarVorigePositie();									
						modus = Modi.SCA;		
						scaleScript.Begin(selectieScript.lastGameObjectHit);
					}							
				}
			}else if(wiimote_getButtonB(0)){
				//buttonpulse = true;
				//Alleen knop B: niks in zetten dat al in A+B zit -> conflicten
			}else{
				//Geen knop ingedrukt
				buttonpulse = false;
			}
			
			if (wiimote_getButtonMinus(0) && !minbuttonpulse) //Terug naar vorige modus
			{
				minbuttonpulse = true;
				if (modus == Modi.NAV_SEL){
					smoothCameraScript.returnFromTopview2();
					topview = false;
				}
				else if (modus == Modi.MOV){ //FROM move TO choicemodus
					moveScript.End();
					modus = Modi.NAV_SEL; 
				}
				else if (modus == Modi.ROT){
					rotateScript.End();
					modus = Modi.NAV_SEL; 
				}
				else if (modus == Modi.SCA){
					scaleScript.End();
					modus = Modi.NAV_SEL; 
				}
				else if (modus == Modi.STA){
					stackScript.End();
					modus = Modi.NAV_SEL; 
				}
			}else{
				minbuttonpulse = false;
			}
			
			if (wiimote_getButtonPlus(0) )
			{
				if(modus == Modi.NAV_SEL){
						if (topview){
							if(selectieScript.lastGameObjectHit != null && selectieScript.lastGameObjectHit.tag == "vloer"){
								moveToSelectedRoom = true;
								returnToCamShowAfterManipulation = false;
								topview = false;
							}

							else if(selectieScript.lastGameObjectHit != null && selectieScript.lastGameObjectHit.tag != "vloer"){
								moveToSelectedRoom = false;
								returnToCamShowAfterManipulation = true;
								topview = true;
							}

							else if(selectieScript.lastGameObjectHit == null){
								returnToCamShowAfterManipulation = false;
								moveToSelectedRoom = false;
								topview = true;
							}
						}
				}
				if (modus == Modi.SCA && !plusbuttonpulse){
					plusbuttonpulse = true;
					scaleScript.releaseLock();
					scaleScript.volgendeAs();
				}
			}else{
				plusbuttonpulse = false;
			}
			
			if (wiimote_getButtonHome(0) && !homebuttonpulse){ //Abort all
				homebuttonpulse = true;
				Debug.Log("Abort");
				rotateScript.End();
				scaleScript.End();
				stackScript.Abort();
				moveScript.End();
				smoothCameraScript.GaNaarVorigePositie();
				modus = Modi.NAV_SEL;
				selectieScript.hideUI();
				selectieScript.GUIShown = false;
				topview = false;
			}else{
				homebuttonpulse = false;
			}
			
			if (wiimote_getButton1(0) && !button1pulse){ //Wisselen van camhoek
				button1pulse = true;
				if (modus == Modi.NAV_SEL){
					smoothCameraScript.getNextCameraLokatie();
				}
			}else{
				button1pulse = false;
			}
			
			if (wiimote_getButton2(0) && !button1pulse){ //Wisselen van camhoek
				button2pulse = true;
				if (modus == Modi.NAV_SEL){
					smoothCameraScript.getVorigCameraLokatie();
				}
			}else{
				button2pulse = false;
			}
			
			if (wiimote_getButtonLeft(0)){
				padbuttonpulse = true;
				if (modus == Modi.NAV_SEL)
					moveCameraLeft();
				else if (modus == Modi.MOV){
					if (moveScript.gridModus)
						moveScript.goToNextAvailablePositionLeft();	
					else
						moveScript.goToLeft();	
				}
				else if (modus == Modi.ROT){
					rotateScript.RotateLeft();
				}
				else if (modus == Modi.STA ){
					if (stackScript.isActive){
						if (stackScript.gridModus){
							stackScript.goToNextAvailablePositionLeft();
						}
						else{
							stackScript.goToLeft();	
						}
					}
				}		
			}
			else if (wiimote_getButtonRight(0)){
				padbuttonpulse = true;
				if (modus == Modi.NAV_SEL)
					moveCameraRight();
				else if (modus == Modi.MOV){
					if (moveScript.gridModus)
						moveScript.goToNextAvailablePositionRight();	
					else
						moveScript.goToRight();	
				}
				else if (modus == Modi.ROT){
					rotateScript.RotateRight();
				}
				else if (modus == Modi.STA){
					if (stackScript.isActive){
						if (stackScript.gridModus){
							stackScript.goToNextAvailablePositionRight();
						}
						else{
							stackScript.goToRight();	
						}
					}
				}
			}
			else if (wiimote_getButtonUp(0)){
				padbuttonpulse = true;
				if (modus == Modi.NAV_SEL && !topview)
					moveCameraForward();
				else if (modus == Modi.MOV){
					if (moveScript.gridModus)
						moveScript.goToNextAvailablePositionTop();	
					else
						moveScript.goToTop();	
				}
				else if (modus == Modi.STA){
					if (stackScript.isActive){
						if (stackScript.gridModus){
							stackScript.goToNextAvailablePositionTop();
						}
						else{
							stackScript.goToTop();	
						}
					}
				}
			}
			else if (wiimote_getButtonDown(0)){
				padbuttonpulse = true;
				if (modus == Modi.NAV_SEL && !topview)
					moveCameraBackward();
				else if (modus == Modi.MOV){
					if (moveScript.gridModus)
						moveScript.goToNextAvailablePositionDown();	
					else
						moveScript.goToBottom();	
				}
				else if (modus == Modi.STA){
					if (stackScript.isActive){
						if (stackScript.gridModus){
							stackScript.goToNextAvailablePositionDown();
						}
						else{
							stackScript.goToBottom();	
						}
					}
				}
			}else{
				padbuttonpulse = false;
			}
			
			//TODO: roll moet 0.5 sec ingehouden worden voordat deze reageert
			//Dus: functie aanroepen die 0.5 sec telt en dan pas bool op false zet
			if (!checkRollOutOfBounds()){
				Debug.Log("Roll in bounds");
				if (roll <= -20 ){ //Wiimote links kantelen
					if (modus == Modi.NAV_SEL && !topview && !rollpulse){ //In navigatie
						rollpulse = true;	
						rotateCameraLeft90Degrees();
					}
					else if (modus == Modi.ROT){
						rotateScript.RotateLeft();
					}
					else if (modus == Modi.STA && !rollpulse){
						rollpulse = true;
						if (stackScript.isActive){
							stackScript.goToNextPossibleStackedObject();
						}
					}else if (modus == Modi.CHO_MAN && !rollpulse){
						rollpulse = true;
						selectieScript.getNextChild();
					}
				}else if (roll >= 120){ //Wiimote rechts kantelen
					if (modus == Modi.NAV_SEL && !topview && !rollpulse){
						rollpulse = true;
						rotateCameraRight90Degrees();
					}	
					else if (modus == Modi.ROT){
						rotateScript.RotateRight();
					}
					else if (modus == Modi.STA && !rollpulse){
						rollpulse = true;
						if (stackScript.isActive)
							stackScript.goToNextPossibleStackedObject();
					}
				}
				else{
					rollpulse = false;
				}
			}
			
			//Pitch gebruiken voor schaleren
			if (!checkPitchOutOfBounds()){
				Debug.Log("Pitch in bounds");
				if (pitch <= -30){ //Wiimote licht recht houden
					if (modus == Modi.SCA)
						scaleScript.ScaleGroter();
				}else if (pitch >= 100 ) { //TESTEN!
					if (modus == Modi.SCA)
						scaleScript.ScaleKleiner();
				}
						
				if (pitch <= -70 && !topview){
					if (modus == Modi.NAV_SEL && checkRollOutOfBounds()){
						smoothCameraScript.GaNaarTopView();
						topview = true;
					}
				}
			}
				
			//Laatste check om te zien of selectiemode aan of uit moet (anders ingewikkeld in overzetten)
			//Reden: nu per key -> dan naar navigatie gemapt wegens wiimote - issues
			if(modus == Modi.NAV_SEL || modus == Modi.CHO_MAN)
				selectieScript.setSelectionmodeOn();
			else{
				selectieScript.setSelectionmodeOff();
				selectieScript.guiscript.hideGui();
				}
				
		}
		
		//Debug
		if (showDebugGUI)
			DoDebugStr(0);	
	}
	
	//Gui
	void OnGUI() {
		if (showDebugGUI) {
			/* show debug info */
			GUI.Label( new Rect(10,10, Screen.width-10, Screen.height-10), display);
			}
		GUI.Label(new Rect(Screen.width/2, 25, 100, 30), modus.ToString()); //Current modus
	}
	
		
	void DoDebugStr (int i) {
		if (wiimoteCount > 0) {
			display = "";

			display += "Wiimote #" + 1 + ":"
				+ "\n\tAccelerometer: " + x + ", " + y + ", " + z 
				+ "\n\t\tRoll: " + roll 
				+ "\n\t\tPitch: " + pitch 
				+ "\n\t\tYaw: " + yaw 
				+ "\n\tIR Pitch & Yaw: " + ir_x + ", " + ir_y
				+ "\n\tOrientation Vector: " + vec
				+ "\n\tSensitivity: " + sensitivity.ToString("F2");
			display += "\n\tButtons Active: ";
			if (wiimote_getButtonA(0)) display += " A ";
			if (wiimote_getButtonB(0)) display += " B ";
			if (wiimote_getButtonHome(0)) display += " Home ";
			if (wiimote_getButtonPlus(0)) display += " + ";
			if (wiimote_getButtonMinus(0)) display += " - ";
			if (wiimote_getButton1(0)) display += " 1 ";
			if (wiimote_getButton2(0)) display += " 2 ";
			/* dpad */
			if (wiimote_getButtonUp(0)) display += " Up ";
			if (wiimote_getButtonDown(0)) display += " Down ";
			if (wiimote_getButtonLeft(0)) display += " Left ";
			if (wiimote_getButtonRight(0)) display += " Right ";
			display += "xcoord" + temp_x + "\nycoord" + temp_y;
		}
		else {
			display = "No Wii Remote detected... \nPress the '1' & '2' buttons on your Wii Remote to search!";
		}
	}
	void OnApplicationQuit() {
		//wiimote_stop();
	}
	
	private bool checkRollOutOfBounds(){
		if (roll > 200 || roll < -120)
			return true;	
		return false;
	}
	private bool checkPitchOutOfBounds(){
		if ( pitch > 200 || pitch < -130)
			return true;
		return false;
	}
	//Eigen functies -> Doe hier om te zorgen dat functies makkelijk copypastebaar zijn tussen keyboard / wiimotescript
	private void moveCameraLeft(){
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
		playerCam.transform.Rotate(0,-90,0);
	}
	private void rotateCameraRight90Degrees(){
		playerCam.transform.Rotate(0,90,0);
	}	
}
