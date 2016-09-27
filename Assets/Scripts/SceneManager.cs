//
//  SceneManager.cs
//
//  Author:
//       Stephan Gensch <stgensch@netzwerkcafe.org>
//
//  Copyright (c) 2015 Stephan Gensch
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
//
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using UnityEngine;
using System.Collections;
using System;

#region event delegates
/// <summary>
/// Generic event handler.
/// </summary>
public delegate void GenericEventHandler (object sender,EventArgs e);
#endregion

public class SceneManager : MonoBehaviour
{

	// see, if we need this for reducing deployment size
	#region prefab resource paths (excluding Assets/Resources)
	public string CursorPrefab = "Prefabs/GazePointer";
	public string StandardController = "CharacterController";
	public float PlayerHeight = 1.8f;
	public Transform VMEStart;
	public bool UseBody;
	public GameObject m_GlobalManager;
	#endregion

	#region subsystem managers


	#if UNITY_ANDROID
	/// <summary>
	/// Reference to the cardboard manager.
	/// </summary>
	public static Cardboard CardboardManager = null;

	public string CardboardControllerPrefab = "Cardboard/Prefabs/CardboardMain";
	#endif

	#endregion

	/// <summary>
	/// Gets the singleton instance.
	/// </summary>
	public static SceneManager instance { get; private set; }

	#region private variables
	// keep a clean parent player object
	private GameObject m_Player;

	//
	private GameObject m_CameraRig;

	//
	private NavMeshAgent m_NavAgent;

	// the heads up display attached to the parent camera (main or left eye)
	private GameObject m_HUD;
	// the menu attached to the reference euler angles at body position
	// private GameObject m_Menu;

	CursorLockMode wantedMode;
	#endregion

	/// <summary>
	/// Awake this instance.
	/// </summary>
	private void Awake ()
	{
		m_GlobalManager = FindObjectOfType<GlobalManager> ().gameObject;
		VMEStart = GameObject.Find (m_GlobalManager.transform.GetComponent<GlobalManager> ().VMEStart).transform;
		UseBody = m_GlobalManager.transform.GetComponent<GlobalManager> ().UseBody;
		// configure in-/ output devices
		DetectDevices ();

		// configure character controller
		#if UNITY_ANDROID
		SetUpController(CardboardControllerPrefab, "Main Camera", VMEStart, UseBody);
		#else
		SetUpController (StandardController, "StandardCamera", VMEStart, UseBody);
		#endif
	}

	// Use this for initialization
	private void Start ()
	{
	}
	
	// Update is called once per frame
	private void Update ()
	{
	}

	// Apply requested cursor state
	void SetCursorState ()
	{
		Cursor.lockState = wantedMode;
		// Hide cursor when locking
		Cursor.visible = (CursorLockMode.Locked != wantedMode);
	}	

	#if UNITY_WEBGL || UNITY_WEBPLAYER
	/// <summary>
	/// Raises the GUI event.
	/// </summary>
	void OnGUI ()
	{
		GUILayout.BeginVertical ();
		// Release cursor on escape keypress
		if (Input.GetKeyDown (KeyCode.Escape))
			Cursor.lockState = wantedMode = CursorLockMode.None;
		
		switch (Cursor.lockState)
		{
		case CursorLockMode.None:
			GUILayout.Label ("Cursor is normal");
			if (GUILayout.Button ("Lock cursor"))
				wantedMode = CursorLockMode.Locked;
			if (GUILayout.Button ("Confine cursor"))
				wantedMode = CursorLockMode.Confined;
			break;
		case CursorLockMode.Confined:
			GUILayout.Label ("Cursor is confined");
			if (GUILayout.Button ("Lock cursor"))
				wantedMode = CursorLockMode.Locked;
			if (GUILayout.Button ("Release cursor"))
				wantedMode = CursorLockMode.None;
			break;
		case CursorLockMode.Locked:
			GUILayout.Label ("Cursor is locked");
			if (GUILayout.Button ("Unlock cursor"))
				wantedMode = CursorLockMode.None;
			if (GUILayout.Button ("Confine cursor"))
				wantedMode = CursorLockMode.Confined;
			break;
		}
		
		GUILayout.EndVertical ();
		
		SetCursorState ();
	}
	#endif

	/// <summary>
	/// Loads the environment.
	/// </summary>
	private void LoadEnvironment ()
	{
	}

	/// <summary>
	/// Detects the devices.
	/// </summary>
	private void DetectDevices ()
	{
	}

	/// <summary>
	/// Sets up controller to the current VR device and camera GO.
	/// </summary>
	/// <param name="controllerGO">Controller G.</param>
	/// <param name="cameraGO">Camera G.</param>
	private void SetUpController (string controllerGO, string cameraGO, Transform locationGO, bool body)
	{
		// set parent GO
		this.m_Player = GameObject.Find ("SceneCharacter");

		if (this.m_Player != null) {

			#if UNITY_ANDROID
			Debug.Log ("Trying to instantiate " + controllerGO);
			this.m_Player = GameObject.Instantiate(Resources.Load (controllerGO) as GameObject);
			this.m_Player.GetComponent<Cardboard>().NeckModelScale = 0;
			this.m_Player.GetComponent<Cardboard>().AutoDriftCorrection = true;
			this.m_Player.AddComponent<CharacterController>();

			if (GameObject.Find("StandardCamera") != null)
			{
				GameObject.Find("StandardCamera").SetActive(false);
			}
			#endif
			
			if (this.m_Player.GetComponent<CharacterController> () == null) {
				this.m_Player.AddComponent<CharacterController> ();
				Debug.Log ("SceneManager: CharacterController added to Player");
			}

			if (this.m_Player.GetComponent<VMEPlayerController> () == null) {
				this.m_Player.AddComponent<VMEPlayerController> ();
				this.m_Player.GetComponent<VMEPlayerController> ().CenterRayTracker.Scene += handleSceneEvent;
				Debug.Log ("SceneManager: VMEPlayerController added to Player");
			}

			Camera[] cams = this.m_Player.GetComponentsInChildren<Camera> ();
			foreach (Camera cam in cams) {
				cam.farClipPlane = 6000;
			}

			if (body) {
				this.m_Player.GetComponent<VMEPlayerController> ().MoveMode = true;
			} else {
				this.m_Player.GetComponent<VMEPlayerController> ().MoveMode = false;
			}
			
			if (m_CameraRig == null) {
				m_CameraRig = GameObject.Find (cameraGO);
			}

			SetUpUI (m_CameraRig.transform, "HUD", 0f, 0f, 0.5f);

			UpdateController (locationGO, body, Vector3.zero);
		}
	}

	/// <summary>
	/// Updates the controller.
	/// </summary>
	/// <param name="locationGO">Location G.</param>
	/// <param name="body">If set to <c>true</c> body.</param>
	private void UpdateController (Transform locationGO, bool body, Vector3 lookAt)
	{
		if (m_Player != null) {	
			
			// set player position and orientation to scene start object
			var p = locationGO.transform.position;
			Debug.Log ("Start at X: " + p.x +
				" Y: " + p.y +
				" Z: " + p.z);
			m_Player.transform.position = p;

			if (body) {
				Debug.Log ("Correcting body height.");
				var q = new Vector3 (0f, PlayerHeight, 0f);
				#if UNITY_STANDALONE || UNITY_STANDALONE_OSX
				m_Player.transform.position = p + q;
				#else
				m_CameraRig.transform.position = p + q;
				#endif

				if (m_NavAgent == null) {
					m_Player.AddComponent<NavMeshAgent> ();
					m_NavAgent = m_Player.GetComponent<NavMeshAgent> ();
					Debug.Log ("SceneManager: NavMeshAgent added to Player");
					CheckNavMeshAgent (m_NavAgent);
				}
				m_Player.GetComponent<VMEPlayerController> ().MoveMode = true;
			} else {
				if (m_Player.GetComponent<NavMeshAgent> () != null) {
					DestroyImmediate (m_Player.GetComponent<NavMeshAgent> ());
					Debug.Log ("SceneManager: NavMeshAgent removed from Player");
				}
				m_Player.GetComponent<VMEPlayerController> ().MoveMode = false;
			}
		}
	}

	/// <summary>
	/// Checks the nav mesh agent.
	/// </summary>
	/// <param name="agent">Agent.</param>
	private void CheckNavMeshAgent (NavMeshAgent agent)
	{
		#if UNITY_STANDALONE || UNITY_STANDALONE_OSX
		agent.baseOffset = 1.7f;
		#else
		agent.baseOffset = 0.1f;
		#endif
		agent.autoTraverseOffMeshLink = true;
		agent.updateRotation = false;
		agent.height = 1.8f;
	}
	
	/// <summary>
	/// Sets up UI by loading a HUD prefab and attaching it to a parent transform that represents 
	/// </summary>
	/// <param name="parentAnchor">Parent anchor.</param>
	private void SetUpUI (Transform parentAnchor, string name, float relX, float relY, float relZ)
	{
		var p = parentAnchor.position;
		p.x += relX;
		p.y += relY;
		p.z += relZ;
		Debug.Log ("Setting " + parentAnchor + " as " + name + " parent!");
		GameObject.Find (name).transform.position = p;
		GameObject.Find (name).transform.SetParent (parentAnchor);
	}

	/// <summary>
	/// Sets up camera.
	/// </summary>
	private void SetUpCamera ()
	{
		//TODO: add any enhancing effects like global fog, etc.
	}



	private void handleSceneEvent (object sender, SceneEventArgs args)
	{
		Debug.Log ("SceneManager: Reloading Player at new position with body " + args.NextBody.ToString () + ".");
		if (this.m_Player.GetComponent<NavMeshAgent> () != null) {
			DestroyImmediate (this.m_Player.GetComponent<NavMeshAgent> ());
			Debug.Log ("SceneManager: NavMeshAgent removed from Player");
		}
		// configure character controller
		UpdateController (args.NextSpot, args.NextBody, args.LookAt);
	}

}
