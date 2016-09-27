using UnityEngine;
using System.Collections;

public class VMECanvasButton : MonoBehaviour
{

	public RectTransform rect;
	public float scale = 1.5f;

	// Use this for initialization
	void Start ()
	{
		if (rect == null) {
			rect = gameObject.GetComponent<RectTransform> ();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void ReshapeEnter ()
	{
		rect.localScale *= scale;
	}

	public void ReshapeExit ()
	{
		rect.localScale /= scale;
	}
}
