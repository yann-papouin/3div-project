using UnityEngine;
using System.Collections;
using System;

// when executing functions of this script -> assume view = topview
// left-right axis of selectedObject points to the right
public class StackScript : MonoBehaviour {
	public bool isActive;	
	public bool gridModus;
	public Color gridColor;
	public float moveStep;
	private GameObject selectedObject;
	private ObjectScript scriptOfSelectedObject;		
	private ArrayList lines;
	private int currentIndexOfPossibleStackedObject;
	private GameObject currentPossibleStackedObject;
	private ObjectScript scriptOfCurrentPossibleStackedObject;
	private int currRowInGrid, currColInGrid;
	private float currLeftRightOnObject, currTopDownOnObject;
	private Vector3 currPointInGridGlobalCoords, currPointInGridLocalCoords;
	private bool topDownAxisInverted;
	private float topDownLength, leftRightLength, height; // dimension of selectedObject
	
	// storing previous camera view
	private Vector3 prevPosCamera;
	private Quaternion prevRotCamera;

	// Use this for initialization
	void Start () {
		isActive = false;
		lines = new ArrayList();
		currPointInGridGlobalCoords = new Vector3(0,0,0);
		currPointInGridLocalCoords = new Vector3(0,0,0);
	}	
	
	public void Begin(GameObject carrier){
		if(!isActive){
			isActive = true;
			selectedObject = carrier;
			scriptOfSelectedObject = (ObjectScript) selectedObject.GetComponent("ObjectScript");	
			currentIndexOfPossibleStackedObject = 0;
			prevPosCamera = Camera.main.transform.position;
			prevRotCamera = Camera.main.transform.rotation;
			
			if(!scriptOfSelectedObject || scriptOfSelectedObject.possibleChildren.Length == 0 || scriptOfSelectedObject.possibleChildren[currentIndexOfPossibleStackedObject].Length == 0){
				isActive = false;
				return;
			}
						
			scriptOfSelectedObject.changeToTopview();			
			topDownAxisInverted = topDownAxisIsInverted();
			calculateDimensions();
			
			if(gridModus)
				goToFirstAvailablePosition();		
			else
				goToDefaultPosition();	
		}
	}
	
	private bool topDownAxisIsInverted(){
		bool result = false;
		switch(scriptOfSelectedObject.localAxisTopDown[0]){
			case 'X':
				result = Math.Abs(Vector3.Angle(Camera.main.transform.up,selectedObject.transform.right)) > 2.0f;
				break;
			case 'Y':
				result = Math.Abs(Vector3.Angle(Camera.main.transform.up,selectedObject.transform.up)) > 2.0f;
				break;
			case 'Z':
				result = Math.Abs(Vector3.Angle(Camera.main.transform.up, selectedObject.transform.forward)) > 2.0f;
				break;
		}
		return result;
	}
	
	private void calculateDimensions(){
		Vector3 scale = selectedObject.transform.localScale;
		Mesh mesh = selectedObject.GetComponent<MeshFilter>().mesh;		
		Bounds bounds = mesh.bounds;
		Vector3 size = bounds.size;
		
		switch(scriptOfSelectedObject.localAxisTopDown[0]){			
			case 'X':
				topDownLength = size.x;//*scale.x;
				break;
			case 'Y':
				topDownLength = size.y;//*scale.y;
				break;
			case 'Z':
				topDownLength = size.z;//*scale.z;
				break;
		}		
		switch(scriptOfSelectedObject.localAxisLeftRight[0]){			
			case 'X':
				leftRightLength = size.x;//*scale.x;
				break;
			case 'Y':
				leftRightLength = size.y;//*scale.y;
				break;
			case 'Z':
				leftRightLength = size.z;//*scale.z;
				break;
		}
		switch(scriptOfSelectedObject.localUpAxis[0]){			
			case 'X':
				height = size.x;//*scale.x;
				break;
			case 'Y':
				height = size.y;//*scale.y;
				break;
			case 'Z':
				height = size.z;//*scale.z;
				break;
		}
	}
	
	public void goToNextPossibleStackedObject(){
		if(!isActive)
			return;
		
		Vector3 temp = currentPossibleStackedObject.transform.position;
		temp.y = -50f;//make the previous shown object "invisible"
		currentPossibleStackedObject.transform.position = temp;
		
		currentIndexOfPossibleStackedObject++;	
		if(currentIndexOfPossibleStackedObject >= scriptOfSelectedObject.possibleChildren.Length)
			currentIndexOfPossibleStackedObject = 0;
		if(scriptOfSelectedObject.possibleChildren[currentIndexOfPossibleStackedObject].Length == 0)
			currentIndexOfPossibleStackedObject = 0;
			
		setPossibleStackedObject(scriptOfSelectedObject.possibleChildren[currentIndexOfPossibleStackedObject]);			
	}
		
	public void goToNextAvailablePositionLeft(){
		bool placed = false;
		int currCol = currColInGrid;
		
		do{
			currCol--;
			if(currCol < 0)
				;
			else if(scriptOfSelectedObject.isGridCellAvailable(currCol, currRowInGrid) == true)
				placed = true;
		
		}while(!placed && currCol >= 0);
		
		if(placed){
			currColInGrid = currCol;	
			drawGrid();
			setPossibleStackedObject(scriptOfSelectedObject.possibleChildren[currentIndexOfPossibleStackedObject]);
		}
	}
	
	public void goToNextAvailablePositionRight(){
		bool placed = false;
		int currCol = currColInGrid;
		
		do{
			currCol++;
			if(currCol >= scriptOfSelectedObject.gridSizeLeftRight)
				;
			else if(scriptOfSelectedObject.isGridCellAvailable(currCol, currRowInGrid) == true)
				placed = true;
		
		}while(!placed && currCol < scriptOfSelectedObject.gridSizeLeftRight);
		
		if(placed){
			currColInGrid = currCol;	
			drawGrid();
			setPossibleStackedObject(scriptOfSelectedObject.possibleChildren[currentIndexOfPossibleStackedObject]);
		}
	}
	
	public void goToNextAvailablePositionTop(){
		bool placed = false;
		int currRow = currRowInGrid;
		
		do{
			currRow++;
			if(currRow >= scriptOfSelectedObject.gridSizeTopBottom)
				;
			else if(scriptOfSelectedObject.isGridCellAvailable(currColInGrid, currRow) == true)
				placed = true;
		
		}while(!placed && currRow < scriptOfSelectedObject.gridSizeTopBottom);
		
		if(placed){
			currRowInGrid = currRow;	
			drawGrid();
			setPossibleStackedObject(scriptOfSelectedObject.possibleChildren[currentIndexOfPossibleStackedObject]);
		}
	}
	
	public void goToNextAvailablePositionDown(){
		bool placed = false;
		int currRow = currRowInGrid;
		
		do{
			currRow--;
			if(currRow < 0)
				;
			else if(scriptOfSelectedObject.isGridCellAvailable(currColInGrid, currRow) == true)
				placed = true;
		
		}while(!placed && currRow >= 0);
		
		if(placed){
			currRowInGrid = currRow;	
			drawGrid();
			setPossibleStackedObject(scriptOfSelectedObject.possibleChildren[currentIndexOfPossibleStackedObject]);
		}
	}
	
	public void goToNextAvailablePosition(){
		if(!isActive)
			return;
			
		bool found = false;
		bool first = true;
		currColInGrid++;
		if(currColInGrid >= scriptOfSelectedObject.gridSizeLeftRight){
			currColInGrid = 0;
			currRowInGrid++;
		}		
		
		for(int i = currRowInGrid; i < scriptOfSelectedObject.gridSizeTopBottom && !found; i++){
			if(!first)
				currColInGrid = 0;
			else 
				first = false;
			for(int j = currColInGrid; j < scriptOfSelectedObject.gridSizeLeftRight && !found; j++){
				if(scriptOfSelectedObject.isGridCellAvailable(j, i) == true){
					found = true;
					currRowInGrid = i;
					currColInGrid = j;
					Debug.Log("found posL: " + currColInGrid + ", posS: " + currRowInGrid);	
				}
				else
					Debug.Log("not found posL: " + currColInGrid + ", posS: " + currRowInGrid);
			}
		}	
		
		if(!found){
			goToFirstAvailablePosition();
		}			
		
		drawGrid();
		setPossibleStackedObject(scriptOfSelectedObject.possibleChildren[currentIndexOfPossibleStackedObject]);
	}	
	
	private void goToFirstAvailablePosition(){
		if(!isActive)
			return;
			
		bool found = false;						
		for(int i = scriptOfSelectedObject.gridSizeTopBottom/2; i < scriptOfSelectedObject.gridSizeTopBottom && !found; i++){
			for(int j = scriptOfSelectedObject.gridSizeLeftRight/2; j < scriptOfSelectedObject.gridSizeLeftRight && !found; j++){
				if(scriptOfSelectedObject.isGridCellAvailable(j, i) == true){
					found = true;			
					currRowInGrid = i;
					currColInGrid = j;
					Debug.Log("found col: " + currColInGrid + ", row: " + currRowInGrid);	
				}
				else
					Debug.Log("not found col: " + currColInGrid + ", row: " + currRowInGrid);
			}	
			for(int j = scriptOfSelectedObject.gridSizeLeftRight/2; j >=0 && !found; j--){
				if(scriptOfSelectedObject.isGridCellAvailable(j, i) == true){
					found = true;			
					currRowInGrid = i;
					currColInGrid = j;
					Debug.Log("found col: " + currColInGrid + ", row: " + currRowInGrid);	
				}
				else
					Debug.Log("not found col: " + currColInGrid + ", row: " + currRowInGrid);
			}	
		}
		
		for(int i = scriptOfSelectedObject.gridSizeTopBottom/2; i >= 0 && !found; i--){
			for(int j = scriptOfSelectedObject.gridSizeLeftRight/2; j < scriptOfSelectedObject.gridSizeLeftRight && !found; j++){
				if(scriptOfSelectedObject.isGridCellAvailable(j, i) == true){
					found = true;			
					currRowInGrid = i;
					currColInGrid = j;
					Debug.Log("found col: " + currColInGrid + ", row: " + currRowInGrid);	
				}
				else
					Debug.Log("not found col: " + currColInGrid + ", row: " + currRowInGrid);
			}	
			for(int j = scriptOfSelectedObject.gridSizeLeftRight/2; j >=0 && !found; j--){
				if(scriptOfSelectedObject.isGridCellAvailable(j, i) == true){
					found = true;			
					currRowInGrid = i;
					currColInGrid = j;
					Debug.Log("found col: " + currColInGrid + ", row: " + currRowInGrid);	
				}
				else
					Debug.Log("not found col: " + currColInGrid + ", row: " + currRowInGrid);
			}	
		}
		
		if(found == false)
			Abort();
			
		drawGrid();
		setPossibleStackedObject(scriptOfSelectedObject.possibleChildren[currentIndexOfPossibleStackedObject]);
	}	
	
	private void goToDefaultPosition(){
		if(!isActive)
			return;	
		
		currLeftRightOnObject = leftRightLength/2.0f;
		currTopDownOnObject = topDownLength/2.0f;
		
		drawNoGrid();
		setPossibleStackedObject(scriptOfSelectedObject.possibleChildren[currentIndexOfPossibleStackedObject]);		
	}
	
	public void goToBottom(){
		currTopDownOnObject -= moveStep;
		if(currTopDownOnObject < 0.0f)
			currTopDownOnObject = 0.0f;
		
		drawNoGrid();
		setPossibleStackedObject(scriptOfSelectedObject.possibleChildren[currentIndexOfPossibleStackedObject]);		
	}	
	public void goToTop(){
		currTopDownOnObject += moveStep;
		if(currTopDownOnObject > topDownLength)
			currTopDownOnObject = topDownLength;
		
		drawNoGrid();
		setPossibleStackedObject(scriptOfSelectedObject.possibleChildren[currentIndexOfPossibleStackedObject]);		
	}
	public void goToLeft(){
		currLeftRightOnObject -= moveStep;
		if(currLeftRightOnObject < 0.0f)
			currLeftRightOnObject = 0.0f;
		
		drawNoGrid();
		setPossibleStackedObject(scriptOfSelectedObject.possibleChildren[currentIndexOfPossibleStackedObject]);			
	}
	public void goToRight(){
		currLeftRightOnObject += moveStep;
		if(currLeftRightOnObject > leftRightLength)
			currLeftRightOnObject = leftRightLength;
		
		drawNoGrid();
		setPossibleStackedObject(scriptOfSelectedObject.possibleChildren[currentIndexOfPossibleStackedObject]);		
	}
	
	
	// The object will be cloned and placed onto selectedObject
	public void End(){
		if(!isActive)
			return;
				
		if(currentPossibleStackedObject){
			Vector3 temp = currentPossibleStackedObject.transform.position;
			temp.y = -50f;
			currentPossibleStackedObject.transform.position = temp;
			
			Vector3 coords = currPointInGridGlobalCoords;		
			ObjectScript oscript = (ObjectScript) currentPossibleStackedObject.GetComponent("ObjectScript");
			string name_ = (string) (oscript.clone(coords,Quaternion.identity));
			GameObject clone = GameObject.Find(name_);
			((ObjectScript)  clone.GetComponent("ObjectScript")).setParentTransform(selectedObject.transform);			
		}
		
		foreach( GameObject obj in lines){
			Destroy(obj);
		}
		lines.Clear();
		
		Camera.main.transform.position = prevPosCamera;
		Camera.main.transform.rotation = prevRotCamera;
		isActive = false;
	}
	
	// The new object will not be placed
	public void Abort(){
		if(!isActive)
			return;
			
		isActive = false;
		if(currentPossibleStackedObject){
			Vector3 temp = currentPossibleStackedObject.transform.position;
			temp.y = -50f;
			currentPossibleStackedObject.transform.position = temp;
		}
		foreach( GameObject obj in lines){
			Destroy(obj);
		}
		lines.Clear();
				
		Camera.main.transform.position = prevPosCamera;
		Camera.main.transform.rotation = prevRotCamera;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private void setPossibleStackedObject(string name){
		// this object will be cloned when chosen -> do NOT make it a child of the selected object
	
		currentPossibleStackedObject = GameObject.Find(name);
		scriptOfCurrentPossibleStackedObject = (ObjectScript) currentPossibleStackedObject.GetComponent("ObjectScript");	
		
		Vector3 pos = currentPossibleStackedObject.transform.position;
		pos = currPointInGridGlobalCoords;
		pos.y += currentPossibleStackedObject.renderer.bounds.extents.y;
		
		currentPossibleStackedObject.transform.position = pos;
	}
	
	private void drawGrid(){	
		if(!isActive)
			return;		
		
		Debug.Log("col: " + currColInGrid + ", row: " + currRowInGrid);	
			
		/**
		**	REMOVE THE PREVIOUSLY CALCULATED LINES
		**/	
		foreach( GameObject obj in lines){
			Destroy(obj);
		}
		lines.Clear();
		
		Vector3 scale = selectedObject.transform.localScale;
		Mesh mesh = selectedObject.GetComponent<MeshFilter>().mesh;		
		Bounds bounds = mesh.bounds;
		
		/**
		**	CALCULATE THE OBJECT POSITION IN THE GRID
		**/
		switch(scriptOfSelectedObject.localUpAxis[0]){				
			case 'X':
				currPointInGridLocalCoords.x = bounds.center.x + (float) height/2;
				break;
			case 'Y':
				currPointInGridLocalCoords.y = bounds.center.y + (float) height/2;
				break;
			case 'Z':
				currPointInGridLocalCoords.z = bounds.center.z + (float) height/2;
				break;
		}
		switch(scriptOfSelectedObject.localAxisLeftRight[0]){				
			case 'X':
				currPointInGridLocalCoords.x = (float) (bounds.center.x  - leftRightLength/2 + leftRightLength/(2*scriptOfSelectedObject.gridSizeLeftRight)) + currColInGrid*(leftRightLength/scriptOfSelectedObject.gridSizeLeftRight);
				break;
			case 'Y':
				currPointInGridLocalCoords.y = (float) (bounds.center.y  - leftRightLength/2 + leftRightLength/(2*scriptOfSelectedObject.gridSizeLeftRight)) + currColInGrid*(leftRightLength/scriptOfSelectedObject.gridSizeLeftRight);
				break;
			case 'Z':
				currPointInGridLocalCoords.z = (float) (bounds.center.z  - leftRightLength/2 + leftRightLength/(2*scriptOfSelectedObject.gridSizeLeftRight)) + currColInGrid*(leftRightLength/scriptOfSelectedObject.gridSizeLeftRight);
				break;
		}
		
		if(!topDownAxisInverted){
			Debug.Log("not inverted");
			switch(scriptOfSelectedObject.localAxisTopDown[0]){				
				case 'X':
					currPointInGridLocalCoords.x = (float) (bounds.center.x  - topDownLength/2 + topDownLength/(2*scriptOfSelectedObject.gridSizeTopBottom)) + currRowInGrid*(topDownLength/scriptOfSelectedObject.gridSizeTopBottom);
					break;
				case 'Y':
					currPointInGridLocalCoords.y = (float) (bounds.center.y  - topDownLength/2 + topDownLength/(2*scriptOfSelectedObject.gridSizeTopBottom)) + currRowInGrid*(topDownLength/scriptOfSelectedObject.gridSizeTopBottom);
					break;
				case 'Z':
					currPointInGridLocalCoords.z = (float) (bounds.center.z  - topDownLength/2 + topDownLength/(2*scriptOfSelectedObject.gridSizeTopBottom)) + currRowInGrid*(topDownLength/scriptOfSelectedObject.gridSizeTopBottom);
					break;
			}
		}else{
			Debug.Log("inverted");
			switch(scriptOfSelectedObject.localAxisTopDown[0]){				
				case 'X':
					currPointInGridLocalCoords.x = (float) (bounds.center.x  + topDownLength/2 - topDownLength/(2*scriptOfSelectedObject.gridSizeTopBottom)) - currRowInGrid*(topDownLength/scriptOfSelectedObject.gridSizeTopBottom);
					break;
				case 'Y':
					currPointInGridLocalCoords.y = (float) (bounds.center.y  + topDownLength/2 - topDownLength/(2*scriptOfSelectedObject.gridSizeTopBottom)) - currRowInGrid*(topDownLength/scriptOfSelectedObject.gridSizeTopBottom);
					break;
				case 'Z':
					currPointInGridLocalCoords.z = (float) (bounds.center.z  + topDownLength/2 - topDownLength/(2*scriptOfSelectedObject.gridSizeTopBottom)) - currRowInGrid*(topDownLength/scriptOfSelectedObject.gridSizeTopBottom);
					break;
			}		
		}
		
		currPointInGridGlobalCoords = selectedObject.transform.TransformPoint(currPointInGridLocalCoords);
			
			
		/**
		**	DRAW THE LARGE GRID LINES
		**/
		for(int row = 0; row < scriptOfSelectedObject.gridSizeTopBottom; ++row)
		{
			Vector3 from = new Vector3(0,0,0);
			Vector3 to = new Vector3(0,0,0);
			switch(scriptOfSelectedObject.localUpAxis[0]){				
				case 'X':
					from.x =  bounds.center.x  + (float) height/2;
					to.x = bounds.center.x  + (float)  height/2;
					break;
				case 'Y':
					from.y = bounds.center.y  + (float) height/2;
					to.y = bounds.center.y  + (float) height/2;
					break;
				case 'Z':
					from.z = bounds.center.z  + (float) height/2;
					to.z = bounds.center. z + (float) height/2;
					break;
			}
			switch(scriptOfSelectedObject.localAxisLeftRight[0]){
				case 'X':
					from.x = bounds.center. x + ((float) -leftRightLength/2);
					to.x = bounds.center. x + (float) leftRightLength/2;
					break;
				case 'Y':
					from.y = bounds.center.y  + ((float) -leftRightLength/2);
					to.y = bounds.center. y + (float) leftRightLength/2;
					break;
				case 'Z':
					from.z = bounds.center.z  + ((float) -leftRightLength/2);
					to.z = bounds.center.z  + (float) leftRightLength/2;
					break;
			}
			
			switch(scriptOfSelectedObject.localAxisTopDown[0]){
				case 'X':
					from.x = (float) (bounds.center.x   - topDownLength/2 + topDownLength/(2*scriptOfSelectedObject.gridSizeTopBottom)) + row*(topDownLength/scriptOfSelectedObject.gridSizeTopBottom);
					to.x = (float) (bounds.center.x   - topDownLength/2 + topDownLength/(2*scriptOfSelectedObject.gridSizeTopBottom)) + row*(topDownLength/scriptOfSelectedObject.gridSizeTopBottom);
					
					break;
				case 'Y':
					from.y =(float) (bounds.center.y   - topDownLength/2 + topDownLength/(2*scriptOfSelectedObject.gridSizeTopBottom)) + row*(topDownLength/scriptOfSelectedObject.gridSizeTopBottom);
					to.y = (float) (bounds.center. y  - topDownLength/2 + topDownLength/(2*scriptOfSelectedObject.gridSizeTopBottom)) + row*(topDownLength/scriptOfSelectedObject.gridSizeTopBottom);
					break;
				case 'Z':
					from.z = (float) (bounds.center.z   - topDownLength/2 + topDownLength/(2*scriptOfSelectedObject.gridSizeTopBottom)) + row*(topDownLength/scriptOfSelectedObject.gridSizeTopBottom);
					to.z = (float) (bounds.center. z  - topDownLength/2 + topDownLength/(2*scriptOfSelectedObject.gridSizeTopBottom)) + row*(topDownLength/scriptOfSelectedObject.gridSizeTopBottom);
					break;
			}
			
		
			Vector3 fromGlobal = selectedObject.transform.TransformPoint(from);
			Vector3 toGlobal = selectedObject.transform.TransformPoint(to);
			fromGlobal.y += 0.2f;
			toGlobal.y += 0.2f;
			
			GameObject line = new GameObject();
			LineRenderer rend = line.AddComponent<LineRenderer>();
			rend.material = new Material(Shader.Find("Particles/Additive"));
			rend.useWorldSpace = true;
			rend.SetWidth(0.08f, 0.08f);
			rend.SetColors(gridColor, gridColor);
			rend.SetVertexCount(2);
			rend.SetPosition(0, fromGlobal);
			rend.SetPosition(1, toGlobal);
			lines.Add(line);			
			
			//Debug.Log("From: " + fromGlobal.x + ", "  + fromGlobal.y + ", "  + fromGlobal.z);
			//Debug.Log("To: " + toGlobal.x + ", "  + toGlobal.y + ", "  + toGlobal.z);
		}	

		/**
		**	DRAW THE SMALL GRID LINES
		**/
		for(int col = 0; col < scriptOfSelectedObject.gridSizeLeftRight; ++col)
		{
			Vector3 from = new Vector3(0,0,0);
			Vector3 to = new Vector3(0,0,0);
			switch(scriptOfSelectedObject.localUpAxis[0]){				
				case 'X':
					from.x = bounds.center.x  + (float) height/2;
					to.x = bounds.center.x  + (float)  height/2;
					break;
				case 'Y':
					from.y = bounds.center.y  +  (float) height/2;
					to.y = bounds.center. y + (float) height/2;
					break;
				case 'Z':
					from.z = bounds.center. z + (float) height/2;
					to.z = bounds.center.z  + (float) height/2;
					break;
			}
			switch(scriptOfSelectedObject.localAxisLeftRight[0]){
				case 'X':
					from.x = (float) (bounds.center.x  + -leftRightLength/2 + leftRightLength/(2*scriptOfSelectedObject.gridSizeLeftRight)) + col*(leftRightLength/scriptOfSelectedObject.gridSizeLeftRight);
					to.x = (float) (bounds.center. x + -leftRightLength/2 + leftRightLength/(2*scriptOfSelectedObject.gridSizeLeftRight)) + col*(leftRightLength/scriptOfSelectedObject.gridSizeLeftRight);
					break;
				case 'Y':
					from.y = (float) (bounds.center.y  + -leftRightLength/2 + leftRightLength/(2*scriptOfSelectedObject.gridSizeLeftRight)) + col*(leftRightLength/scriptOfSelectedObject.gridSizeLeftRight);
					to.y = (float) (bounds.center.y  + -leftRightLength/2 + leftRightLength/(2*scriptOfSelectedObject.gridSizeLeftRight)) + col*(leftRightLength/scriptOfSelectedObject.gridSizeLeftRight);
					break;
				case 'Z':
					from.z = (float) (bounds.center. z + -leftRightLength/2 + leftRightLength/(2*scriptOfSelectedObject.gridSizeLeftRight)) + col*(leftRightLength/scriptOfSelectedObject.gridSizeLeftRight);
					to.z = (float) (bounds.center.z  + -leftRightLength/2 + leftRightLength/(2*scriptOfSelectedObject.gridSizeLeftRight)) + col*(leftRightLength/scriptOfSelectedObject.gridSizeLeftRight);
					break;
			}
			switch(scriptOfSelectedObject.localAxisTopDown[0]){
				case 'X':
					from.x = bounds.center.x  + (float) -topDownLength/2;
					to.x = bounds.center.x  + (float) topDownLength/2;
					break;
				case 'Y':
					from.y =bounds.center.y  + (float) -topDownLength/2;
					to.y = bounds.center.y  + (float) topDownLength/2;
					break;
				case 'Z':
					from.z = bounds.center.z  + (float) -topDownLength/2;
					to.z = bounds.center.z  + (float) topDownLength/2;
					break;
			}
		
			Vector3 fromGlobal = selectedObject.transform.TransformPoint(from);
			Vector3 toGlobal = selectedObject.transform.TransformPoint(to);
			fromGlobal.y += 0.2f;
			toGlobal.y += 0.2f;
			
			GameObject line = new GameObject();
			LineRenderer rend = line.AddComponent<LineRenderer>();
			rend.material = new Material(Shader.Find("Particles/Additive"));
			rend.useWorldSpace = true;
			rend.SetWidth(0.08f, 0.08f);
			rend.SetColors(gridColor,gridColor);
			rend.SetVertexCount(2);
			rend.SetPosition(0, fromGlobal);
			rend.SetPosition(1, toGlobal);
			lines.Add(line);
			
			
			//Debug.Log("From: " + fromGlobal.x + ", "  + fromGlobal.y + ", "  + fromGlobal.z);
			//Debug.Log("To: " + toGlobal.x + ", "  + toGlobal.y + ", "  + toGlobal.z);
		}	
	}

	private void drawNoGrid(){
		if(!isActive)
			return;		
		
		Vector3 scale = selectedObject.transform.localScale;
		Mesh mesh = selectedObject.GetComponent<MeshFilter>().mesh;		
		Bounds bounds = mesh.bounds;
		
		/**
		**	CALCULATE THE OBJECT POSITION IN THE GRID
		**/
		switch(scriptOfSelectedObject.localUpAxis[0]){				
			case 'X':
				currPointInGridLocalCoords.x = bounds.center.x + (float) height/2;
				break;
			case 'Y':
				currPointInGridLocalCoords.y = bounds.center.y + (float) height/2;
				break;
			case 'Z':
				currPointInGridLocalCoords.z = bounds.center.z + (float) height/2;
				break;
		}
		switch(scriptOfSelectedObject.localAxisLeftRight[0]){				
			case 'X':
				currPointInGridLocalCoords.x = (float) (bounds.center.x  - leftRightLength/2 + currLeftRightOnObject);
				break;
			case 'Y':
				currPointInGridLocalCoords.y = (float) (bounds.center.y  - leftRightLength/2 + currLeftRightOnObject);
				break;
			case 'Z':
				currPointInGridLocalCoords.z = (float) (bounds.center.z  - leftRightLength/2 + currLeftRightOnObject);
				break;
		}
		
		if(!topDownAxisInverted){
			Debug.Log("not inverted");
			switch(scriptOfSelectedObject.localAxisTopDown[0]){				
				case 'X':
					currPointInGridLocalCoords.x = (float) (bounds.center.x  - topDownLength/2 + currTopDownOnObject);
					break;
				case 'Y':
					currPointInGridLocalCoords.y = (float) (bounds.center.y  - topDownLength/2 + currTopDownOnObject);
					break;
				case 'Z':
					currPointInGridLocalCoords.z = (float) (bounds.center.z  - topDownLength/2 + currTopDownOnObject);
					break;
			}
		}else{
			Debug.Log("inverted");
			switch(scriptOfSelectedObject.localAxisTopDown[0]){				
				case 'X':
					currPointInGridLocalCoords.x = (float) (bounds.center.x  + topDownLength/2 - currTopDownOnObject);
					break;
				case 'Y':
					currPointInGridLocalCoords.y = (float) (bounds.center.y  + topDownLength/2 - currTopDownOnObject);
					break;
				case 'Z':
					currPointInGridLocalCoords.z = (float) (bounds.center.z  + topDownLength/2 - currTopDownOnObject);
					break;
			}		
		}
		
		currPointInGridGlobalCoords = selectedObject.transform.TransformPoint(currPointInGridLocalCoords);
	}	
}
