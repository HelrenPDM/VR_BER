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
public delegate void GenericEventHandler(object sender, EventArgs e);
#endregion

public class SceneManager : MonoBehaviour {

	// see, if we need this for reducing deployment size
	#region prefab resource paths (excluding Assets/Resources)
	public string CursorPrefab = "Prefabs/GazePointer";
	public string StandardController = "CharacterController";
	public float PlayerHeight = 1.8f;
	#endregion

	#region subsystem managers

	#if UNITY_STANDALONE
	public string OVRControllerPrefab = "OVR/Prefabs/OVRPlayerController";
	#endif

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

	public static GameObject globalEventSystem { get; private set; }

	#region private variables
	// keep a clean parent player object
	private GameObject Player;

	private GameObject CameraRig;

	// the heads up display attached to the parent camera (main or left eye)
	private GameObject HUD;
	// the menu attached to the reference euler angles at body position
	private GameObject Menu;
	#endregion

	/// <summary>
	/// Awake this instance.
	/// </summary>
	private void Awake()
	{
		globalEventSystem = GameObject.Find ("EventSystem");

		LoadEnvironment();

		// configure in-/ output devices
		DetectDevices();

		// configure character controller
		#if UNITY_STANDALONE
		SetUpController(OVRControllerPrefab,"CenterEyeAnchor", "VMEMainHall", false);
		#elif UNITY_ANDROID
		SetUpController(CardboardControllerPrefab,"Main Camera", "VMEStart", false);
		#elif UNITY_EDITOR && UNITY_WEBGL
		SetUpController(StandardController,"StandardCamera", "VMEStart", false);
		#endif
	}

	// Use this for initialization
	private void Start ()
	{
	}
	
	// Update is called once per frame
	private void Update () {

	}

	/// <summary>
	/// Loads the environment.
	/// </summary>
	private void LoadEnvironment()
	{

	}

	/// <summary>
	/// Detects the devices.
	/// </summary>
	private void DetectDevices()
	{
	#if UNITY_STANDALONE
		Debug.Log ("OVR HMD Count: " + Ovr.Hmd.Detect().ToString());
		// detect OVR device
		if (Ovr.Hmd.Detect() > 0)
		{
			Debug.Log ("Detected OVR Version " + Ovr.Hmd.GetVersionString() + "!");
			Ovr.Hmd.Initialize();
		} else if (Ovr.Hmd.Detect() == 0)
		{
			Debug.Log ("No OVR Device detected!");
		} else if (Ovr.Hmd.Detect() == -1)
		{
			Debug.Log ("OVR service unreachable!");
		}
	#endif
	}

	/// <summary>
	/// Sets up controller to the current VR device and camera GO.
	/// </summary>
	/// <param name="controllerGO">Controller G.</param>
	/// <param name="cameraGO">Camera G.</param>
	private void SetUpController(string controllerGO, string cameraGO, string locationGO, bool body)
	{
		// set parent GO
		this.Player = GameObject.Find("SceneCharacter");

		if (this.Player != null)
		{
			// set player position and orientation to scene start object
			var p = GameObject.Find (locationGO).transform.position;
			Debug.Log("Start at X: " + p.x +
			          " Y: " + p.y +
			          " Z: " + p.z);
			#if UNITY_ANDROID
			if (this.Player.transform.name != "CardboardMain")
			{
				Debug.Log ("Trying to instantiate " + controllerGO);
				this.Player = GameObject.Instantiate(Resources.Load (controllerGO) as GameObject);
				this.Player.AddComponent<CharacterController>();
				if (GameObject.Find("StandardCamera") != null)
				{
					GameObject.Find("StandardCamera").SetActive(false);
				}
			}
			#elif UNITY_STANDALONE
			if (this.Player.transform.name != "OVRPlayerController")
			{
				Debug.Log ("Trying to instantiate " + controllerGO);
				this.Player = GameObject.Instantiate(Resources.Load (controllerGO) as GameObject);
				this.Player.GetComponent<OVRPlayerController>().HmdRotatesY = false;	
			
				if (GameObject.Find("StandardCamera") != null)
				{
					GameObject.Find("StandardCamera").SetActive(false);
				}
			}
			#endif
			this.Player.transform.position = p;

			if (body)
			{
				if (this.Player.GetComponent<NavMeshAgent>() == null)
				{
					this.Player.AddComponent<NavMeshAgent>();
					Debug.Log("SceneManager: NavMeshAgent added to Player");
				}
			}
			else
			{
				if (this.Player.GetComponent<NavMeshAgent>() != null)
				{
					Destroy(this.Player.GetComponent<NavMeshAgent>());
					Debug.Log("SceneManager: NavMeshAgent removed from Player");
				}
			}
			if (this.Player.GetComponent<CharacterController>() == null)
			{
				this.Player.AddComponent<CharacterController>();
				Debug.Log("SceneManager: CharacterController added to Player");
			}
			if (this.Player.GetComponent<VMEPlayerController>() == null)
			{
				this.Player.AddComponent<VMEPlayerController>();
				this.Player.GetComponent<VMEPlayerController>().CenterRayTracker.Scene += handleSceneEvent;
				Debug.Log("SceneManager: VMEPlayerController added to Player");
			}

			if (body)
			{
				this.Player.GetComponent<VMEPlayerController>().MoveMode = true;
			}
			else
			{
				this.Player.GetComponent<VMEPlayerController>().MoveMode = false;
			}
			
			if (CameraRig == null)
			{
				CameraRig = GameObject.Find(cameraGO);
			}

			#if !UNITY_STANDALONE || !UNITY_ANDROID
			if (body)
			{
				p.y += PlayerHeight;
			}
			#endif
			CameraRig.transform.position = p;
			Vector3 euler = Vector3.zero;
			Player.transform.rotation = Quaternion.Euler(euler);
			CameraRig.transform.localRotation = Quaternion.Euler(euler);

			SetUpUI(CameraRig.transform, "HUD", 0f, 0f, 0.5f);

			if (body)
			{
				SetUpUI(this.Player.transform, "Menu", 0f, -0.7f, 0.5f);
			}

			Transform[] PlayerItems = this.Player.GetComponentsInChildren<Transform>(false);
			foreach (Transform e in PlayerItems)
			{
				Debug.Log (e.name + " in Player tree.");
			}
		}
	}

	/// <summary>
	/// Updates the controller.
	/// </summary>
	/// <param name="locationGO">Location G.</param>
	/// <param name="body">If set to <c>true</c> body.</param>
	private void UpdateController(string locationGO, bool body)
	{
		if (this.Player != null)
		{
			// set player position and orientation to scene start object
			var p = GameObject.Find (locationGO).transform.position;
			Debug.Log("Start at X: " + p.x +
			          " Y: " + p.y +
			          " Z: " + p.z);
			this.Player.transform.position = p;
			
			if (body)
			{
				if (this.Player.GetComponent<NavMeshAgent>() == null)
				{
					this.Player.AddComponent<NavMeshAgent>();
					Debug.Log("SceneManager: NavMeshAgent added to Player");
				}
				this.Player.GetComponent<VMEPlayerController>().MoveMode = true;
			}
			else
			{
				if (this.Player.GetComponent<NavMeshAgent>() != null)
				{
					Destroy(this.Player.GetComponent<NavMeshAgent>());
					Debug.Log("SceneManager: NavMeshAgent removed from Player");
				}
				this.Player.GetComponent<VMEPlayerController>().MoveMode = false;
			}

			#if !UNITY_STANDALONE || !UNITY_ANDROID
			if (body)
			{
				p.y += PlayerHeight;
			}
			#endif

			CameraRig.transform.position = p;
			Vector3 euler = Vector3.zero;
			Player.transform.rotation = Quaternion.Euler(euler);
			CameraRig.transform.localRotation = Quaternion.Euler(euler);
		}
	}

	/// <summary>
	/// Sets up UI by loading a HUD prefab and attaching it to a parent transform that represents 
	/// </summary>
	/// <param name="parentAnchor">Parent anchor.</param>
	private void SetUpUI(Transform parentAnchor, string name, float relX, float relY, float relZ)
	{
		var p = parentAnchor.position;
		p.x += relX;
		p.y += relY;
		p.z += relZ;
		Debug.Log ("Setting " + parentAnchor + " as " + name + " parent!");
		GameObject.Find (name).transform.position = p;
		GameObject.Find (name).transform.SetParent(parentAnchor);
	}

	/// <summary>
	/// Sets up camera.
	/// </summary>
	private void SetUpCamera()
	{
	}



	private void handleSceneEvent(object sender, SceneEventArgs args)
	{
		Debug.Log("SceneManager: Reloading Player at new position with body " + args.NextBody.ToString() + ".");
		// configure character controller
		UpdateController(args.NextSpot.name, args.NextBody);
	}

}
