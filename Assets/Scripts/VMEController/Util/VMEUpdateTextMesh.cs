using UnityEngine;
using System.Collections;

/// <summary>
/// Update text mesh.
/// </summary>
public class VMEUpdateTextMesh : MonoBehaviour {

	// Use this for initialization
	void Start () {
		gameObject.GetComponent<TextMesh> ().text = "TextMesh";
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/// <summary>
	/// Updates the text.
	/// </summary>
	/// <param name="text">Text.</param>
	public void UpdateText(string text)
	{
		gameObject.GetComponent<TextMesh> ().text = text;
	}
}
