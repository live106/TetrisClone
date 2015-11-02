using UnityEngine;
using System.Collections;

public class Group : MonoBehaviour
{
	float lastFall = 0;
	float tick = 0.5f;

	private Vector3 distanceVector;
	private Vector3 lastPosition;
	private bool trackMouse = false;

	private Vector3 mousePosition;
	private float moveSpeed = 0.1f;

	public SwipeDetector swipeDetector;

	void Start ()
	{
		if (!isValidGridPos ()) {
			Debug.Log ("Game Over !");
			Destroy (gameObject);
		}
	}
	
	void Update ()
	{
		if (swipeDetector.lastSwipe == SwipeDetector.SwipeDirection.Up) {
			transform.Rotate (0, 0, -90);
			if (isValidGridPos ()) {
				updateGrid ();
			} else {
				transform.Rotate (0, 0, 90);
			}
			swipeDetector.reset();
		}
//		if (Input.GetAxis("Mouse Y") >=0.3) {
//			transform.Rotate (0, 0, -90);
//			if (isValidGridPos ()) {
//				updateGrid ();
//			} else {
//				transform.Rotate (0, 0, 90);
//			}
//			//Input.ResetInputAxes();
//		} else 
//			if (Input.touchCount >= 2) 
//		{
//			while (isValidGridPos())
//			{
//				transform.position += new Vector3 (0, -1, 0);
//			}
//			transform.position += new Vector3 (0, 1, 0);
//			updateGrid ();
//			Grid.deleteFullRows ();
//			FindObjectOfType<Spawner> ().spawnNext ();
////			Input.ResetInputAxes();
//			enabled = false;
//		}
		else if (Input.GetButtonDown ("Fire1"))
		{
			trackMouse = true;
			lastPosition = Input.mousePosition;
		}

		if (Time.time - lastFall >= tick) 
		{
			transform.position += new Vector3 (0, -1, 0);
			if (isValidGridPos ()) {
				updateGrid ();
			} else {
				transform.position += new Vector3 (0, 1, 0);
				
				Grid.deleteFullRows ();
				
				FindObjectOfType<Spawner> ().spawnNext ();
				
				enabled = false;
				swipeDetector.enabled = false;
			}
			lastFall = Time.time;
		}

		if (Input.GetButtonUp ("Fire1"))
		{
			trackMouse = false;
//			Debug.Log ("Mouse moved " + mouseDistance + " while button was down.");
			distanceVector = new Vector3(0, 0, 0);
		}
		if (trackMouse)
		{
			Vector3 newPosition = Input.mousePosition;
			// If you just want the x-axis:
			distanceVector += (Camera.main.ScreenToWorldPoint(newPosition) - Camera.main.ScreenToWorldPoint(lastPosition));
			// If you just want the y-axis,change newPosition.x to newPosition.y and lastPosition.x to lastPosition.y
			// If you want the entire distance moved (not just the X-axis, use:
			//mouseDistance += (newPosition - lastPosition).magnitude;
			lastPosition = newPosition;

			transform.position += new Vector3(Mathf.Floor(distanceVector.x), 0, 0);

			if (isValidGridPos ()) {
				updateGrid ();
			} else {
				transform.position -= new Vector3(Mathf.Floor(distanceVector.x), 0, 0);
			}

//			Debug.Log("distance : " + distanceVector.x);

			distanceVector.x -= Mathf.Floor(distanceVector.x);
			distanceVector.y = 0;
			distanceVector.z = 0;
		}

		Update0 ();
	}

	void Update0 ()
	{
//		if (Input.GetKeyDown (KeyCode.Mouse0)) {
//			mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
//			transform.position = Vector2.Lerp(transform.position, mousePosition, moveSpeed);
//		} else

		if (Input.GetKeyDown (KeyCode.LeftArrow)) {
			transform.position += new Vector3 (-1, 0, 0);
			if (isValidGridPos ()) {
				updateGrid ();
			} else {
				transform.position += new Vector3 (1, 0, 0);
			}
		} else if (Input.GetKeyDown (KeyCode.RightArrow)) {
			transform.position += new Vector3 (1, 0, 0);
			if (isValidGridPos ()) {
				updateGrid ();
			} else {
				transform.position += new Vector3 (-1, 0, 0);
			}
		} else if (Input.GetKeyDown (KeyCode.UpArrow)) {
			transform.Rotate (0, 0, -90);
			if (isValidGridPos ()) {
				updateGrid ();
			} else {
				transform.Rotate (0, 0, 90);
			}
		} else if (Input.GetKeyDown (KeyCode.DownArrow)) {
			while (isValidGridPos()) {
				transform.position += new Vector3 (0, -1, 0);
			}
			transform.position += new Vector3 (0, 1, 0);
			updateGrid ();
			Grid.deleteFullRows ();
			FindObjectOfType<Spawner> ().spawnNext ();
//			Input.ResetInputAxes();
			enabled = false;
		}
//		else if (Input.GetKeyDown (KeyCode.DownArrow) || Time.time - lastFall >= tick) {
//			transform.position += new Vector3 (0, -1, 0);
//			if (isValidGridPos ()) {
//				updateGrid ();
//			} else {
//				transform.position += new Vector3 (0, 1, 0);
//
//				Grid.deleteFullRows ();
//
//				FindObjectOfType<Spawner> ().spawnNext ();
//
//				enabled = false;
//			}
//			lastFall = Time.time;
//		}
	}

	bool isValidGridPos ()
	{
		foreach (Transform child in transform) {
			Vector2 v = Grid.roundVec (child.position);

			if (!Grid.insideBorder (v)) {
				return false;
			}

			if (Grid.grid [(int)v.x, (int)v.y] != null && Grid.grid [(int)v.x, (int)v.y].parent != transform) {
				return false;
			}
		}
		return true;
	}

	void updateGrid ()
	{
		for (int y = 0; y < Grid.h; y++) {
			for (int x = 0; x < Grid.w; x++) {
				if (Grid.grid [x, y] != null) {
					if (Grid.grid [x, y].parent == transform) {
						Grid.grid [x, y] = null;
					}
				}
			}
		}
		foreach (Transform child in transform) {
			Vector2 v = Grid.roundVec (child.position);
			Grid.grid [(int)v.x, (int)v.y] = child;
		}
	}
	
}
