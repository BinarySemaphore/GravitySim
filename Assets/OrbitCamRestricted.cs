using UnityEngine;
using System.Collections;

public class OrbitCamRestricted : MonoBehaviour {

	public float speed = 20.0f;
	public float zoomSpeed = 5.0f;
	public float zoomMin = -1.0f;
	public float zoomMax = -10000.0f;
	public Transform camTransform;
	public Menu menu;

	private bool orbitDelta;
	private bool targetting;
	private Vector3 mouseDelta;
	private GameObject target;

	// Use this for initialization
	void Start () {
		orbitDelta = false;
		targetting = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!menu.isPaused () || menu.isActionView ()) {
			Control ();
		}
	}

	void Control () {
		Orbit ();
		
		if (UniverseBehavior.IB.GetKeyDown (KeyCode.R)) {
			Reset ();
		} else if (UniverseBehavior.IB.GetKeyDown (KeyCode.F)) {
			UnsetTarget ();
		}

		if (targetting) {
			if (target != null && target.activeSelf) {
				SetLocation (target.transform.position);
			} else {
				UnsetTarget ();
			}
		} else {
			Move ();
		}
	}

	void Move () {
		float forward = 0.0f;
		float sideways = 0.0f;
		float vertical = 0.0f;

		if (UniverseBehavior.IB.GetKeyPress (KeyCode.W)) {
			forward += speed;
		}
		if (UniverseBehavior.IB.GetKeyPress (KeyCode.S)) {
			forward -= speed;
		}
		if (UniverseBehavior.IB.GetKeyPress (KeyCode.D)) {
			sideways += speed;
		}
		if (UniverseBehavior.IB.GetKeyPress (KeyCode.A)) {
			sideways -= speed;
		}
		if (UniverseBehavior.IB.GetKeyPress (KeyCode.LeftShift)) {
			vertical += speed;
		}
		if (UniverseBehavior.IB.GetKeyPress (KeyCode.LeftControl)) {
			vertical -= speed;
		}

		if (forward != 0.0f || sideways != 0.0f || vertical != 0.0f) {
			Vector3 posDelta = new Vector3 (sideways, vertical, forward);
			posDelta *= Time.unscaledDeltaTime * (-0.1f * camTransform.localPosition.z);
			posDelta = transform.rotation * posDelta;
			transform.position += posDelta;
		}
	}

	void Orbit () {
		float scroll = UniverseBehavior.IB.GetFloatKey("scrollDeltaY");

		if (Input.GetMouseButton (0)) {
			if (!orbitDelta) {
				mouseDelta = Input.mousePosition;
				orbitDelta = true;
			} else {
				mouseDelta = (Input.mousePosition - mouseDelta) * Time.unscaledDeltaTime * speed * 2;
				transform.RotateAround (transform.position, Vector3.up, mouseDelta.x);
				transform.Rotate(new Vector3(-mouseDelta.y, 0.0f, 0.0f));
				orbitDelta = false;
			}
		} else if (orbitDelta) orbitDelta = false;

		if (scroll != 0.0f) {
			Vector3 position = camTransform.localPosition;

			position.z += position.z * scroll * Time.unscaledDeltaTime * -zoomSpeed;
			if (position.z > zoomMin) {
				position.z = zoomMin;
			}
			if (position.z < zoomMax) {
				position.z = zoomMax;
			}

			camTransform.localPosition = position;
		}
	}

	public void Reset () {
		UnsetTarget ();
		SetLocation (Vector3.zero);
	}

	public void SetLocation (Vector3 location) {
		transform.position = location;
	}

	public GameObject GetTarget () {
		if (targetting) {
			return target;
		}
		return null;
	}

	public void SetTarget (GameObject target) {
		targetting = true;
		SetLocation (Vector3.zero);
		this.target = target;
	}

	public void UnsetTarget () {
		targetting = false;
	}
}
