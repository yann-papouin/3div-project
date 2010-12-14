// Author: Sibrand Staessens

using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void test(){
		GameObject trex = GameObject.Find("trex_ori");
		ObjectScript script = (ObjectScript)trex.GetComponent("ObjectScript");
		script.clone();

		GameObject clone = GameObject.Find("trex_ori1");
		GameObject tafel = GameObject.Find("tafel");
		ObjectScript cloneScript = (ObjectScript) clone.GetComponent("ObjectScript");
	}
	
	public void testStack(){
		GameObject tafel = GameObject.Find("Tafel");
		ObjectScript oscript = (ObjectScript)tafel.GetComponent("ObjectScript");
		
		GameObject ic = GameObject.Find("InputController");
		StackScript sscript = (StackScript)ic.GetComponent("StackScript");
		sscript.Begin(tafel);
	}
	
	public void testParentMove(){
		GameObject tafel = GameObject.Find("TestCube");
		tafel.transform.Translate(Vector3.forward * 2, Space.World);
	}
	public void testParentRotate(){
		GameObject tafel = GameObject.Find("Tafel");
		tafel.transform.RotateAround(tafel.transform.position, Vector3.up, 5);
	}
	
	public void testMove(){
		GameObject tafel = GameObject.Find("schaal1");
		MoveScript script = (MoveScript) gameObject.GetComponent("MoveScript");
		script.Begin(tafel);
	}
	
	public void testChangeParent(){
		GameObject tafel = GameObject.Find("schaal1");
		MoveScript script = (MoveScript) gameObject.GetComponent("MoveScript");
		script.changeStackParent(GameObject.Find("schaal1"), GameObject.Find("TestCube"));
		
	}
}
