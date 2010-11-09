using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using System;

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
	private int[] cursor_x, cursor_y;
	private Vector3 vec;
	private Vector3 oldVec;
	private int wiimoteCount;
	//wiicoords
	private int x;
	private int y;
	private int z;
	private float ir_x;
	private float ir_y;
	private float roll;
	private float pitch;
	private float yaw;
	//Nunchuck
	private float nx; 
	private float ny;
	private float nsx;
	private float nsy;
	
	//Public
	public float sensitivity = 8.0f;
	public float pitchFudge = 30.0f;
	public float rollFudge = 50.0f;
	public bool showDebugGUI = true;
	public float moveStep = 0.5f;
	public float rotateStep = 2.0f;
	public Texture2D wiimote_cursor_tex; //Texture voor de wiimote pointer?
	
	//Shared tussen keyboard en wiimote
	public GameObject playerCam; 

	// Use this for initialization
	void Start () {
		//Init controllers
		wiimote_start();
		cursor_x = new int[16];
		cursor_y = new int[16];
		//wiimote_cursor_tex = (Texture2D) Resources.Load("crosshair");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
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
			ir_x = wiimote_getIrX(0);
			ir_y = wiimote_getIrY(0);
			
			//Orientation vectors				
			if (!float.IsNaN(roll) && !float.IsNaN(pitch)) {
					vec = new Vector3(pitch, 0 , -1 * roll);
/*					vec = new Vector3(pitch, yaw , -1 * roll);*/
					vec = Vector3.Lerp(oldVec, vec, Time.deltaTime * sensitivity);
					oldVec = vec;
				}
			/* ir cursor values */
			if ( (ir_x != -100) && (ir_y != -100) ) {
				float temp_x = ((ir_x + 1.0f)/ 2.0f) * Screen.width;
			    float temp_y = Screen.height - (((ir_y + 1.0f)/ 2.0f) * Screen.height);
			    cursor_x[wiimoteCount] = Mathf.RoundToInt(temp_x);
			    cursor_y[wiimoteCount] = Mathf.RoundToInt(temp_y);
			}	
			
			//Hierna komen eigen aanroepen en tests voor bijv. camera enzo
			if (roll <= 30) //Wiimote links kantelen
				moveCameraLeft();
			else if (roll >=130) //Wiimote rechts kantelen
				moveCameraRight();
			
			if (pitch <= -60) //Wiimote rechtop houden
				zoomCameraOut();
		}
		
		//Debug
		if (showDebugGUI)
			DoDebugStr(0);	
	}
	
	//Gui
	void OnGUI() {
		if (showDebugGUI) {
			GUIStyle label_wiimote_cursor;
			if (wiimote_cursor_tex) {
				label_wiimote_cursor = new GUIStyle();
				label_wiimote_cursor.normal.background = wiimote_cursor_tex;
			}
			else
				label_wiimote_cursor = "box";
			
			label_wiimote_cursor.clipping = TextClipping.Overflow;
			label_wiimote_cursor.normal.textColor = Color.red;

			/* show debug info */
			GUI.Label( new Rect(10,10, Screen.width-10, Screen.height-10), display);

			/* use ir */
			/*
			float ir_x = wiimote_getIrX(0);
			float ir_y = wiimote_getIrY(0);
			float temp_x = (Screen.width * 0.5f) + ir_x * Screen.width * 0.5f;
			float temp_y = Screen.height - (ir_y * Screen.height * 0.5f);
			GUI.Box ( new Rect (temp_x, temp_y, 64.0f, 64.0f), "IR Pointer #" + 1, label_wiimote_cursor);
			*/
			Debug.Log("Started the Debug gui for the wiimote");
			}
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
		}
		else {
			display = "No Wii Remote detected... \nPress the '1' & '2' buttons on your Wii Remote to search!";
		}
	}
	void OnApplicationQuit() {
		wiimote_stop();
	}
	//Eigen functies -> Doe hier om te zorgen dat functies makkelijk copypastebaar zijn tussen keyboard / wiimotescript
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
