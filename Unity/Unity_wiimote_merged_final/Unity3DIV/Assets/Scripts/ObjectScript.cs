// Author: Sibrand Staessens

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

// This script is a data structure for a game object.
// Each object should have an instance of this script.
// Other scripts may use this script for manipulating objects.
public class ObjectScript : MonoBehaviour {
	public bool canMove = true;
	public bool canScale = true;
	public bool canRotate = true;
	public bool canBeStackedOn = true;
	
	public bool canBeDeleted = false;	// only cloned objects can be deleted
	public bool canBeCloned = true; // only original objects can be cloned	
	public int topViewDistance = 15;	 //distance to move the camera upwards
	
	// for positioning other objects onto this one
	public string localUpAxis = "Y";
	public string localAxisLeftRight = "X"; // or: for dividing into columns
	public string localAxisTopDown = "Z"; // or: for dividing into rows
	
	// strings for the names of clonable objects that can be a child of this object
	public string[] possibleChildren; 	
	public ArrayList children; // GameObjects	

		
	// for positioning this object onto another one with the help of a grid
	public int colInGrid; // == index on localAxisLeftRight, this = child
	public int rowInGrid; // == index on localAxisTopDown, this = child
	public int gridSizeLeftRight; // this = parent
	public int gridSizeTopBottom;// this = parent
	
	// for positioning this object onto another one without the help of a grid
	public float posLeftRight; // == index on localAxisLeftRight, this = child
	public float posTopDown; // == index on localAxisTopDown, this = child
	
	// for cloning
	public int cloneID;	// original item -> ID == 0
	private int lastUsedCloneID;
	private GameObject original = null;	
	
	// Use this for initialization
	void Start () {		
		cloneID = 0;
		lastUsedCloneID = 0;
		children = new ArrayList();
		origColor = renderer.material.color;
		isHoovered = false;
		isSelected = false;
		buildObjectPossibilities();
	}
	
	// Update is called once per frame
	private Color origColor;
	public Color selectColor = Color.white;
	private bool isHoovered, isSelected;
	void Update () {
		;
	}
	
	public void selectThis(bool select){
		if(gameObject.GetComponent<Renderer>()){		
			isSelected = select;
			
			if(isSelected){
				Color c = new Color();
				c.a = Math.Max(1.0f, origColor.a + 0.5f);
				c.r = Math.Max(1.0f, origColor.r + 1.0f);
				c.g = Math.Max(1.0f, origColor.g + 1.0f);
				c.b = Math.Max(1.0f, origColor.b + 1.0f);
				renderer.material.color = c;
			}
			else
				renderer.material.color = origColor;
		}
	}
	
	public void hooverThis(bool select){	
		if(gameObject.GetComponent<Renderer>()){		
			isHoovered = select;
			
			if(isHoovered && !isSelected){
				Color c = new Color();
				c.a = Math.Max(1.0f, origColor.a + 0.5f);
				c.r = Math.Max(1.0f, origColor.r + 0.5f);
				c.g = Math.Max(1.0f, origColor.g + 0.5f);
				c.b = Math.Max(1.0f, origColor.b + 0.5f);
				renderer.material.color = c;
			}
			else if(!isHoovered && !isSelected)
				renderer.material.color = origColor;
		}
	}
	
	private string[] objectPossibilities = {"","","","","",""};
	private void buildObjectPossibilities(){
		int i = 0;
		if(canMove){
			objectPossibilities[i] = "Move";
			i++;
		}
		if(canRotate){
		objectPossibilities[i] = "Rotate";
			i++;
		}
		if(canScale){
			objectPossibilities[i] = "Scale";
			i++;
		}
		if(canBeCloned){
			objectPossibilities[i] = "Clone";
			i++;
		}
		if(canBeStackedOn){
			objectPossibilities[i] = "Add";
			i++;
		}
		if(canBeDeleted){
			objectPossibilities[i] = "Delete";
		}
	}
	public string[] getObjectPossibilities(){
		return objectPossibilities;
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
	
	public void addChild(GameObject child, float leftright, float topdown){
		if(child){
			ObjectScript script = (ObjectScript) child.GetComponent("ObjectScript");	
		
			child.transform.parent = transform;
			children.Add(child);
			script.posLeftRight = leftright;
			script.posTopDown = topdown;
		}
	}	

	public void addChildInGrid(GameObject child, int col, int row){
		if(child){
			ObjectScript script = (ObjectScript) child.GetComponent("ObjectScript");	
			
			child.transform.parent = transform;
			script.colInGrid = col;
			script.rowInGrid = row;
			children.Add(child);
		}
	}	
	
	public void detachChild(GameObject child){
		if(child){
			ObjectScript script = (ObjectScript) child.GetComponent("ObjectScript");	

			children.Remove(child);
			child.transform.parent = null;
		}
	}
	
	public GameObject getParent(){
		if(transform.parent){
			Transform t = transform.parent;
			return t.gameObject;
		} else 
			return null;
	}
	
	public void delete(){
	//	ObjectScript[] ch = gameObject.gameObject.GetComponentsInChildren<ObjectScript>();
//			foreach(ObjectScript script in ch ){	
//				script.delete();
//			}			
//		gameObject.active = false;
Destroy(gameObject);
	}	
	
	public string clone(){
		GameObject clone = (GameObject) Instantiate(gameObject, gameObject.transform.position, gameObject.transform.rotation);
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
		
		colInGrid = origScript.colInGrid;
		rowInGrid = origScript.rowInGrid;
		
		canBeDeleted = true;
		canBeCloned = false;
		
		cloneID = origScript.lastUsedCloneID + 1;
		origScript.lastUsedCloneID = cloneID;
		original = orig;
		
		name = orig.name + cloneID;
		
		buildObjectPossibilities();//Herbouw de keuzearray
	}	
}
