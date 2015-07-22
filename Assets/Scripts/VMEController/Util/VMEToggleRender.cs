using UnityEngine;
using System.Collections;

public class VMEToggleRender : MonoBehaviour {

	public Transform[] targets;

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

	public void RenderTarget()
	{
		if (targets.Length > 0)
		{
			foreach (Transform t in targets)
			{
				t.transform.GetComponent<MeshRenderer>().enabled = !t.transform.GetComponent<MeshRenderer>().enabled;
			}
		}
	}
}
