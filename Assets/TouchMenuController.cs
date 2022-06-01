using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

[System.Serializable]
public class TouchConfig {
	public bool touchFriendly = false;
	public bool touchMenu = false;
	public bool touchMovementControls = false;
	public bool touchZoomAndTimeControls = false;
}

public class TouchMenuController : MonoBehaviour {

	public TouchConfig conf;
	public TouchMenu touchMenu;
	public GameObject[] otherTouchDependants;

	private bool hidden;
	private Button button;

	private string configFilename = "./settings.json";

	void Awake () {
		button = GetComponent<Button> ();
		hidden = false;
	}

	void Start () {
		LoadSettingsFile ();
		UpdateFromConfig ();
	}

	void Update () {
	}

	private void LoadSettingsFile () {
		TouchConfig data = null;

		try {
			using (StreamReader r = new StreamReader (configFilename)) {
				data = JsonUtility.FromJson<TouchConfig> (r.ReadToEnd ());
				r.Close ();
			}

			SetStateJson (data);
		}
		catch (System.Exception e) {
			Debug.Log ("Settings Using Defaults: " + e.ToString ());
		}
	}

	public void Hide () {
		button.gameObject.SetActive (false);
		touchMenu.SetEnableBy (false, false, false);
		hidden = true;
	}

	public void Show () {
		UpdateFromConfig ();
	}

	public bool isHidden () {
		return hidden;
	}

	public void ToggleTouchMenu () {
		conf.touchMenu = !conf.touchMenu;
		UpdateFromConfig ();
	}

	public void ToggleTouchMovementControls () {
		conf.touchMovementControls = !conf.touchMovementControls;
		UpdateFromConfig ();
	}

	public void ToggleTouchZoomAndTimeControls () {
		conf.touchZoomAndTimeControls = !conf.touchZoomAndTimeControls;
		UpdateFromConfig ();
	}

	public void UpdateFromConfig () {
		button.gameObject.SetActive (conf.touchFriendly);
		touchMenu.SetEnableBy (
			conf.touchMenu,
			conf.touchMovementControls,
			conf.touchZoomAndTimeControls
		);
		hidden = false;

		for (int i = 0; i < otherTouchDependants.Length; i++) {
			otherTouchDependants [i].SetActive (conf.touchFriendly);
		}
	}

	public void SetStateJson (TouchConfig data) {
		conf = data;
	}

	public TouchConfig GetStateJson () {
		
		return conf;
	}
}
