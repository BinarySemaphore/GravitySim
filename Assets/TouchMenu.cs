using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TouchMenu : MonoBehaviour {

	public TouchControls movement;
	public TouchControls zoomAndTime;

	private int massSelect;
	private Canvas canvas;

	void Awake () {
		canvas = GetComponent<Canvas> ();
	}

	void Start () {
		massSelect = 1;
		canvas.enabled = false;
	}

	void Update () {
	}

	public void SetEnableBy (bool menuOn, bool movementOn, bool zoomAndTimeOn) {
		if (menuOn) {
			canvas.enabled = true;
		} else {
			canvas.enabled = false;
		}
		if (movementOn) {
			movement.Enable ();
		} else {
			movement.Disable ();
		}
		if (zoomAndTimeOn) {
			zoomAndTime.Enable ();
		} else {
			zoomAndTime.Disable ();
		}
	}

	public void KeyEscape () {
		UniverseBehavior.IB.SetKeyDown (KeyCode.Escape);
	}

	public void ResetCam () {
		UniverseBehavior.IB.SetKeyDown (KeyCode.R);
	}

	public void FreeCam () {
		UniverseBehavior.IB.SetKeyDown (KeyCode.F);
	}

	public void NextObject () {
		UniverseBehavior.IB.SetKeyDown (KeyCode.Equals);
	}

	public void PreviousObject () {
		UniverseBehavior.IB.SetKeyDown (KeyCode.Minus);
	}

	public void MassSelect (int rank) {
		massSelect = rank;
		MassSelect ();
	}

	public void MassNext () {
		massSelect -= 1;

		if (massSelect < 1) {
			massSelect = 1;
		}

		MassSelect ();
	}

	public void MassPrevious () {
		massSelect += 1;

		if (massSelect > 9) {
			massSelect = 9;
		}

		MassSelect ();
	}

	private void MassSelect () {
		if (massSelect == 1) {
			UniverseBehavior.IB.SetKeyDown (KeyCode.Alpha1);
		} else if (massSelect == 2) {
			UniverseBehavior.IB.SetKeyDown (KeyCode.Alpha2);
		} else if (massSelect == 3) {
			UniverseBehavior.IB.SetKeyDown (KeyCode.Alpha3);
		} else if (massSelect == 4) {
			UniverseBehavior.IB.SetKeyDown (KeyCode.Alpha4);
		} else if (massSelect == 5) {
			UniverseBehavior.IB.SetKeyDown (KeyCode.Alpha5);
		} else if (massSelect == 6) {
			UniverseBehavior.IB.SetKeyDown (KeyCode.Alpha6);
		} else if (massSelect == 7) {
			UniverseBehavior.IB.SetKeyDown (KeyCode.Alpha7);
		} else if (massSelect == 8) {
			UniverseBehavior.IB.SetKeyDown (KeyCode.Alpha8);
		} else if (massSelect == 9) {
			UniverseBehavior.IB.SetKeyDown (KeyCode.Alpha9);
		}
	}
}
