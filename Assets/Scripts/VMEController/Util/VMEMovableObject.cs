using UnityEngine;
using System.Collections;

public class VMEMovableObject : MonoBehaviour {

	public Vector3 MoveShift;
	private Vector3 MoveStart;
	private Vector3 MoveEnd;
	private Vector3 CurrentPos;

	public float speed = 1.0F;
	private float startTime;
	private float journeyLength;

	private bool Rest;

	// Use this for initialization
	void Start () {
		MoveStart = gameObject.transform.localPosition;
		MoveEnd = MoveStart + MoveShift;
		journeyLength = Vector3.Distance(MoveStart, MoveEnd);
		Rest = true;
	}
	
	// Update is called once per frame
	void Update () {
		CurrentPos = transform.localPosition;
		if (!Rest && (CurrentPos != MoveEnd))
		{
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			transform.localPosition = Vector3.Lerp(CurrentPos, MoveEnd, fracJourney);
		}
		else if (Rest && (CurrentPos != MoveStart))
		{
			float distCovered = (Time.time - startTime) * speed;
			float fracJourney = distCovered / journeyLength;
			transform.localPosition = Vector3.Lerp(CurrentPos, MoveStart, fracJourney);
		}
	}

	public void ToggleMove()
	{	
		Debug.Log(transform.name + ".ToggleMove()");
		startTime = Time.time;
		Rest = !Rest;
	}
}
