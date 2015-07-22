using UnityEngine;
using System.Collections;

public class VMECanvasButton : MonoBehaviour {

	public RectTransform parent;
	public float scale = 0.5f;

	private Vector3 sv;

	// Use this for initialization
	void Start () {
		if (parent == null)
		{
			parent = gameObject.GetComponent<RectTransform>();
		}
		sv = new Vector3(scale,scale,scale);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ReshapeEnter()
	{
		parent.localScale += sv;
	}

	public void ReshapeExit()
	{
		parent.localScale -= sv;
	}
}
