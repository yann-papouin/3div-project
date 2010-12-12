using UnityEngine;
using System.Collections;

public class VerwijderScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void DeleteObject(GameObject item){
		ObjectScript script = (ObjectScript) item.GetComponent("ObjectScript");			
		script.delete();		
	}
}