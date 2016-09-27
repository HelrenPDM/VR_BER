using UnityEngine;
using System.Collections;

public class GlobalManager : MonoBehaviour
{
	public string VMEStart = "";
	public bool UseBody;
	
	void Awake ()
	{
		DontDestroyOnLoad (this.gameObject);
	}

	// Use this for initialization
	void Start ()
	{
		if (VMEStart == "") {
			Debug.LogError ("Initial starting location not set. Terminating.");
			Application.Quit ();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
