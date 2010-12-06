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
	
	// for positioning other objects onto this one
	public int gridSizeSmallSide;
	public int gridSizeLargeSide;		
	public ArrayList children; // GameObjects
	public string[] possibleChildren; // strings for the names
	
	// for positioning this object onto another one
	public int posInGridSmallSide;
	public int posInGridLargeSide;
	public GameObject parent; // if parent == gameObject -> no parent
	
	// for cloning
	public int cloneID;	// original item -> ID == 0
	private int lastUsedCloneID;
	private GameObject original;

	// Use this for initialization
	void Start () {
		canMove = false;
		canScale = false;
		canRotate = false;
		canBeStackedOn = false;
	
		canBeDeleted = false;	
		canBeCloned = true;	
		
		cloneID = 0;
		lastUsedCloneID = 0;
		original = gameObject;
		
		gridSizeSmallSide = 0;
		gridSizeLargeSide = 0;
		posInGridLargeSide = 0;
		posInGridSmallSide = 0;
		children = new ArrayList();
		parent = gameObject;
		possibleChildren = new string[50];
	}
	
	// Update is called once per frame
	void Update () {
	
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
	
	public bool isGridCellAvailable(int posLargeSide, int posSmallSide){
		if(canBeStackedOn){
			 foreach (GameObject item in children) {
				ObjectScript script = (ObjectScript) item.GetComponent("ObjectScript");	
				if(script.posInGridLargeSide == posLargeSide && script.posInGridSmallSide == posInGridSmallSide)
					return false;
			}
			
			return true;
		}	
		else return false;
	}
	
	public void removeChildFromGridCell(int posLargeSide, int posSmallSide){
		GameObject toBeRemoved = gameObject;
		bool found = false;
		
		foreach (GameObject item in children) {
			ObjectScript script = (ObjectScript) item.GetComponent("ObjectScript");	
			if(script.posInGridLargeSide == posLargeSide && script.posInGridSmallSide == posInGridSmallSide){
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
	
	public GameObject getChildFromGridCell(int posLargeSide, int posSmallSide){
		foreach (GameObject item in children) {
			ObjectScript script = (ObjectScript) item.GetComponent("ObjectScript");	
			if(script.posInGridLargeSide == posLargeSide && script.posInGridSmallSide == posInGridSmallSide){
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
	
	public void clone(Vector3 pos, Quaternion rot){
		GameObject clone = (GameObject) Instantiate(gameObject, pos, rot);
		ObjectScript cloneScript = (ObjectScript) clone.GetComponent("ObjectScript");
		cloneScript.setOriginator(gameObject);
	}
	
	private void setOriginator(GameObject orig){
		ObjectScript origScript = (ObjectScript) orig.GetComponent("ObjectScript");	
		canMove = origScript.canMove;
		canScale = origScript.canScale;
		canRotate = origScript.canRotate;
		canBeStackedOn = origScript.canBeStackedOn;
		
		gridSizeSmallSide = origScript.gridSizeSmallSide;
		gridSizeLargeSide = origScript.gridSizeLargeSide;
		posInGridLargeSide = origScript.posInGridLargeSide;
		posInGridSmallSide = origScript.posInGridSmallSide;
		
		canBeDeleted = true;
		canBeCloned = false;
		
		cloneID = origScript.lastUsedCloneID + 1;
		origScript.lastUsedCloneID = cloneID;
		original = orig;
		
		name = orig.name + cloneID;
	}	
}
