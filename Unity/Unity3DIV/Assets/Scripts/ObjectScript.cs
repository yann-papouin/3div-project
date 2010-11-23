using UnityEngine;
using System.Collections;

public class ObjectScript : MonoBehaviour {
	public bool canMove;
	public bool canScale;
	public bool canRotate;
	
	public bool canBeDeleted;	
	public bool canBeCloned;	
	
	// for cloning
	public int cloneID;	// original items -> ID == 0
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
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	Dictionary getObjectPossibilities(){
		Dictionary result = new Dictionary<string, bool>;
		result["move"] = canMove;
		result["rotate"] = canRotate;
		result["scale"] = canScale;
		result["stackOn"] = canBeStackedOn;
		result["clone"] = canBeCloned;
		result["delete"] = canBeDeleted;
		
		return result;
	}		
	
	public void addChild(GameObject child){
		child.transform.parent = transform;
	}
	
	public void setParent(GameObject parent_){
		transform.parent = parent_.transform;
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
		
		canBeDeleted = true;
		canBeCloned = false;
		
		cloneID = origScript.lastUsedCloneID + 1;
		origScript.lastUsedCloneID = cloneID;
		original = orig;
		
		name = orig.name + cloneID;
	}
}
