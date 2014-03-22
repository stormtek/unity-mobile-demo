using UnityEngine;
using System.Collections;

public class UserInput : MonoBehaviour {

	private GameManager gameManager;
	private float timeSinceClickDetected = 0.0f;
	private bool handleClick = false, startedPan = false;
	private Vector3 detectedClickPosition = Resources.InvalidPosition;
	private Vector3 lastClickPosition = Resources.InvalidPosition;
	private float distanceBetweenTwoTouches = 0.0f;

	void Start () {
		gameManager = transform.root.GetComponent<GameManager>();
	}

	void Update () {
		//detect click and schedule HandleClick()
		// - delay the call of this by x milliseconds
		//if click drag is detected before HandleClick() is called do not run it
		ManageCamera();
		if(timeSinceClickDetected >= Resources.waitTime && handleClick) {
			HandleClick();
			handleClick = false;
		}
		timeSinceClickDetected += Time.deltaTime;
	}

	private void ManageCamera() {
		if(Input.touchCount == 1 || Input.GetMouseButtonDown(0)) {
			SingleTouch();
		} else if(Input.touchCount == 2) {
			PinchZoom();
		}
	}

	private void SingleTouch() {
		bool detectedClick = false, clickDrag = false;
		Vector3 clickPosition = Resources.InvalidPosition;
		foreach(Touch touch in Input.touches) {
			if(touch.phase == TouchPhase.Began) {
				detectedClick = true;
				clickPosition = new Vector3(touch.position.x, touch.position.y, 0.0f);
			} else if(touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) {
				if(timeSinceClickDetected > 0.9 * Resources.waitTime) {
					clickDrag = true;
					clickPosition = new Vector3(touch.position.x, touch.position.y, 0.0f);
				}
			} else if(touch.phase == TouchPhase.Ended) {
				startedPan = false;
				clickDrag = false;
				lastClickPosition = Resources.InvalidPosition;
			}
		}
		if(Input.GetMouseButtonDown(0)) {
			detectedClick = true;
			clickPosition = Input.mousePosition;
		}
		if(clickPosition == Resources.InvalidPosition) return;
		if(detectedClick) {
			handleClick = true;
			detectedClickPosition = clickPosition;
			timeSinceClickDetected = 0.0f;
		} else if(clickDrag) {
			handleClick = false;
			detectedClickPosition = Resources.InvalidPosition;
		}
		if(clickDrag) {
			Vector2 screenPosition = new Vector2(clickPosition.x, clickPosition.y);
			bool movingCamera = false;
			Vector3 movement = new Vector3(0, 0, 0);
			if(Resources.leftZone.Contains(screenPosition)) {
				movement.x -= Resources.panSpeed;
				movingCamera = true;
			}
			if(Resources.rightZone.Contains(screenPosition)) {
				movement.x += Resources.panSpeed;
				movingCamera = true;
			}
			if(Resources.topZone.Contains(screenPosition)) {
				movement.z += Resources.panSpeed;
				movingCamera = true;
			}
			if(Resources.bottomZone.Contains(screenPosition)) {
				movement.z -= Resources.panSpeed;
				movingCamera = true;
			}
			if(movingCamera) {
				Vector3 origin = Camera.main.transform.position;
				Vector3 destination = origin;
				movement = Camera.main.transform.TransformDirection(movement);
				destination.x += movement.x;
				destination.z += movement.z;
				Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Resources.panSpeed * Time.deltaTime);
				return;
			}
			if(startedPan) { //rotating camera
				float xDrag = lastClickPosition.x - clickPosition.x;
				Camera.main.transform.RotateAround(Camera.main.transform.position, Vector3.up, xDrag * Resources.rotateSpeed * Time.deltaTime);
				lastClickPosition = clickPosition;
			} else {
				if(lastClickPosition == Resources.InvalidPosition) {
					lastClickPosition = clickPosition;
					startedPan = true;
				}
			}
		}
	}

	private void PinchZoom() {
		Vector2 touch1 = Input.touches[0].position, touch2 = Input.touches[1].position;
		if(Input.touches[0].phase == TouchPhase.Began && Input.touches[1].phase == TouchPhase.Began) {
			distanceBetweenTwoTouches = Mathf.Abs((touch1 - touch2).magnitude);
			handleClick = false;
		}
		bool one = Input.touches[0].phase == TouchPhase.Moved || Input.touches[0].phase == TouchPhase.Stationary;
		bool two = Input.touches[1].phase == TouchPhase.Moved || Input.touches[1].phase == TouchPhase.Stationary;
		bool neither = Input.touches[0].phase == TouchPhase.Stationary && Input.touches[1].phase == TouchPhase.Stationary;
		if(one || two && !neither) {
			handleClick = false;
			float newDistanceBetweenTwoTouches = Mathf.Abs((touch1 - touch2).magnitude);
			float difference = newDistanceBetweenTwoTouches - distanceBetweenTwoTouches;
			// difference < 0 = zoomOut, difference > 0 = zoomIn
			Vector3 origin = Camera.main.transform.position;
			Vector3 destination = origin;
			Vector3 movement = new Vector3(0, 0, difference * Resources.zoomSpeed);
			movement = Camera.main.transform.TransformDirection(movement);
			destination.x += movement.x;
			destination.y += movement.y;
			destination.z += movement.z;
			Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Resources.zoomSpeed * Time.deltaTime);
			distanceBetweenTwoTouches = newDistanceBetweenTwoTouches;
		}
	}

	private void HandleClick() {
		if(detectedClickPosition == Resources.InvalidPosition) return;
		GameObject hitObject = FindHitObject(detectedClickPosition);
		Vector3 hitPoint = FindHitPoint(detectedClickPosition);
		if(gameManager) gameManager.GetActivePlayer().HandleClick(hitObject, hitPoint);
	}

	private GameObject FindHitObject(Vector3 screenPosition) {
		Ray ray = Camera.main.ScreenPointToRay(screenPosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit)) return hit.collider.gameObject;
		else return null;
	}
	
	private Vector3 FindHitPoint(Vector3 screenPosition) {
		Ray ray = Camera.main.ScreenPointToRay(screenPosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit)) return hit.point;
		else return Resources.InvalidPosition;
	}
}
