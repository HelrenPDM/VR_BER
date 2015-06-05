//
//  VMEFocusTimer.cs
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

/// <summary>
/// Focus locked event handler.
/// </summary>
public delegate void FocusEventHandler(object sender, FocusEventArgs e);

/// <summary>
/// VME focus timer is a helper class to handle duration of a ray hit on an object and returning focus events.
/// </summary>
public class VMEFocusTimer {

	/// <summary>
	/// Occurs when focus locked.
	/// </summary>
	public event FocusEventHandler Focus;

	#region private variables
	private float focusThreshold = 2.0f;
	private float currentTimePassed = 0.0f;

	// handle to the object in focus
	private GameObject focusObject;
	private bool locked;
	private bool nullObject;
	#endregion

	#region init section
	// Use this for initialization
	public void Init ()
	{
		this.focusObject = null;
		this.locked = false;
		this.nullObject = true;
	}

	// Use this for parametrized initialization
	public void Init(float focThreshold)
	{
		this.focusThreshold = focThreshold;
		this.focusObject = null;
		this.locked = false;
		this.nullObject = true;
	}
	#endregion

	#region public functions
	/// <summary>
	/// Updates the focus.
	/// </summary>
	/// <param name="focObject">Foc object.</param>
	public void UpdateFocus (GameObject focObject)
	{
		FocusEventArgs args = new FocusEventArgs();

		if (focObject != null)
		{
			this.nullObject = false;
			// still looking at object
			if (this.focusObject == focObject)
			{
				if (!locked)
				{
					if (this.currentTimePassed <= this.focusThreshold)
					{
						this.currentTimePassed += Time.deltaTime;
					}
					else
					{
						// fire event InFocus true
						args.FocusObject = focObject;
						args.InFocus = true;
						OnFocusEvent(args);
						ResetFocus();
					}
				}
			}
			// object changed, set local to new
			else
			{
				this.focusObject = focObject;
				ResetFocus();
				// fire event InFocus false
				args.FocusObject = focObject;
				args.InFocus = false;
				OnFocusEvent(args);
			}
		}
		else
		{
			// register null instance one time
			if (!this.nullObject)
			{
				this.nullObject = true;
				this.focusObject = null;
				ResetFocus();
				// fire event InFocus false
				args.FocusObject = null;
				args.InFocus = false;
				OnFocusEvent(args);
			}
		}
	}

	/// <summary>
	/// Resets the focus.
	/// </summary>
	public void ResetFocus()
	{
		this.currentTimePassed = 0.0f;
		this.locked = false;
	}

	/// <summary>
	/// Gets the focus time.
	/// </summary>
	public float GetFocusTime()
	{
		return this.currentTimePassed;
	}

	/// <summary>
	/// Gets the focus time percentage as a value between 0 and 1.
	/// </summary>
	/// <returns>The focus time percentage.</returns>
	public float GetFocusTimePercentage()
	{
		if (this.focusThreshold != 0) {
			return (this.currentTimePassed / this.focusThreshold);
		} else
			return 0.0f;
	}
	#endregion

	/// <summary>
	/// Raises the focus locked event event.
	/// </summary>
	/// <param name="e">E.</param>
	protected virtual void OnFocusEvent(FocusEventArgs e)
	{
		if (Focus != null) {
			Focus (this, e);
		}
	}
}

public class FocusEventArgs : EventArgs
{
	public GameObject FocusObject { get; set; }
	public bool InFocus { get; set; }
}
