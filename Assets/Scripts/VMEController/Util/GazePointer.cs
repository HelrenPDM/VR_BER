using UnityEngine;
using System.Collections;

public class GazePointer : MonoBehaviour {

	public Color colorStart = Color.red;
	public Color colorEnd = Color.green;
	public Renderer rend;
	public float distanceScaleCorrection = .02f;

	// Use this for initialization
	void Start () {
		rend = GetComponent<Renderer> ();
	}
	
	// Update is called once per frame
	void Update () {
		// set transparent, if needed (e.g. not looking at a triggering object	
	}

	/// <summary>
	/// Updates the color at a Lerp between red (0) and green (1)
	/// </summary>
	/// <param name="value">Value.</param>
	public void UpdateColor(float value, float transparency)
	{
		this.rend.material.color = Color.Lerp (this.colorStart, this.colorEnd, value);
		if (transparency >= 0f || transparency <= 1f)
		{
			var alpha = this.rend.material.color;
			alpha.a = transparency;
			this.rend.material.color = alpha;
		}
	}

	/// <summary>
	/// Updates the position and scale.
	/// </summary>
	/// <param name="pos">Position.</param>
	/// <param name="scale">Scale.</param>
	/// <param name="distance">Distance.</param>
	public void UpdatePositionAndScale(Vector3 pos, float distance)
	{
		transform.position = pos;
		var s = distance * distanceScaleCorrection;
		transform.localScale = new Vector3(s,s,s);
	}
}
