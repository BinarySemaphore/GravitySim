using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler {

	public UnityEvent onPress;

	private bool over;
	private bool selected;

	void Start () {
		over = false;
		selected = false;
	}

	void Update () {
		
		if (selected) {
			onPress.Invoke ();
		}
	}

	public void OnPointerEnter (PointerEventData eventData) {
		over = true;
	}

	public void OnPointerDown (PointerEventData eventData) {
		selected = over;
	}

	public void OnPointerUp (PointerEventData eventData) {
		selected = false;
	}

	public void OnPointerExit (PointerEventData eventData) {
		selected = false;
		over = false;
	}

}
