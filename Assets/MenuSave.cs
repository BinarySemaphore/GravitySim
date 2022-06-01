using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class JsonPrettyString {
	public string data = "";

	public JsonPrettyString (string json) {
		bool spaceOrNewLine = false;
		char cur = '\0';
		int indLevel = 0;
		string indLevelStr = "    ";
		string newLineAdd = "\"";
		string spaceAdd = ":,";
		string indLevelAdd = "{[";
		string indLevelSub = "}]";

		data = "";
		for (int i = 0; i < json.Length; i++) {
			cur = json [i];

			if (spaceOrNewLine) {
				if (newLineAdd.IndexOf (cur) >= 0) {
					data += '\n';
					for (int j = 0; j < indLevel; j++) {
						data += indLevelStr;
					}
				} else if (indLevelSub.IndexOf (cur) >= 0) {
					data += '\n';
					for (int j = 0; j < indLevel; j++) {
						data += indLevelStr;
					}
				} else {
					data += ' ';
				}
				spaceOrNewLine = false;
			}

			if (indLevelSub.IndexOf (cur) >= 0) {
				indLevel--;
				data += '\n';
				for (int j = 0; j < indLevel; j++) {
					data += indLevelStr;
				}
			}

			data += cur;

			if (indLevelAdd.IndexOf (cur) >= 0) {
				indLevel++;
				data += '\n';
				for (int j = 0; j < indLevel; j++) {
					data += indLevelStr;
				}
			} else if (spaceAdd.IndexOf (cur) >= 0) {
				spaceOrNewLine = true;
			}
		}
	}
}

[System.Serializable]
public class SaveFileJsonData {
	public UniverseJsonData universeSettings;
	public ParticleJson[] Particles;
}

public class MenuSave : MonoBehaviour {

	public string saveDirectory = "./saves/";
	public string saveFileType = ".json";
	public string saveNameValid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_ ";
	public UniverseBehavior universe;

	private Canvas canvas;
	private Button saveButton;
	private InputField saveField;
	private Dropdown saveSelect;

	void Start () {
		canvas = GetComponent<Canvas> ();
		saveButton = GetComponentInChildren<Button> ();
		saveField = GetComponentInChildren<InputField> ();
		saveSelect = GetComponentInChildren<Dropdown> ();

		saveField.onValidateInput = onValidInput;

		Disable ();
	}

	void Update () {
	}

	void SetNewSaveString () {
		saveField.text = System.DateTime.Now.ToString ("yyyyMMdd_HHmmss");
	}

	void SaveButtonOverride () {
		saveButton.GetComponentInChildren<Text> ().text = "Override";
		ColorBlock colors = saveButton.colors;
		Color color = new Color (0.976f, 0.804f, 0.843f);
		colors.highlightedColor = color;
		saveButton.colors = colors;
	}

	void SaveButtonNew () {
		saveButton.GetComponentInChildren<Text> ().text = "Save";
		ColorBlock colors = saveButton.colors;
		Color color = new Color (0.804f, 0.976f, 0.855f);
		colors.highlightedColor = color;
		saveButton.colors = colors;
	}

	void PopulateSavedFiles () {
		Dropdown.OptionData item;
		string [] saveFiles = Directory.GetFiles (saveDirectory);
		List<Dropdown.OptionData> options = new List<Dropdown.OptionData> ();

		saveSelect.ClearOptions ();

		item = new Dropdown.OptionData ();
		item.text = "+ New Save";
		options.Add (item);

		for (int i = 0; i < saveFiles.Length; i++) {
			if (saveFiles [i].EndsWith (saveFileType)) {
				item = new Dropdown.OptionData ();
				item.text = saveFiles [i].Split ('/') [2].TrimEnd (saveFileType.ToCharArray ());
				options.Add (item);
			}
		}

		saveSelect.AddOptions (options);
	}

	public void Enable () {
		canvas.enabled = true;
		SetNewSaveString ();
		PopulateSavedFiles ();
	}

	public void Disable () {
		canvas.enabled = false;
	}

	public void UpdateInputField () {
		string text = saveSelect.captionText.text;
		if (text == "+ New Save") {
			SetNewSaveString ();
			SaveButtonNew ();
		} else {
			saveField.text = text;
			SaveButtonOverride ();
		}

	}

	public void Save () {
		string filename = saveDirectory + saveField.text + saveFileType;
		JsonPrettyString dataPretty = null;
		SaveFileJsonData data = new SaveFileJsonData ();

		data.universeSettings = universe.GetJsonData ();
		data.Particles = universe.GetParticlesJsonData ();

		try {
			dataPretty = new JsonPrettyString (JsonUtility.ToJson (data));
			using (StreamWriter w = new StreamWriter (filename)) {
				w.Write (dataPretty.data);
				w.Flush ();
				w.Close ();
			}
		} catch (System.Exception e) {
			Debug.LogError (e.ToString ());
		}
	}

	public char onValidInput (string text, int charIndex, char addedChar) {
		if (saveNameValid.IndexOf (addedChar) >= 0) {
			return addedChar;
		}
		return '\0';
	}
}


// indLevel: 2
// spaceOrNewLine: true