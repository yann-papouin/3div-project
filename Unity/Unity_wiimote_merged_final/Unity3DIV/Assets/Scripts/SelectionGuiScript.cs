using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SelectionGuiScript : MonoBehaviour {

	public GameObject playerCam;

	private bool enableGui = false;
	private float mouseX, mouseY;
	private float paddingX, paddingY; //Padding for the grouped box
	private static int guiWidth = 400;
	private static int guiHeight = 300;
	private string[] choiceArray ={"Move", "Add", "Delete", "Rotate", "Scale", "Clone"}; //Basis choicearray
	// Use this for initialization
	void Start () {
		Vector3 mousePos= Input.mousePosition;
		mouseX = mousePos.x;
		mouseY = Screen.height - mousePos.y;	
		initGuiRect();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	public void showGui(float cursX, float cursY, string[] choices) 
	{
		setChoiceArray(choices);
		//Vector3 mousePos= Input.mousePosition;
		mouseX = cursX*Screen.width;
		mouseY = Screen.height -(cursY*Screen.height);	
		enableGui = true;
		//Debug.Log("Drawing gui around " + mouseX + ", " + mouseY);
	}
	public void hideGui()
	{
		enableGui = false;
	}
	
	private void OnGUI(){
		GUI.enabled = enableGui;
		if(enableGui){
			buildGui(choiceArray);
			//buildGui(testmap);
		}
	}
	
	public void setChoiceArray(string[] input){
		choiceArray = input;
	}

	public void buildGui(string[] choicearray){
		paddingX = mouseX - guiWidth/2;
		paddingY = mouseY - guiHeight / 2;
		//GUI.BeginGroup(new Rect(mouseX - guiWidth/2, mouseY - guiHeight/2, guiWidth, guiHeight));
		GUI.BeginGroup(new Rect(paddingX, paddingY, guiWidth, guiHeight));
		for(int i = 0; i < choicearray.Length; i++){
			if(choicearray[i] != ""){
				Debug.Log("Creating button with name" + choicearray[i] +".");
				buttonname[i] = choicearray[i];
				GUI.Button(r[i], choicearray[i]);
			}
		}
		GUI.EndGroup();
	}
	
	public string getButtonNameBehindPos(float x, float y){
		string toreturn = "";
		//For some reason, y needs to be inverted...
		y = Screen.height - y;
		//Debug.Log("Original inp: " + x + " : " + y + "\tPadding: " + paddingX + " : " + paddingY);
		Debug.Log("Scanning sector: " + (x - paddingX) + " : " + (y - paddingY));
		for(int i = 0; i < r.Length ||i < 8; i++){
			Debug.Log("Testing block on sector" + r[i].x + " : " + r[i].y);
			if(r[i].Contains(new Vector2(x - paddingX, y - paddingY ))){
				return buttonname[i];
			}
		}
		return toreturn;
	}

	string[] buttonname = new string[8];
	Rect[] r = new Rect[8];
	private void initGuiRect(){
		r[0] = new Rect(guiWidth/2-50, guiHeight-50, 100,50);
		r[1] = new Rect(guiWidth/2-50, 0, 100,50);
		r[2] = new Rect(0, guiHeight/2-25, 100,50);
		r[3] = new Rect(guiWidth-100, guiHeight/2-25, 100,50);
		r[4] = new Rect(0, 0, 100, 50);
		r[5] = new Rect(guiWidth - 100, 0, 100, 50);
		//Andere opties suspended -> houden voor opkuisen, marge te groot langs y waarde
	}

}
