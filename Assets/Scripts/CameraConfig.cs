﻿using UnityEngine;
using System.Collections;

public class CameraConfig : MonoBehaviour
{

	
	float devHeight = 640 / 32f;
	float devWidth = 960 / 32f;

	// Use this for initialization
	void Start ()
	{
		float screenHeight = Screen.height;
	
		Debug.Log ("screenHeight = " + screenHeight);
	
		//this.GetComponent<Camera>().orthographicSize = screenHeight / 200.0f;
			
		float orthographicSize = this.GetComponent<Camera> ().orthographicSize;

		float aspectRatio = Screen.width * 1.0f / Screen.height;
	
		float cameraWidth = orthographicSize * 2 * aspectRatio;

		Debug.Log ("cameraWidth = " + cameraWidth);
		 
		if (cameraWidth < devWidth) {
			orthographicSize = devWidth / (2 * aspectRatio);
			Debug.Log ("new orthographicSize = " + orthographicSize);
			this.GetComponent<Camera> ().orthographicSize = orthographicSize;
		}

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
