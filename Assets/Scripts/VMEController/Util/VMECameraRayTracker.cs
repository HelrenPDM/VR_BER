//
//  VMECameraRayTracker.cs
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
using System;
using System.Collections;

/// <summary>
/// Focus locked event handler.
/// </summary>
public delegate void SceneEventHandler(object sender, SceneEventArgs e);

/// <summary>
/// Camera ray tracker.
/// </summary>
public class VMECameraRayTracker {

	#region public variables
	/// <summary>
	/// The name of the object.
	/// </summary>
	public string TargetObjectName { get ; private set; }

	/// <summary>
	/// The target object tag.
	/// </summary>
	public string TargetObjectTag { get; private set; }

	/// <summary>
	/// Gets a value indicating whether this <see cref="OVRCameraRig"/> has an object in ray.
	/// </summary>
	/// <value><c>true</c> if object in ray; otherwise, <c>false</c>.</value>
	public bool TargetObjectInRay { get; private set; }

	/// <summary>
	/// Gets the object in ray distance.
	/// </summary>
	/// <value>The object in ray distance.</value>
	public float TargetObjectDistance { get; private set; }

	/// <summary>
	/// Gets the target object.
	/// </summary>
	/// <value>The target object.</value>
	public GameObject TargetObject { get; private set; }

	/// <summary>
	/// Gets a value indicating whether this <see cref="VMECameraRayTracker"/> target object focus is locked.
	/// </summary>
	/// <value><c>true</c> if target object focus timed in; otherwise, <c>false</c>.</value>
	public bool TargetObjectFocusLocked { get; private set; }

	/// <summary>
	/// Gets the focus time percentage for visual progress information.
	/// </summary>
	/// <value>The focus time percentage.</value>
	public float FocusLockPercentage { get; private set; }

	/// <summary>
	/// Gets the target object hit point.
	/// </summary>
	/// <value>The target object hit point.</value>
	public Vector3 TargetObjectHitPoint { get; private set; }

	/// <summary>
	/// Occurs when switching to new scene position.
	/// </summary>
	public event SceneEventHandler Scene;

	#endregion

	#region private variables
	protected Camera CameraController;
	protected VMEFocusTimer FocusTimer = new VMEFocusTimer();

	// default ray origin (center screen)
	private float RayXPosition = 0.5f;
	private float RayYPosition = 0.5f;
	#endregion

	#region public methods
	/// <summary>
	/// Init this instance.
	/// </summary>
	public void Init () {
		this.FocusTimer.Init();
		this.FocusTimer.Focus += handleFocusEvent;
		this.FocusTimer.FocusEnter += handleFocusEnter;
		this.FocusTimer.FocusExit += handleFocusExit;
		this.TargetObjectFocusLocked = false;
	}

	/// <summary>
	/// Init the specified rayX and rayY.
	/// </summary>
	/// <param name="rayX">Ray x as float between 0 (leftmost) and 1 (rightmost).</param>
	/// <param name="rayY">Ray y as float between 0 (lowest) and 1 (upmost).</param>
	public void Init (float rayX, float rayY) {
		this.RayXPosition = rayX;
		this.RayYPosition = rayY;
		this.FocusTimer.Init();
		this.FocusTimer.Focus += handleFocusEvent;
		this.FocusTimer.FocusEnter += handleFocusEnter;
		this.FocusTimer.FocusExit += handleFocusExit;
		this.TargetObjectFocusLocked = false;
	}

	/// <summary>
	/// Sets the OVR camera controller.
	/// </summary>
	/// <param name="cameraController">Camera controller.</param>
	public void SetCameraController(ref Camera cameraController)
	{
		if (cameraController != null)
		{
			this.CameraController = cameraController;
		}
		else
		{
			Debug.Log("VMECameraTracker: No reference to camera!");
		}
	}

	/// <summary>
	/// Fires the ray.
	/// </summary>
	public void FireRay()
	{
		Ray rayleft = this.CameraController.ViewportPointToRay(new Vector3(this.RayXPosition, this.RayYPosition, 0));
		RaycastHit hitleft;
		if (Physics.Raycast (rayleft, out hitleft))
		{
			this.TargetObjectHitPoint = hitleft.point;
			this.TargetObjectDistance = hitleft.distance;
			this.FocusTimer.UpdateFocus(hitleft.transform.gameObject);
			this.FocusLockPercentage = this.FocusTimer.GetFocusTimePercentage();
		}
		else
		{
			this.FocusTimer.UpdateFocus(null);
			this.FocusLockPercentage = 0.0f;
		}
	}
	#endregion

	#region private methods
	private void handleFocusEvent(object sender, FocusEventArgs args)
	{
		//Debug.Log(this.ToString() + ".handleFocusEvent(...)");
		if (args.FocusObject != null)
		{
			this.TargetObject = args.FocusObject;
			this.TargetObjectName = this.TargetObject.transform.name;
			this.TargetObjectTag = this.TargetObject.tag;
			this.TargetObjectFocusLocked = args.InFocus;
			if (this.TargetObjectFocusLocked)
			{
				FocusTargetBehaviour(this.TargetObject);
			}
		}
	}

	private void handleFocusEnter(object sender, FocusEventArgs args)
	{
		//Debug.Log(this.ToString() + ".handleFocusEnter(...)");
		FocusEnterBehaviour(args.FocusObject);
	}

	private void handleFocusExit(object sender, FocusEventArgs args)
	{
		//Debug.Log(this.ToString() + ".handleFocusExit(...)");
		FocusExitBehaviour(args.FocusObject);
	}

	private void FocusTargetBehaviour(GameObject go)
	{
		switch (go.tag)
		{
		case "TriggerScene":
			SceneEventArgs args = new SceneEventArgs();
			args.NextSpot = go.GetComponent<VMENextSpot>().nextSpot;
			args.NextBody = go.GetComponent<VMENextSpot>().nextBody;
			args.LookAt = go.GetComponent<VMENextSpot>().lookAt;
			OnSceneEvent(args);
			break;
		case "TriggerObject":
			go.GetComponent<VMEObjectAction>().LockAction();
			Debug.Log("FocusEventVMECameraRayTracker on object: " + this.TargetObjectName);
			break;
		default:
			Debug.Log("FocusEventVMECameraRayTracker on object: " + this.TargetObjectName);
			break;
		}
	}

	private void FocusEnterBehaviour(GameObject go)
	{
		if (go.GetComponent<VMEObjectAction>() != null)
		{
			go.GetComponent<VMEObjectAction>().EnterAction();
		}
	}

	private void FocusExitBehaviour(GameObject go)
	{
		if (go.GetComponent<VMEObjectAction>() != null)
		{
			go.GetComponent<VMEObjectAction>().ExitAction();
		}
	}

	/// <summary>
	/// Raises the focus locked event event.
	/// </summary>
	/// <param name="e">E.</param>
	protected virtual void OnSceneEvent(SceneEventArgs e)
	{
		if (Scene != null) {
			Scene (this, e);
		}
	}
	#endregion
}

public class SceneEventArgs : EventArgs
{
	public Transform NextSpot { get; set; }
	public bool NextBody { get; set; }
	public Vector3 LookAt { get; set; }
}
