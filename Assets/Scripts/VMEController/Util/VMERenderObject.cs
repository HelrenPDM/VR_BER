using UnityEngine;
using System.Collections;

public class VMERenderObject : MonoBehaviour {

	public Transform[] targets;
	public bool render { get; private set; }

	// Use this for initialization
	void Start () {
		if (targets.Length == 0)
		{
			Debug.LogWarning("No render target set for " + this.gameObject.ToString());
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void renderTarget()
	{
		if (targets.Length > 0)
		{
			foreach (Transform t in targets)
			{
				t.transform.GetComponent<MeshRenderer>().enabled = !this.render;
			}
			this.render = !this.render;
		}
	}
}
