// Author: Sibrand Staessens

using UnityEngine;
using System.Collections;
using System;

/*
*	Script for moving an existing object onto the current stack parent.
*
*/

// when executing functions of this script -> assume view = topview
// left-right axis of stackParentObject points to the right
public class MoveScript : MonoBehaviour {

	public bool isActive;	
	public bool gridModus;
	public Color gridColor;
	public float moveStep;
	protected GameObject selectedObject, parentObject;
	protected ObjectScript scriptOfSelectedObject, scriptOfParentObject;
	
	protected ArrayList lines;
	protected int currRowInGrid, currColInGrid;
	protected float currLeftRightOnObject, currTopDownOnObject;
	protected Vector3 currPointInGridGlobalCoords, currPointInGridLocalCoords;
	protected bool topDownAxisInverted;
	protected float topDownLength, leftRightLength, height; // dimension of parentObject

	// Use this for initialization
	void Start () {
		isActive = false;
		lines = new ArrayList();
		currPointInGridGlobalCoords = new Vector3(0,0,0);
		currPointInGridLocalCoords = new Vector3(0,0,0);
	}	
	
	public void Begin(GameObject moveable){
		if(!isActive){
			isActive = true;
			selectedObject = moveable;
			scriptOfSelectedObject = (ObjectScript) moveable.GetComponent("ObjectScript");	
			if(gridModus){
				currRowInGrid = scriptOfSelectedObject.rowInGrid;
				currColInGrid = scriptOfSelectedObject.colInGrid;
			}else{
				currLeftRightOnObject = scriptOfSelectedObject.posLeftRight;
				currTopDownOnObject = scriptOfSelectedObject.posTopDown;
			}
			
			if(scriptOfSelectedObject == null){
				isActive = false;
				return;
			}
			
			parentObject = scriptOfSelectedObject.getParent();
			if(parentObject == null){
				isActive = false;
				return;
			}		
						
			scriptOfParentObject = 	(ObjectScript) parentObject.GetComponent("ObjectScript");	
			
			((SmoothCameraScript) GetComponent("SmoothCameraScript")).changeViewedObject(parentObject);	
			topDownAxisInverted = topDownAxisIsInverted();
			calculateDimensions();
			
			if(gridModus){
				drawGrid();
			}else{
				drawNoGrid();
			}			
			
			move();
		}
	}
	
	protected bool topDownAxisIsInverted(){
		bool result = false;
		switch(scriptOfParentObject.localAxisTopDown[0]){
			case 'X':
				result = Math.Abs(Vector3.Angle(Camera.main.transform.up,parentObject.transform.right)) > 2.0f;
				break;
			case 'Y':
				result = Math.Abs(Vector3.Angle(Camera.main.transform.up,parentObject.transform.up)) > 2.0f;
				break;
			case 'Z':
				result = Math.Abs(Vector3.Angle(Camera.main.transform.up, parentObject.transform.forward)) > 2.0f;
				break;
		}
		return result;
	}
	
	protected void calculateDimensions(){
		Vector3 scale = parentObject.transform.localScale;
		Mesh mesh = parentObject.GetComponent<MeshFilter>().mesh;		
		Bounds bounds = mesh.bounds;
		Vector3 size = bounds.size;
		
		switch(scriptOfParentObject.localAxisTopDown[0]){			
			case 'X':
				topDownLength = size.x;
				break;
			case 'Y':
				topDownLength = size.y;
				break;
			case 'Z':
				topDownLength = size.z;
				break;
		}		
		switch(scriptOfParentObject.localAxisLeftRight[0]){			
			case 'X':
				leftRightLength = size.x;
				break;
			case 'Y':
				leftRightLength = size.y;
				break;
			case 'Z':
				leftRightLength = size.z;
				break;
		}
		switch(scriptOfParentObject.localUpAxis[0]){			
			case 'X':
				height = size.x;
				break;
			case 'Y':
				height = size.y;
				break;
			case 'Z':
				height = size.z;
				break;
		}
	}
		
	public void goToNextAvailablePositionLeft(){
		bool placed = false;
		int currCol = currColInGrid;
		
		do{
			currCol--;
			if(currCol < 0)
				;
			else if(scriptOfParentObject.isGridCellAvailable(currCol, currRowInGrid) == true)
				placed = true;
		
		}while(!placed && currCol >= 0);
		
		if(placed){
			currColInGrid = currCol;	
			drawGrid();
			move();
		}
	}
		
	public void goToNextAvailablePositionRight(){
		bool placed = false;
		int currCol = currColInGrid;
		
		do{
			currCol++;
			if(currCol >= scriptOfParentObject.gridSizeLeftRight)
				;
			else if(scriptOfParentObject.isGridCellAvailable(currCol, currRowInGrid) == true)
				placed = true;
		
		}while(!placed && currCol < scriptOfParentObject.gridSizeLeftRight);
		
		if(placed){
			currColInGrid = currCol;	
			drawGrid();
			move();
		}
	}
	
	public void goToNextAvailablePositionTop(){
		bool placed = false;
		int currRow = currRowInGrid;
		
		do{
			currRow++;
			if(currRow >= scriptOfParentObject.gridSizeTopBottom)
				;
			else if(scriptOfParentObject.isGridCellAvailable(currColInGrid, currRow) == true)
				placed = true;
		
		}while(!placed && currRow < scriptOfParentObject.gridSizeTopBottom);
		
		if(placed){
			currRowInGrid = currRow;	
			drawGrid();
			move();
		}
	}
	
	public void goToNextAvailablePositionDown(){
		bool placed = false;
		int currRow = currRowInGrid;
		
		do{
			currRow--;
			if(currRow < 0)
				;
			else if(scriptOfParentObject.isGridCellAvailable(currColInGrid, currRow) == true)
				placed = true;
		
		}while(!placed && currRow >= 0);
		
		if(placed){
			currRowInGrid = currRow;	
			drawGrid();
			move();
		}
	}
	
	public void goToNextAvailablePosition(){
		if(!isActive)
			return;
			
		bool found = false;
		bool first = true;
		currColInGrid++;
		if(currColInGrid >= scriptOfParentObject.gridSizeLeftRight){
			currColInGrid = 0;
			currRowInGrid++;
		}		
		
		for(int i = currRowInGrid; i < scriptOfParentObject.gridSizeTopBottom && !found; i++){
			if(!first)
				currColInGrid = 0;
			else 
				first = false;
			for(int j = currColInGrid; j < scriptOfParentObject.gridSizeLeftRight && !found; j++){
				if(scriptOfParentObject.isGridCellAvailable(j, i) == true){
					found = true;
					currRowInGrid = i;
					currColInGrid = j;
				}
				else
					;
			}
		}	
		
		drawGrid();
		move();
	}	
	
	public void goToBottom(){
		currTopDownOnObject -= moveStep;
		if(currTopDownOnObject < 0.0f)
			currTopDownOnObject = 0.0f;
		
		drawNoGrid();
		move();	
	}	
	public void goToTop(){
		currTopDownOnObject += moveStep;
		if(currTopDownOnObject > topDownLength)
			currTopDownOnObject = topDownLength;
		
		drawNoGrid();
		move();		
	}
	public void goToLeft(){
		currLeftRightOnObject -= moveStep;
		if(currLeftRightOnObject < 0.0f)
			currLeftRightOnObject = 0.0f;
		
		drawNoGrid();
		move();			
	}
	public void goToRight(){
		currLeftRightOnObject += moveStep;
		if(currLeftRightOnObject > leftRightLength)
			currLeftRightOnObject = leftRightLength;
		
		drawNoGrid();
		move();		
	}
	
	public void changeStackParent(GameObject selected, GameObject newParent){
		isActive = true;
		selectedObject = selected;
		scriptOfSelectedObject = (ObjectScript) selected.GetComponent("ObjectScript");	
			
		if(scriptOfSelectedObject == null){
			isActive = false;
			return;
		}
			
		parentObject = scriptOfSelectedObject.getParent();
		if(parentObject == null){
			isActive = false;
			return;
		}		
						
		scriptOfParentObject = 	(ObjectScript) parentObject.GetComponent("ObjectScript");			
		ObjectScript npScript = (ObjectScript) newParent.GetComponent("ObjectScript");	
		if(!npScript)
			return;
		if(npScript.canBeStackedOn == false)
			return;
	
		scriptOfParentObject.detachChild(selected);
		parentObject = newParent;						
		scriptOfParentObject = 	npScript;		
		((SmoothCameraScript) GetComponent("SmoothCameraScript")).changeViewedObject(newParent);	
		topDownAxisInverted = topDownAxisIsInverted();
		calculateDimensions();
		
		if(gridModus){
			selected.transform.parent = null;
			selected.transform.rotation = parentObject.transform.rotation;
			goToDefaultPositionGrid();				
			scriptOfParentObject.addChildInGrid(selected, currColInGrid, currRowInGrid);
		}			
		else{
			selected.transform.parent = null;
			selected.transform.rotation = parentObject.transform.rotation;
			goToDefaultPosition();	
			scriptOfSelectedObject.addChild(selected, currLeftRightOnObject, currTopDownOnObject);
		}
	}
	
	protected void goToDefaultPositionGrid(){
		if(!isActive)
			return;
								
		currRowInGrid = scriptOfParentObject.gridSizeTopBottom/2; 
		currColInGrid = scriptOfParentObject.gridSizeLeftRight/2;
					
		drawGrid();
		move();
	}	
	
	protected void goToDefaultPosition(){
		if(!isActive)
			return;	
		
		currLeftRightOnObject = leftRightLength/2.0f;
		currTopDownOnObject = topDownLength/2.0f;
		
		drawNoGrid();
		move();
	}
	
	// The object will be cloned and placed onto selectedObject
	public void End(){
		if(!isActive)
			return;
				
		move();
		
		foreach( GameObject obj in lines){
			Destroy(obj);
		}
		lines.Clear();
		
		((SmoothCameraScript) GetComponent("SmoothCameraScript")).returnFromTopview();
		isActive = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	protected void move(){
		if(gridModus){
			scriptOfSelectedObject.colInGrid = currColInGrid;
			scriptOfSelectedObject.rowInGrid = currRowInGrid;
			
			Vector3 pos = scriptOfSelectedObject.transform.position;
			pos = currPointInGridGlobalCoords;
			pos.y += selectedObject.renderer.bounds.extents.y;		
			scriptOfSelectedObject.transform.position = pos;
		}else{
			scriptOfSelectedObject.posLeftRight = currLeftRightOnObject;
			scriptOfSelectedObject.posTopDown = currTopDownOnObject;
			
			Vector3 pos = scriptOfSelectedObject.transform.position;
			pos = currPointInGridGlobalCoords;
			pos.y += selectedObject.renderer.bounds.extents.y;		
			scriptOfSelectedObject.transform.position = pos;
		}
	}
	
	protected void drawGrid(){	
		if(!isActive)
			return;		
			
		/**
		**	REMOVE THE PREVIOUSLY CALCULATED LINES
		**/	
		foreach( GameObject obj in lines){
			Destroy(obj);
		}
		lines.Clear();
		
		Vector3 scale = parentObject.transform.localScale;
		Mesh mesh = parentObject.GetComponent<MeshFilter>().mesh;		
		Bounds bounds = mesh.bounds;
		
		/**
		**	CALCULATE THE OBJECT POSITION IN THE GRID
		**/
		switch(scriptOfParentObject.localUpAxis[0]){				
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
		switch(scriptOfParentObject.localAxisLeftRight[0]){				
			case 'X':
				currPointInGridLocalCoords.x = (float) (bounds.center.x  - leftRightLength/2 + leftRightLength/(2*scriptOfParentObject.gridSizeLeftRight)) + currColInGrid*(leftRightLength/scriptOfParentObject.gridSizeLeftRight);
				break;
			case 'Y':
				currPointInGridLocalCoords.y = (float) (bounds.center.y  - leftRightLength/2 + leftRightLength/(2*scriptOfParentObject.gridSizeLeftRight)) + currColInGrid*(leftRightLength/scriptOfParentObject.gridSizeLeftRight);
				break;
			case 'Z':
				currPointInGridLocalCoords.z = (float) (bounds.center.z  - leftRightLength/2 + leftRightLength/(2*scriptOfParentObject.gridSizeLeftRight)) + currColInGrid*(leftRightLength/scriptOfParentObject.gridSizeLeftRight);
				break;
		}
		
		if(!topDownAxisInverted){
			Debug.Log("not inverted");
			switch(scriptOfParentObject.localAxisTopDown[0]){				
				case 'X':
					currPointInGridLocalCoords.x = (float) (bounds.center.x  - topDownLength/2 + topDownLength/(2*scriptOfParentObject.gridSizeTopBottom)) + currRowInGrid*(topDownLength/scriptOfParentObject.gridSizeTopBottom);
					break;
				case 'Y':
					currPointInGridLocalCoords.y = (float) (bounds.center.y  - topDownLength/2 + topDownLength/(2*scriptOfParentObject.gridSizeTopBottom)) + currRowInGrid*(topDownLength/scriptOfParentObject.gridSizeTopBottom);
					break;
				case 'Z':
					currPointInGridLocalCoords.z = (float) (bounds.center.z  - topDownLength/2 + topDownLength/(2*scriptOfParentObject.gridSizeTopBottom)) + currRowInGrid*(topDownLength/scriptOfParentObject.gridSizeTopBottom);
					break;
			}
		}else{
			Debug.Log("inverted");
			switch(scriptOfParentObject.localAxisTopDown[0]){				
				case 'X':
					currPointInGridLocalCoords.x = (float) (bounds.center.x  + topDownLength/2 - topDownLength/(2*scriptOfParentObject.gridSizeTopBottom)) - currRowInGrid*(topDownLength/scriptOfParentObject.gridSizeTopBottom);
					break;
				case 'Y':
					currPointInGridLocalCoords.y = (float) (bounds.center.y  + topDownLength/2 - topDownLength/(2*scriptOfParentObject.gridSizeTopBottom)) - currRowInGrid*(topDownLength/scriptOfParentObject.gridSizeTopBottom);
					break;
				case 'Z':
					currPointInGridLocalCoords.z = (float) (bounds.center.z  + topDownLength/2 - topDownLength/(2*scriptOfParentObject.gridSizeTopBottom)) - currRowInGrid*(topDownLength/scriptOfParentObject.gridSizeTopBottom);
					break;
			}		
		}
		
		currPointInGridGlobalCoords = scriptOfParentObject.transform.TransformPoint(currPointInGridLocalCoords);
			
			
		/**
		**	DRAW THE LARGE GRID LINES
		**/
		for(int row = 0; row < scriptOfParentObject.gridSizeTopBottom; ++row)
		{
			Vector3 from = new Vector3(0,0,0);
			Vector3 to = new Vector3(0,0,0);
			switch(scriptOfParentObject.localUpAxis[0]){				
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
			switch(scriptOfParentObject.localAxisLeftRight[0]){
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
			
			switch(scriptOfParentObject.localAxisTopDown[0]){
				case 'X':
					from.x = (float) (bounds.center.x   - topDownLength/2 + topDownLength/(2*scriptOfParentObject.gridSizeTopBottom)) + row*(topDownLength/scriptOfParentObject.gridSizeTopBottom);
					to.x = (float) (bounds.center.x   - topDownLength/2 + topDownLength/(2*scriptOfParentObject.gridSizeTopBottom)) + row*(topDownLength/scriptOfParentObject.gridSizeTopBottom);
					
					break;
				case 'Y':
					from.y =(float) (bounds.center.y   - topDownLength/2 + topDownLength/(2*scriptOfParentObject.gridSizeTopBottom)) + row*(topDownLength/scriptOfParentObject.gridSizeTopBottom);
					to.y = (float) (bounds.center. y  - topDownLength/2 + topDownLength/(2*scriptOfParentObject.gridSizeTopBottom)) + row*(topDownLength/scriptOfParentObject.gridSizeTopBottom);
					break;
				case 'Z':
					from.z = (float) (bounds.center.z   - topDownLength/2 + topDownLength/(2*scriptOfParentObject.gridSizeTopBottom)) + row*(topDownLength/scriptOfParentObject.gridSizeTopBottom);
					to.z = (float) (bounds.center. z  - topDownLength/2 + topDownLength/(2*scriptOfParentObject.gridSizeTopBottom)) + row*(topDownLength/scriptOfParentObject.gridSizeTopBottom);
					break;
			}
			
		
			Vector3 fromGlobal = scriptOfParentObject.transform.TransformPoint(from);
			Vector3 toGlobal = scriptOfParentObject.transform.TransformPoint(to);
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
		}	

		/**
		**	DRAW THE SMALL GRID LINES
		**/
		for(int col = 0; col < scriptOfParentObject.gridSizeLeftRight; ++col)
		{
			Vector3 from = new Vector3(0,0,0);
			Vector3 to = new Vector3(0,0,0);
			switch(scriptOfParentObject.localUpAxis[0]){				
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
			switch(scriptOfParentObject.localAxisLeftRight[0]){
				case 'X':
					from.x = (float) (bounds.center.x  + -leftRightLength/2 + leftRightLength/(2*scriptOfParentObject.gridSizeLeftRight)) + col*(leftRightLength/scriptOfParentObject.gridSizeLeftRight);
					to.x = (float) (bounds.center. x + -leftRightLength/2 + leftRightLength/(2*scriptOfParentObject.gridSizeLeftRight)) + col*(leftRightLength/scriptOfParentObject.gridSizeLeftRight);
					break;
				case 'Y':
					from.y = (float) (bounds.center.y  + -leftRightLength/2 + leftRightLength/(2*scriptOfParentObject.gridSizeLeftRight)) + col*(leftRightLength/scriptOfParentObject.gridSizeLeftRight);
					to.y = (float) (bounds.center.y  + -leftRightLength/2 + leftRightLength/(2*scriptOfParentObject.gridSizeLeftRight)) + col*(leftRightLength/scriptOfParentObject.gridSizeLeftRight);
					break;
				case 'Z':
					from.z = (float) (bounds.center. z + -leftRightLength/2 + leftRightLength/(2*scriptOfParentObject.gridSizeLeftRight)) + col*(leftRightLength/scriptOfParentObject.gridSizeLeftRight);
					to.z = (float) (bounds.center.z  + -leftRightLength/2 + leftRightLength/(2*scriptOfParentObject.gridSizeLeftRight)) + col*(leftRightLength/scriptOfParentObject.gridSizeLeftRight);
					break;
			}
			switch(scriptOfParentObject.localAxisTopDown[0]){
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
		
			Vector3 fromGlobal = scriptOfParentObject.transform.TransformPoint(from);
			Vector3 toGlobal = scriptOfParentObject.transform.TransformPoint(to);
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
		}	
	}

	protected void drawNoGrid(){
		if(!isActive)
			return;		
		
		Vector3 scale = parentObject.transform.localScale;
		Mesh mesh = parentObject.GetComponent<MeshFilter>().mesh;		
		Bounds bounds = mesh.bounds;
		
		/**
		**	CALCULATE THE OBJECT POSITION IN THE GRID
		**/
		switch(scriptOfParentObject.localUpAxis[0]){				
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
		switch(scriptOfParentObject.localAxisLeftRight[0]){				
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
			switch(scriptOfParentObject.localAxisTopDown[0]){				
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
			switch(scriptOfParentObject.localAxisTopDown[0]){				
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
		
		currPointInGridGlobalCoords = scriptOfParentObject.transform.TransformPoint(currPointInGridLocalCoords);
	}	
}
