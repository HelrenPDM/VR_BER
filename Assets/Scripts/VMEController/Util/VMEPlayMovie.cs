using UnityEngine;
using System.Collections;

public class VMEPlayMovie : MonoBehaviour {

#if UNITY_ANDROID
	[Tooltip("Path of movie to be played. Must be in Assets/StreamingAssets folder!")] 
	public string Path = "";
#endif

	public Transform Screen;
	public bool Loop;

	// Use this for initialization
	void Start () {
		if (Screen == null)
		{
			Debug.LogWarning("No movie target set for " + this.gameObject.ToString());
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	/// <summary>
	/// Execute this instances action.
	/// </summary>
	public void Execute()
	{
		Debug.Log("Ececute called for " + this.gameObject.ToString());
		Renderer r = Screen.GetComponent<Renderer>();
#if UNITY_STANDALONE
		MovieTexture movie = (MovieTexture)r.material.mainTexture;
		
		if (movie.isPlaying) {
			movie.Pause();
		}
		else {
			movie.Play();
		}
#elif UNITY_ANDROID
		if (Path != "")
		{
			Handheld.PlayFullScreenMovie(Path);
		}
		else
		{
			Debug.LogWarning("No path to movie specified!");
		}
#endif
	}
}
