using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InputBroker {
	public Dictionary<string, float> floatKeys = new Dictionary<string, float> ();
	public Dictionary<string, float> lateFloatKeys = new Dictionary<string, float> ();
	public Dictionary<KeyCode, bool> pressKeys = new Dictionary<KeyCode, bool> ();
	public Dictionary<KeyCode, bool> downKeys = new Dictionary<KeyCode, bool> ();
	public Dictionary<KeyCode, bool> latePressKeys = new Dictionary<KeyCode, bool> ();
	public Dictionary<KeyCode, bool> lateDownKeys = new Dictionary<KeyCode, bool> ();

	public void Init () {
		string[] floatKeyCodes = {
			"scrollDeltaY"
		};
		KeyCode[] pressKeyCodes = {
			KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D,
			KeyCode.LeftShift, KeyCode.LeftControl
		};
		KeyCode[] downKeyCodes = {
			KeyCode.Escape, KeyCode.Q, KeyCode.Comma, KeyCode.Period,
			KeyCode.R, KeyCode.F, KeyCode.Minus, KeyCode.Equals,
			KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5,
			KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9
		};

		for (int i = 0; i < floatKeyCodes.Length; i++) {
			floatKeys.Add (floatKeyCodes [i], 0.0f);
			lateFloatKeys.Add (floatKeyCodes [i], 0.0f);
		}
			
		for (int i = 0; i < pressKeyCodes.Length; i++) {
			pressKeys.Add (pressKeyCodes [i], false);
			latePressKeys.Add (pressKeyCodes [i], false);
		}

		for (int i = 0; i < downKeyCodes.Length; i++) {
			downKeys.Add (downKeyCodes [i], false);
			lateDownKeys.Add (downKeyCodes [i], false);
		}
	}

	public void Update () {
		lateFloatKeys ["scrollDeltaY"] = lateFloatKeys ["scrollDeltaY"] + Input.mouseScrollDelta.y;

		List <KeyCode> keyList = new List<KeyCode> (pressKeys.Keys);
		foreach (KeyCode key in keyList) {
			latePressKeys [key] = latePressKeys [key] | Input.GetKey (key);
		}

		keyList = new List<KeyCode> (downKeys.Keys);
		foreach (KeyCode key in keyList) {
			lateDownKeys [key] = lateDownKeys [key] | Input.GetKeyUp (key);
		}
	}
	
	public void PostUpdate () {
		floatKeys ["scrollDeltaY"] = lateFloatKeys ["scrollDeltaY"];
		lateFloatKeys ["scrollDeltaY"] = 0.0f;

		List <KeyCode> keyList = new List<KeyCode> (pressKeys.Keys);
		foreach (KeyCode key in keyList) {
			pressKeys [key] = latePressKeys [key];
			latePressKeys [key] = false;
		}

		keyList = new List<KeyCode> (downKeys.Keys);
		foreach (KeyCode key in keyList) {
			downKeys [key] = lateDownKeys [key];
			lateDownKeys [key] = false;
		}
	}

	public float GetFloatKey (string key) {
		if (floatKeys.ContainsKey (key)) {
			return floatKeys [key];
		}
		return 0.0f;
	}

	public void SetFloatKey (string key, float value) {
		if (lateFloatKeys.ContainsKey (key)) {
			lateFloatKeys [key] += value;
		}
	}

	public bool GetKeyPress (KeyCode key) {
		if (pressKeys.ContainsKey (key)) {
			return pressKeys [key];
		}
		return false;
	}

	public bool GetKeyDown (KeyCode key) {
		if (downKeys.ContainsKey (key)) {
			return downKeys [key];
		}
		return false;
	}

	public void SetKeyPress (KeyCode key) {
		if (latePressKeys.ContainsKey (key)) {
			latePressKeys [key] = true;
		}
	}

	public void SetKeyDown (KeyCode key) {
		if (lateDownKeys.ContainsKey (key)) {
			lateDownKeys [key] = true;
		}
	}
}

public class Menu : MonoBehaviour {

	public TouchMenuController touchController;

	private bool focus;
	private bool paused;
	private bool actionView;
	private int previousTouch;
	private Canvas canvas;
	private Button[] buttons;

	void Start () {
		canvas = GetComponent<Canvas> ();
		buttons = GetComponentsInChildren<Button> ();

		previousTouch = 0;
		Disable ();
	}

	void Update () {
		if (paused) {
			if (focus) {
				if (UniverseBehavior.IB.GetKeyDown (KeyCode.Escape)) {
					Disable ();
				} else if (UniverseBehavior.IB.GetKeyDown (KeyCode.Q)) {
					QuitGame ();
				}
			} else if (actionView) {
				if (UniverseBehavior.IB.GetKeyDown (KeyCode.Escape) ||
				    UniverseBehavior.IB.GetKeyDown (KeyCode.Space)) {
					DisableActionView ();
				}
				if (touchController.conf.touchFriendly) {
					int touches = Input.touchCount;
					if (Input.GetMouseButton (1)) {
						touches += 2;
					}
					if (previousTouch - touches < -1) {
						if (touchController.isHidden ()) {
							touchController.UpdateFromConfig ();
						} else {
							touchController.Hide ();
						}
					}
					previousTouch = touches;
				}
			}
		} else if (!paused) {
			if (UniverseBehavior.IB.GetKeyDown (KeyCode.Escape)) {
				Enable ();
			}
		} 
	}

	void Hide () {
		canvas.enabled = false;
		DisableButtons ();
	}

	void Show () {
		canvas.enabled = true;
		EnableButtons ();
	}

	public bool isPaused () {
		return paused;
	}

	public bool isActionView () {
		return actionView;
	}

	public void Enable () {
		Time.timeScale = 0.0f;
		focus = true;
		paused = true;
		actionView = false;
		Show ();
	}

	public void Disable () {
		Time.timeScale = 1.0f;
		focus = false;
		paused = false;
		actionView = false;
		Hide ();
	}

	public void EnableActionView () {
		Hide ();
		actionView = true;
	}

	public void DisableActionView () {
		Show ();
		touchController.UpdateFromConfig ();
		actionView = false;
	}

	public void EnableButtons () {
		for (int i = 0; i < buttons.Length; i++) {
			buttons [i].interactable = true;
		}
		focus = true;
	}

	public void DisableButtons () {
		for (int i = 0; i < buttons.Length; i++) {
			buttons [i].interactable = false;
		}
		focus = false;
	}

	public void QuitGame () {
		Application.Quit ();
	}
}
