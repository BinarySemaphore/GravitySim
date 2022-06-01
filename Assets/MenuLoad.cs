using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class MenuLoad : MonoBehaviour {

	public string saveDirectory = "./saves/";
	public string saveFileType = ".json";
	public UniverseBehavior universe;

	private Canvas canvas;
	private Button loadButton;
	private Dropdown loadSelect;

	void Start () {
		canvas = GetComponent<Canvas> ();
		loadButton = GetComponentInChildren<Button> ();
		loadSelect = GetComponentInChildren<Dropdown> ();

		Disable ();
	}

	void Update () {
	}

	void PopulateSavedFiles () {
		Dropdown.OptionData item;
		string [] saveFiles = Directory.GetFiles (saveDirectory);
		List<Dropdown.OptionData> options = new List<Dropdown.OptionData> ();

		loadSelect.ClearOptions ();

		for (int i = 0; i < saveFiles.Length; i++) {
			if (saveFiles [i].EndsWith (saveFileType)) {
				item = new Dropdown.OptionData ();
				item.text = saveFiles [i].Split ('/') [2].TrimEnd (saveFileType.ToCharArray ());
				options.Add (item);
			}
		}

		if (options.Count > 0) {
			loadButton.interactable = true;
		} else {
			loadButton.interactable = false;
		}

		loadSelect.AddOptions (options);
	}

	public void Enable () {
		canvas.enabled = true;
		PopulateSavedFiles ();
	}

	public void Disable () {
		canvas.enabled = false;
	}

	public void Load () {
		string filename = saveDirectory + loadSelect.captionText.text + saveFileType;
		SaveFileJsonData data = null;

		try {
			using (StreamReader r = new StreamReader (filename)) {
				data = JsonUtility.FromJson<SaveFileJsonData> (r.ReadToEnd ());
				r.Close ();
			}

			if (data != null && data.universeSettings != null && data.Particles != null) {
				universe.CreateFromData (data.universeSettings, data.Particles);
			}

		} catch (System.Exception e) {
			Debug.LogError (e.ToString ());
		}
	}
}
