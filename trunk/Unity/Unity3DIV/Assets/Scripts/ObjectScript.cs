using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// This script is a data structure for a game object.
// Each object should have an instance of this script.
// Other scripts may use this script for manipulating objects.
public class ObjectScript : MonoBehaviour {
	public bool canMove;
	public bool canScale;
	public bool canRotate;
	public bool canBeStackedOn;
	
	public bool canBeDeleted;	
	public bool canBeCloned;	
	public int topViewDistance;	 //distance to move the camera upwards
	
	// for positioning other objects onto this one
	public string localUpAxis;
	public string localAxisLeftRight; // or: for dividing into columns
	public string localAxisTopDown; // or: for dividing into rows
	
	public int gridSizeLeftRight;
	public int gridSizeTopBottom;		
	public ArrayList children; // GameObjects
	// strings for the names of clonable objects that can be a child of this object
	public string[] possibleChildren; 
	
	// for positioning this object onto another one
	public int colInGrid; // == index on localAxisLeftRight
	public int rowInGrid; // == index on localAxisTopDown
	public GameObject parent; // if parent == gameObject -> no parent
	
	// for cloning
	public int cloneID;	// original item -> ID == 0
	private int lastUsedCloneID;
	private GameObject original;

	// Use this for initialization
	void Start () {
		canBeDeleted = false;	
		canBeCloned = true;	
		
		cloneID = 0;
		lastUsedCloneID = 0;
		original = gameObject;
		
		colInGrid = -1;
		rowInGrid = -1;
		children = new ArrayList();
		parent = gameObject;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void changeToTopview(){
		Vector3 pos = transform.position;
		pos.y += topViewDistance;
		Vector3 rot = transform.eulerAngles;	

		Camera.main.transform.position = pos;	
		Camera.main.transform.rotation = Quaternion.identity;	
		float angle = 0.0f;
		
		Vector3 rotp = new Vector3(90,0,0);
		Camera.main.transform.Rotate (rotp, Space.World);
		
		// make sure topDown en leftRight are correctly shown
		switch(localAxisLeftRight[0]){
			case 'X':
				angle = Vector3.Angle(Camera.main.transform.right, transform.right);
				break;
			case 'Y':
				angle = Vector3.Angle(Camera.main.transform.right, transform.up);
				break;
			case 'Z':
				angle = Vector3.Angle(Camera.main.transform.right, transform.forward);
				break;
		}		
				
		rotp = new Vector3(0,angle,0);
		Camera.main.transform.Rotate (rotp, Space.World);
		
		bool flip = false;
		switch(localAxisLeftRight[0]){
			case 'X':
				flip = Math.Abs(Vector3.Angle(Camera.main.transform.right, transform.right)) > 2.0f;
				break;
			case 'Y':
				flip = Math.Abs(Vector3.Angle(Camera.main.transform.right, transform.up)) > 2.0f;
				break;
			case 'Z':
				flip = Math.Abs(Vector3.Angle(Camera.main.transform.right, transform.forward)) > 2.0f;
				break;
		}
		if(flip){
			Camera.main.transform.Rotate (-2*rotp, Space.World);
		}
	}	
	
	public Dictionary<string, bool> getObjectPossibilities(){
		Dictionary<string, bool> result = new Dictionary<string, bool>();
		result["move"] = canMove;
		result["rotate"] = canRotate;
		result["scale"] = canScale;
		result["stackOn"] = canBeStackedOn;
		result["clone"] = canBeCloned;
		result["delete"] = canBeDeleted;
		
		return result;
	}		
	
	public bool isGridCellAvailable(int pcolInGrid, int prowInGrid){
		if(canBeStackedOn){
			 foreach (GameObject item in children) {
				ObjectScript script = (ObjectScript) item.GetComponent("ObjectScript");	
				if(script.colInGrid == pcolInGrid && script.rowInGrid == prowInGrid)
					return false;
			}
			
			return true;
		}	
		else return false;
	}
	
	public void delete(){
		foreach (GameObject child in children) {
			ObjectScript script = (ObjectScript) child.GetComponent("ObjectScript");	
			script.delete();
		}	
		gameObject.active = false;
	}
	
	public void removeChildFromGridCell(int pcolInGrid, int prowInGrid){
		GameObject toBeRemoved = gameObject;
		bool found = false;
		
		foreach (GameObject item in children) {
			ObjectScript script = (ObjectScript) item.GetComponent("ObjectScript");	
			if(script.colInGrid == pcolInGrid && script.rowInGrid == prowInGrid){
				toBeRemoved = item;
				found = true;
			}
		}
		
		if(found)
			children.Remove(toBeRemoved);
	}
	
	public void removeChildFromGrid(GameObject child){
		children.Remove(child);
	}
	
	public GameObject getChildFromGridCell(int pcolInGrid, int prowInGrid){
		foreach (GameObject item in children) {
			ObjectScript script = (ObjectScript) item.GetComponent("ObjectScript");	
			if(script.colInGrid == pcolInGrid && script.rowInGrid == prowInGrid){
				return item;
			}
		}
		
		return new GameObject("dummy");
	}
	
	public void addChild(GameObject child){
		child.transform.parent = transform;
	}	
	
	public void setParent(GameObject parent_){
		transform.parent = parent_.transform;
	}
	
	public void setParentTransform(Transform parent_){
		transform.parent = parent_;
	}
	
	public string clone(Vector3 pos, Quaternion rot){
		GameObject clone = (GameObject) Instantiate(gameObject, pos, rot);
		ObjectScript cloneScript = (ObjectScript) clone.GetComponent("ObjectScript");
		cloneScript.setOriginator(gameObject);
		
		return clone.name;
	}
	
	private void setOriginator(GameObject orig){
		ObjectScript origScript = (ObjectScript) orig.GetComponent("ObjectScript");	
		canMove = origScript.canMove;
		canScale = origScript.canScale;
		canRotate = origScript.canRotate;
		canBeStackedOn = origScript.canBeStackedOn;
		topViewDistance = origScript.topViewDistance;
	
		localUpAxis = origScript.localUpAxis;
		localAxisLeftRight = origScript.localAxisLeftRight; 
		localAxisTopDown = origScript.localAxisTopDown; 
		gridSizeLeftRight = origScript.gridSizeLeftRight;
		gridSizeTopBottom = origScript.gridSizeTopBottom;		
		children = new ArrayList();
		possibleChildren = origScript.possibleChildren; 
		
		colInGrid = -1;
		rowInGrid = -1; 
		
		canBeDeleted = true;
		canBeCloned = false;
		
		cloneID = origScript.lastUsedCloneID + 1;
		origScript.lastUsedCloneID = cloneID;
		original = orig;
		
		name = orig.name + cloneID;
	}	
}
