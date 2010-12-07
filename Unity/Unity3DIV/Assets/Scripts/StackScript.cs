using UnityEngine;
using System.Collections;

public class StackScript : MonoBehaviour {
	private GameObject selectedObject;
	private ObjectScript scriptOfSelectedObject;	
	private bool isActive;	
	private ArrayList lines;
	private int currentIndexOfPossibleStackedObject;
	private GameObject currentPossibleStackedObject;
	private ObjectScript scriptOfCurrentPossibleStackedObject;
	private int currPosInGridSmallSide;
	private int currPosInGridLargeSide;
	private Vector3 currPointInGridGlobalCoords, currPointInGridLocalCoords;

	// Use this for initialization
	void Start () {
		isActive = false;
		lines = new ArrayList();
		currPointInGridGlobalCoords = new Vector3(0,0,0);
		currPointInGridLocalCoords = new Vector3(0,0,0);
	}	
	
	public void Begin(GameObject carrier){
		isActive = true;
		selectedObject = carrier;
		scriptOfSelectedObject = (ObjectScript) selectedObject.GetComponent("ObjectScript");	
		currentIndexOfPossibleStackedObject = 0;

		if(!scriptOfSelectedObject || scriptOfSelectedObject.possibleChildren.Length == 0 || scriptOfSelectedObject.possibleChildren[currentIndexOfPossibleStackedObject].Length == 0){
			isActive = false;
			return;
		}
		
		bool found = false;
		for(currPosInGridSmallSide = 0; currPosInGridSmallSide < scriptOfSelectedObject.gridSizeSmallSide && !found; ++currPosInGridSmallSide){
			for(currPosInGridLargeSide = 0; currPosInGridLargeSide < scriptOfSelectedObject.gridSizeLargeSide && !found; ++currPosInGridLargeSide)
				if(scriptOfSelectedObject.isGridCellAvailable(currPosInGridLargeSide, currPosInGridSmallSide) == true)
					found = true;			
		}			
		
		if(found == false)
			Abort();
		
		Debug.Log("posL: " + currPosInGridLargeSide + ", posS: " + currPosInGridSmallSide);		
		drawGrid();		
		
		setPossibleStackedObject(scriptOfSelectedObject.possibleChildren[currentIndexOfPossibleStackedObject]);
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
	
	public void chooseCurrentObject(){
		if(!isActive)
			return;
	}
	
	public void goToNextPosition(){
		if(!isActive)
			return;
			
		bool found = false;
		bool first = true;
		for(; currPosInGridSmallSide < scriptOfSelectedObject.gridSizeSmallSide && !found; ++currPosInGridSmallSide){
			if(!first)
				currPosInGridLargeSide = 0;
			else first = false;
			for(; currPosInGridLargeSide < scriptOfSelectedObject.gridSizeLargeSide && !found; ++currPosInGridLargeSide)
				if(scriptOfSelectedObject.isGridCellAvailable(currPosInGridLargeSide, currPosInGridSmallSide) == true)
					found = true;			
		}	
		
		if(!found){
			for(currPosInGridSmallSide = 0; currPosInGridSmallSide < scriptOfSelectedObject.gridSizeSmallSide && !found; ++currPosInGridSmallSide){
				for(currPosInGridLargeSide = 0; currPosInGridLargeSide < scriptOfSelectedObject.gridSizeLargeSide && !found; ++currPosInGridLargeSide)
					if(scriptOfSelectedObject.isGridCellAvailable(currPosInGridLargeSide, currPosInGridSmallSide) == true)
						found = true;			
			}	
		}
			
		drawGrid();
		setPossibleStackedObject(scriptOfSelectedObject.possibleChildren[currentIndexOfPossibleStackedObject]);
	}	
	
	// stacking manipulation = finished
	public void choosePosition(){
		if(!isActive)
			return;
		foreach( GameObject obj in lines){
			Destroy(obj);
		}
		lines.Clear();
		isActive = false;
	}
	
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
	}
	
	// Update is called once per frame
	void Update () {
		if(isActive){			
			;
		}
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
			
		/**
		**	REMOVE THE PREVIOUSLY CALCULATED LINES
		**/	
		foreach( GameObject obj in lines){
			Destroy(obj);
		}
		lines.Clear();
	
		char largeSideInLocalAxes = getLargestHorizontalSide();
		char smallSideInLocalAxes = getSmallestHorizontalSide();
		char localY = getLocalAxisSameAsGlobalY();		
		
		Vector3 scale = selectedObject.transform.localScale;
		Mesh mesh = selectedObject.GetComponent<MeshFilter>().mesh;		
		Bounds bounds = mesh.bounds;
		Vector3 size = bounds.size;
		float smallLength = 0;
		float largeLength = 0;
		float height = 0;
		
		switch(smallSideInLocalAxes){			
			case 'x':
				smallLength = size.x;//*scale.x;
				break;
			case 'y':
				smallLength = size.y;//*scale.y;
				break;
			case 'z':
				smallLength = size.z;//*scale.z;
				break;
		}
		switch(largeSideInLocalAxes){			
			case 'x':
				largeLength = size.x;//*scale.x;
				break;
			case 'y':
				largeLength = size.y;//*scale.y;
				break;
			case 'z':
				largeLength = size.z;//*scale.z;
				break;
		}
		switch(localY){			
			case 'x':
				height = size.x;//*scale.x;
				break;
			case 'y':
				height = size.y;//*scale.y;
				break;
			case 'z':
				height = size.z;//*scale.z;
				break;
		}
		
		/**
		**	CALCULATE THE OBJECT POSITION IN THE GRID
		**/
		switch(localY){				
			case 'x':
				currPointInGridLocalCoords.x = (float) height/2;
				break;
			case 'y':
				currPointInGridLocalCoords.y = (float) height/2;
				break;
			case 'z':
				currPointInGridLocalCoords.z = (float) height/2;
				break;
		}
		switch(smallSideInLocalAxes){				
			case 'x':
				currPointInGridLocalCoords.x = (float) (-smallLength/2 + smallLength/(2*scriptOfSelectedObject.gridSizeSmallSide)) + currPosInGridSmallSide*(smallLength/scriptOfSelectedObject.gridSizeSmallSide);
				break;
			case 'y':
				currPointInGridLocalCoords.y = (float) (-smallLength/2 + smallLength/(2*scriptOfSelectedObject.gridSizeSmallSide)) + currPosInGridSmallSide*(smallLength/scriptOfSelectedObject.gridSizeSmallSide);
				break;
			case 'z':
				currPointInGridLocalCoords.z = (float) (-smallLength/2 + smallLength/(2*scriptOfSelectedObject.gridSizeSmallSide)) + currPosInGridSmallSide*(smallLength/scriptOfSelectedObject.gridSizeSmallSide);
				break;
		}
		switch(largeSideInLocalAxes){				
			case 'x':
				currPointInGridLocalCoords.x = (float) (-largeLength/2 + largeLength/(2*scriptOfSelectedObject.gridSizeLargeSide)) + currPosInGridLargeSide*(largeLength/scriptOfSelectedObject.gridSizeLargeSide);
				break;
			case 'y':
				currPointInGridLocalCoords.y = (float) (-largeLength/2 + largeLength/(2*scriptOfSelectedObject.gridSizeLargeSide)) + currPosInGridLargeSide*(largeLength/scriptOfSelectedObject.gridSizeLargeSide);
				break;
			case 'z':
				currPointInGridLocalCoords.z = (float) (-largeLength/2 + largeLength/(2*scriptOfSelectedObject.gridSizeLargeSide)) + currPosInGridLargeSide*(largeLength/scriptOfSelectedObject.gridSizeLargeSide);
				break;
		}

		currPointInGridGlobalCoords = selectedObject.transform.TransformPoint(currPointInGridLocalCoords);
			
			
		/**
		**	DRAW THE LARGE GRID LINES
		**/
		for(int row = 0; row < scriptOfSelectedObject.gridSizeSmallSide; ++row)
		{
			Vector3 from = new Vector3(0,0,0);
			Vector3 to = new Vector3(0,0,0);
			switch(localY){				
				case 'x':
					from.x = (float) height/2;
					to.x = (float)  height/2;
					break;
				case 'y':
					from.y = (float) height/2;
					to.y = (float) height/2;
					break;
				case 'z':
					from.z = (float) height/2;
					to.z = (float) height/2;
					break;
			}
			switch(largeSideInLocalAxes){
				case 'x':
					from.x = (float) -largeLength/2;
					to.x = (float) largeLength/2;
					break;
				case 'y':
					from.y = (float) -largeLength/2;
					to.y = (float) largeLength/2;
					break;
				case 'z':
					from.z = (float) -largeLength/2;
					to.z = (float) largeLength/2;
					break;
			}
			switch(smallSideInLocalAxes){
				case 'x':
					from.x = (float) (-smallLength/2 + smallLength/(2*scriptOfSelectedObject.gridSizeSmallSide)) + row*(smallLength/scriptOfSelectedObject.gridSizeSmallSide);
					to.x = (float) (-smallLength/2 + smallLength/(2*scriptOfSelectedObject.gridSizeSmallSide)) + row*(smallLength/scriptOfSelectedObject.gridSizeSmallSide);
					break;
				case 'y':
					from.y =(float) (-smallLength/2 + smallLength/(2*scriptOfSelectedObject.gridSizeSmallSide)) + row*(smallLength/scriptOfSelectedObject.gridSizeSmallSide);
					to.y = (float) (-smallLength/2 + smallLength/(2*scriptOfSelectedObject.gridSizeSmallSide)) + row*(smallLength/scriptOfSelectedObject.gridSizeSmallSide);
					break;
				case 'z':
					from.z = (float) (-smallLength/2 + smallLength/(2*scriptOfSelectedObject.gridSizeSmallSide)) + row*(smallLength/scriptOfSelectedObject.gridSizeSmallSide);
					to.z = (float) (-smallLength/2 + smallLength/(2*scriptOfSelectedObject.gridSizeSmallSide)) + row*(smallLength/scriptOfSelectedObject.gridSizeSmallSide);
					break;
			}
		
			Vector3 fromGlobal = selectedObject.transform.TransformPoint(from);
			Vector3 toGlobal = selectedObject.transform.TransformPoint(to);
			fromGlobal.y += 0.2f;
			toGlobal.y += 0.2f;
			
			Color c1 = Color.red;
			Color c2 = Color.red;
			GameObject line = new GameObject();
			LineRenderer rend = line.AddComponent<LineRenderer>();
			rend.material = new Material(Shader.Find("Particles/Additive"));
			rend.useWorldSpace = true;
			rend.SetWidth(0.08f, 0.08f);
			rend.SetColors(c1, c2);
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
		for(int col = 0; col < scriptOfSelectedObject.gridSizeLargeSide; ++col)
		{
			Vector3 from = new Vector3(0,0,0);
			Vector3 to = new Vector3(0,0,0);
			switch(localY){				
				case 'x':
					from.x = (float) height/2;
					to.x = (float)  height/2;
					break;
				case 'y':
					from.y = (float) height/2;
					to.y = (float) height/2;
					break;
				case 'z':
					from.z = (float) height/2;
					to.z = (float) height/2;
					break;
			}
			switch(largeSideInLocalAxes){
				case 'x':
					from.x = (float) (-largeLength/2 + largeLength/(2*scriptOfSelectedObject.gridSizeLargeSide)) + col*(largeLength/scriptOfSelectedObject.gridSizeLargeSide);
					to.x = (float) (-largeLength/2 + largeLength/(2*scriptOfSelectedObject.gridSizeLargeSide)) + col*(largeLength/scriptOfSelectedObject.gridSizeLargeSide);
					break;
				case 'y':
					from.y = (float) (-largeLength/2 + largeLength/(2*scriptOfSelectedObject.gridSizeLargeSide)) + col*(largeLength/scriptOfSelectedObject.gridSizeLargeSide);
					to.y = (float) (-largeLength/2 + largeLength/(2*scriptOfSelectedObject.gridSizeLargeSide)) + col*(largeLength/scriptOfSelectedObject.gridSizeLargeSide);
					break;
				case 'z':
					from.z = (float) (-largeLength/2 + largeLength/(2*scriptOfSelectedObject.gridSizeLargeSide)) + col*(largeLength/scriptOfSelectedObject.gridSizeLargeSide);
					to.z = (float) (-largeLength/2 + largeLength/(2*scriptOfSelectedObject.gridSizeLargeSide)) + col*(largeLength/scriptOfSelectedObject.gridSizeLargeSide);
					break;
			}
			switch(smallSideInLocalAxes){
				case 'x':
					from.x = (float) -smallLength/2;
					to.x = (float) smallLength/2;
					break;
				case 'y':
					from.y =(float) -smallLength/2;
					to.y = (float) smallLength/2;
					break;
				case 'z':
					from.z = (float) -smallLength/2;
					to.z = (float) smallLength/2;
					break;
			}
		
			Vector3 fromGlobal = selectedObject.transform.TransformPoint(from);
			Vector3 toGlobal = selectedObject.transform.TransformPoint(to);
			fromGlobal.y += 0.2f;
			toGlobal.y += 0.2f;
			
			Color c1 = Color.green;
			Color c2 = Color.green;
			GameObject line = new GameObject();
			LineRenderer rend = line.AddComponent<LineRenderer>();
			rend.material = new Material(Shader.Find("Particles/Additive"));
			rend.useWorldSpace = true;
			rend.SetWidth(0.08f, 0.08f);
			rend.SetColors(c1, c2);
			rend.SetVertexCount(2);
			rend.SetPosition(0, fromGlobal);
			rend.SetPosition(1, toGlobal);
			lines.Add(line);
			
			
			//Debug.Log("From: " + fromGlobal.x + ", "  + fromGlobal.y + ", "  + fromGlobal.z);
			//Debug.Log("To: " + toGlobal.x + ", "  + toGlobal.y + ", "  + toGlobal.z);
		}	
	}
	
	private char getLocalAxisSameAsGlobalY(){	
		if(selectedObject.transform.up == Vector3.up) // local y is same as global y
			return 'y';
		if(selectedObject.transform.forward == Vector3.up) // local z is same as global y
			return 'z';
		if(selectedObject.transform.right == Vector3.up) // local x is same as global y
			return 'x';
			
		return 'y';
	}
	
	private char getLargestHorizontalSide(){
		Vector3 scale = selectedObject.transform.localScale;
		Mesh mesh = selectedObject.GetComponent<MeshFilter>().mesh;		
		Bounds bounds = mesh.bounds;
		Vector3 size = bounds.size;
	
		if(selectedObject.transform.up == Vector3.up) // local y is same as global y
		{
			if(size.x*scale.x > size.z*scale.z)
					return 'x';
			else 
				return 'z';
		}
		if(selectedObject.transform.forward == Vector3.up) // local z is same as global y
		{
			if(size.x*scale.x > size.y*scale.y)
					return 'x';
			else 
				return 'y';
		}
		if(selectedObject.transform.right == Vector3.up) // local x is same as global y
		{
			if(size.z*scale.z > size.y*scale.y)
					return 'z';
			else 
				return 'y';
		}
		
		return 'y';
	}
	
	private char getSmallestHorizontalSide(){
		Vector3 scale = selectedObject.transform.localScale;
		Mesh mesh = selectedObject.GetComponent<MeshFilter>().mesh;		
		Bounds bounds = mesh.bounds;
		Vector3 size = bounds.size;
	
		if(selectedObject.transform.up == Vector3.up) // local y is same as global y
		{
			if(size.x*scale.x > size.z*scale.z)
					return 'z';
			else 
				return 'x';
		}
		if(selectedObject.transform.forward == Vector3.up) // local z is same as global y
		{
			if(size.x*scale.x > size.y*scale.y)
					return 'y';
			else 
				return 'x';
		}
		if(selectedObject.transform.forward == Vector3.up) // local x is same as global y
		{
			if(size.z*scale.z > size.y*scale.y)
					return 'y';
			else 
				return 'z';
		}
		
		return 'y';
	}

}
