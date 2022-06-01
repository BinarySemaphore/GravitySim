using UnityEngine;
using System.Collections;

public class TouchControls : MonoBehaviour {

	private Canvas canvas;

	void Awake () {
		canvas = GetComponent<Canvas> ();
	}

	void Start () {
		canvas.enabled = false;
	}

	void Update () {
	}

	public void Enable () {
		canvas.enabled = true;
	}

	public void Disable () {
		canvas.enabled = false;
	}

	public void KeyMoveForward () {
		UniverseBehavior.IB.SetKeyPress (KeyCode.W);
	}

	public void KeyMoveBackward () {
		UniverseBehavior.IB.SetKeyPress (KeyCode.S);
	}

	public void KeyMoveRight () {
		UniverseBehavior.IB.SetKeyPress (KeyCode.D);
	}

	public void KeyMoveLeft () {
		UniverseBehavior.IB.SetKeyPress (KeyCode.A);
	}

	public void KeyMoveUp () {
		UniverseBehavior.IB.SetKeyPress (KeyCode.LeftShift);
	}

	public void KeyMoveDown () {
		UniverseBehavior.IB.SetKeyPress (KeyCode.LeftControl);
	}

	public void KeyZoomIn () {
		UniverseBehavior.IB.SetFloatKey ("scrollDeltaY", 1.0f);
	}

	public void KeyZoomOut () {
		UniverseBehavior.IB.SetFloatKey ("scrollDeltaY", -1.0f);
	}

	public void KeyTimeFactorLess () {
		UniverseBehavior.IB.SetKeyDown (KeyCode.Comma);
	}

	public void KeyTimeFactorMore () {
		UniverseBehavior.IB.SetKeyDown (KeyCode.Period);
	}
}
