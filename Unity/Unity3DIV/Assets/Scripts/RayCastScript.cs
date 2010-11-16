using UnityEngine;
using System.Collections;

public class RayCastScript: MonoBehaviour {

	public float distance = 100.0F;
	public Vector3 direction = Vector3.forward;
	public int maxarraysize = 10;
	public bool debug = true;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public GameObject getTargetObjects(Vector3 startpoint, Camera cam){
		//GameObject[] colliderobjects = new GameObject[10];
		//RaycastHit[] hits = Physics.raycastAll(startpoint, direction, distance);
		//return colliderobjects;
		GameObject firstCollidedObject = null;
		RaycastHit hit;
		Ray fireray;
		//Startpoint is in het geval van guitexture de transform.position
		//bool success = Physics.Raycast(startpoint, direction, out hit, distance); //Werkt niet echt goed
		fireray = cam.ScreenPointToRay(startpoint);
		bool success = Physics.Raycast(fireray, out hit, distance);
		if (success){
			firstCollidedObject = hit.collider.gameObject;
			if(firstCollidedObject != null)
				print("There was a hit on " + firstCollidedObject.name);
			else
				print("Debug further");
		}
		if (debug)
		{
			drawRay5Sec(startpoint, hit.point);
		}
		
		return firstCollidedObject;
	}
	
	private IEnumerator drawRay5Sec(Vector3 start, Vector3 end){
		float duration = 10;
		do{
			duration -= Time.deltaTime;
			Debug.DrawLine(start, end, Color.red);
			yield return 0;
		}while (duration > 0);
	}
}
