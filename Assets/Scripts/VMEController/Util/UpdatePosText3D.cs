using UnityEngine;
using System.Collections;

public class UpdatePosText3D : MonoBehaviour {

	private Transform CamPos;

	// Use this for initialization
	void Start () {
		CamPos = transform.parent.parent;
		string x = CamPos.transform.position.x.ToString();
		string y = CamPos.transform.position.y.ToString();
		string z = CamPos.transform.position.z.ToString();
		transform.GetComponent<TextMesh>().text = ("x: " + x + "\n" +
		                                           "y: " + y + "\n" +
		                                           "z: " + z + "\n");
	}
	
	// Update is called once per frame
	void Update () {		
		string x = CamPos.transform.position.x.ToString();
		string y = CamPos.transform.position.y.ToString();
		string z = CamPos.transform.position.z.ToString();
		transform.GetComponent<TextMesh>().text = ("x: " + x + "\n" +
		                                           "y: " + y + "\n" +
		                                           "z: " + z + "\n");
	}
}
