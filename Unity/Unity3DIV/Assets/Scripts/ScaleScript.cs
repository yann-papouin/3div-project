using UnityEngine;
using System.Collections;

public class ScaleScript : MonoBehaviour {
	public GameObject selectedObject;
	public float scaleStep = 0.001f;
	
	private bool drawFeedback = false;
	private char gekozenAsGlobaal = ' ';
	
	private GameObject line;
	private LineRenderer rend;
	public	Color c1 = Color.blue;
	private Color c2 = Color.blue;
	private float animatieScale = 1.0f;
	private bool isAnimatieGroter = true;


	
	// Use this for initialization
	void Start () {			
			c2 = c1;
			line = new GameObject();
			rend = line.AddComponent<LineRenderer>();
			rend.material = new Material(Shader.Find("Particles/Additive"));
			rend.useWorldSpace = true;
			rend.SetWidth(0.5f, 0.5f);
			rend.SetColors(c1, c2);
			rend.SetVertexCount(2);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	public void SetDrawFeedback(bool hasDrawFeedback, char gekozenAs){
				
		if (hasDrawFeedback){
					
			//Gekozen as transformeren naar globaal assenstelsel
			if (gekozenAs == 'x'){
				gekozenAsGlobaal = getLocalAxisSameAsGlobalX();
			} else if (gekozenAs == 'y'){
				gekozenAsGlobaal = getLocalAxisSameAsGlobalY();
			} else if (gekozenAs == 'z') {
					gekozenAsGlobaal = getLocalAxisSameAsGlobalZ();
			}
			
			drawLine();
		}	
		
		drawFeedback = hasDrawFeedback;	
		line.active = drawFeedback;
	}
	
	private void drawLine(){
			Vector3 from = new Vector3(0,0,0);
			Vector3 to = new Vector3(0,0,0);
			
			from.x = getLineStartXPositie();
			from.y = getLineStartYPositie();
			from.z = getLineStartZPositie();
			
			to.x = getLineEndXPositie();
			to.y = getLineEndYPositie();
			to.z = getLineEndZPositie();	
			
			Vector3 fromGlobal = selectedObject.transform.TransformPoint(from);
			Vector3 toGlobal = selectedObject.transform.TransformPoint(to);		
					
			rend.SetPosition(0, fromGlobal);
			rend.SetPosition(1, toGlobal);
			
			animatieScale = 1.0f;
	}
	
	

		
	private float getLineStartXPositie(){
		Vector3 scale = selectedObject.transform.localScale;
		Mesh mesh = selectedObject.GetComponent<MeshFilter>().mesh;		
		Bounds bounds = mesh.bounds;
		
		if(gekozenAsGlobaal == 'x'){
			return bounds.min.x - 1 ;
		} 
			
		if(gekozenAsGlobaal == 'y'){
			return bounds.center.x;
		}
			
		if(gekozenAsGlobaal == 'z'){
			return bounds.center.x;
		}	
		return 0;
	}
	
	private float getLineStartZPositie(){
		Vector3 scale = selectedObject.transform.localScale;
		Mesh mesh = selectedObject.GetComponent<MeshFilter>().mesh;		
		Bounds bounds = mesh.bounds;
		
		if(gekozenAsGlobaal == 'x'){
			return bounds.center.z ;
		} 
			
		if(gekozenAsGlobaal == 'y'){
			return bounds.center.z;
		}
			
		if(gekozenAsGlobaal == 'z'){
			return bounds.min.z -1;
		}	
		return 0;
	}
	
		private float getLineStartYPositie(){
		Vector3 scale = selectedObject.transform.localScale;
		Mesh mesh = selectedObject.GetComponent<MeshFilter>().mesh;		
		Bounds bounds = mesh.bounds;
		
		if(gekozenAsGlobaal == 'x'){
			return 0;
		} 
			
		if(gekozenAsGlobaal == 'y'){
			return bounds.min.y -1;
		}
			
		if(gekozenAsGlobaal == 'z'){
			return 0;
		}	
		return 0;
	}
	
	private float getLineEndXPositie(){
		Vector3 scale = selectedObject.transform.localScale;
		Mesh mesh = selectedObject.GetComponent<MeshFilter>().mesh;		
		Bounds bounds = mesh.bounds;
		
		if(gekozenAsGlobaal == 'x'){
			return bounds.max.x +1;
		} 
			
		if(gekozenAsGlobaal == 'y'){
			return bounds.center.x;
		}
			
		if(gekozenAsGlobaal == 'z'){
			return bounds.center.x;
		}	
		return 0;
	}
	
	private float getLineEndZPositie(){
		Vector3 scale = selectedObject.transform.localScale;
		Mesh mesh = selectedObject.GetComponent<MeshFilter>().mesh;		
		Bounds bounds = mesh.bounds;
		
		if(gekozenAsGlobaal == 'x'){
			return bounds.center.z ;
		} 
			
		if(gekozenAsGlobaal == 'y'){
			return bounds.center.z;
		}
			
		if(gekozenAsGlobaal == 'z'){
			return bounds.max.z +1;
		}	
		return 0;
	}
	
	private float getLineEndYPositie(){
		Vector3 scale = selectedObject.transform.localScale;
		Mesh mesh = selectedObject.GetComponent<MeshFilter>().mesh;		
		Bounds bounds = mesh.bounds;
		
		if(gekozenAsGlobaal == 'x'){
			return 0;
		} 
			
		if(gekozenAsGlobaal == 'y'){
			return bounds.max.y +1;
		}
			
		if(gekozenAsGlobaal == 'z'){
			return 0;
		}	
		return 0;
	}
	
	public void ScaleGroter(){
		if (gekozenAsGlobaal == 'x'){
			selectedObject.transform.localScale += new Vector3 (scaleStep,0,0);
		} else if (gekozenAsGlobaal == 'y'){
			selectedObject.transform.localScale += new Vector3 (0,scaleStep,0);
		} else if (gekozenAsGlobaal == 'z') {
			selectedObject.transform.localScale += new Vector3 (0,0,scaleStep);	
		}
		drawLine();

	}
		
	public void ScaleKleiner(){
		if (gekozenAsGlobaal == 'x'){
			selectedObject.transform.localScale += new Vector3 (-scaleStep,0,0);
		} else if (gekozenAsGlobaal == 'y'){
			selectedObject.transform.localScale += new Vector3 (0,-scaleStep,0);
		} else if (gekozenAsGlobaal == 'z') {
			selectedObject.transform.localScale += new Vector3 (0,0,-scaleStep);	
		}
		drawLine();
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
	
	private char getLocalAxisSameAsGlobalX(){
//		if(selectedObject.transform.right == Vector3.right) // local x is same as global y
//			return 'x';	
//		if(selectedObject.transform.up == Vector3.right) // local y is same as global y
//			return 'y';
//		if(selectedObject.transform.forward == Vector3.right) // local z is same as global y
//			return 'z';
		if (getLocalAxisSameAsGlobalY() == 'x'){
			return 'z';	
		} else if (getLocalAxisSameAsGlobalY() == 'y' ){	
			return 'x';
		} else if (getLocalAxisSameAsGlobalY() == 'z'){
			return 'x';
		}
		return 'x';
		
	}
	
	private char getLocalAxisSameAsGlobalZ(){
		if (getLocalAxisSameAsGlobalY() == 'x'){
			return 'y';	
		} else if (getLocalAxisSameAsGlobalY() == 'y' ){	
			return 'z';
		} else if (getLocalAxisSameAsGlobalY() == 'z'){
			return 'y';
		}
		return 'y';
	}
	
//	private void AnimateLine(){
//		Vector3 scaleVector = new Vector3(0,0,0);
//		
//		if (isAnimatieGroter){
//			if (gekozenAsGlobaal == 'x'){
//				scaleVector.x += scaleStep;
//			} else if (gekozenAsGlobaal == 'y'){
//				scaleVector.y += scaleStep;
//			} else if (gekozenAsGlobaal == 'z') {
//				scaleVector.z += scaleStep;	
//			}
//			animatieScale += scaleStep;
//		} else {
//			if (gekozenAsGlobaal == 'x'){
//				scaleVector.x -= scaleStep;
//			} else if (gekozenAsGlobaal == 'y'){
//				scaleVector.y -= scaleStep;
//			} else if (gekozenAsGlobaal == 'z') {
//				scaleVector.z -= scaleStep;	
//			}
//			animatieScale -= scaleStep;
//		}
//		
//		Vector3 scaleToGlobal = selectedObject.transform.TransformPoint(scaleVector);
//		line.transform.localScale += scaleToGlobal;
//		
//		if (animatieScale < 1.0f){
//			isAnimatieGroter = true;	
//		}
//		
//		if (animatieScale > 1.5f){
//			isAnimatieGroter = false;	
//		}
//		
//	}

}
