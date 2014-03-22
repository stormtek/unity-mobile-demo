using UnityEngine;
using System.Collections;

public static class Resources {
	public static Vector3 InvalidPosition = new Vector3(-9999, -9999, -9999);
	public static float zoomSpeed = 15.0f;
	public static float panSpeed = 15.0f;
	public static float rotateSpeed = 10.0f;
	public static  float waitTime = 0.1f;
	public static  int edgeBounds = 50;
	public static Rect leftZone { get { return new Rect(0, 0, edgeBounds, Screen.height); } }
	public static Rect rightZone { get { return new Rect(Screen.width - edgeBounds, 0, edgeBounds, Screen.height); } }
	public static Rect topZone { get { return new Rect(0, Screen.height - edgeBounds, Screen.width, edgeBounds); } }
	public static Rect bottomZone { get { return new Rect(0, 0, Screen.width, edgeBounds); } }
}
