using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FetchImageFromURL : MonoBehaviour {

	public string url = "http://www.berlin-airport.de/_images/presse/mediathek/_fotos/baustellendoku/2012-juni/2012-ber-airport-city.jpg";

	private WWW resource;

	void Start ()
	{
		Debug.Log("Image loading by" + gameObject.transform.name + " started");
		StartCoroutine("StartFetch");
	}

	// Use this for initialization
	IEnumerator StartFetch () {
		resource = new WWW(url);

		while (!resource.isDone)
		{
			Debug.Log("Downloading image " + resource.progress + " %");
			yield return resource.isDone;
		}
		UpdateRawImage();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	void Update ()
	{
	}
	
	// Update is called once per frame
	void UpdateRawImage () {
		Debug.Log("Setting image with height: " + resource.texture.height +
		          " width: " + resource.texture.width);
		gameObject.GetComponent<RawImage>().texture = resource.texture;
	}
}
