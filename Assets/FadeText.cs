using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeText : MonoBehaviour {

	public float visibilityTime = 2f;
	public float fadeSpeed = 0.6f;
	public bool showAtStart = false;
	public bool autoShowOnChange = true;

	private float shown;
	private Text text;

	void Awake () {
		text = GetComponent<Text> ();
		shown = 0f;

		SetAlpha (0f);

		if (showAtStart) {
			Show ();
		}
	}

	void Start () {
	}

	void Update () {
		if (shown > 0f) {
			shown -= Time.unscaledDeltaTime;
		} else if (text.color.a > 0f) {
			UpdateAlpha (-fadeSpeed);
		}
	}

	void UpdateAlpha (float change) {
		Color color = text.color;
		color.a += change * Time.unscaledDeltaTime;
		text.color = color;
	}

	void SetAlpha (float value) {
		Color color = text.color;
		color.a = value;
		text.color = color;
	}

	public void SetText (string message) {
		text.text = message;
		if (autoShowOnChange) {
			Show ();
		}
	}

	public void Show () {
		shown = visibilityTime;
		SetAlpha (1.0f);
	}
}
