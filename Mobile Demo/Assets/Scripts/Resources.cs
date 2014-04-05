using UnityEngine;
using System.Collections;

public static class Resources {
	public static Vector3 InvalidPosition = new Vector3(-9999, -9999, -9999);
	public static float zoomSpeed = 15.0f;
	public static float panSpeed = 15.0f;
	public static float rotateSpeed = 10.0f;
	public static float waitTime = 0.1f;
	public static float lowSplit = 0.35f;
	public static float highSplit = 0.65f;
	public static int edgeBounds = 35;
	private static int padding = 10;
	private static int size = 50;
	public static Rect leftZone { get { return new Rect(edgeBounds, Screen.height - edgeBounds - padding - 2 * size, size, size); } }
	public static Rect rightZone { get { return new Rect(edgeBounds + 2 * padding + 2 * size, Screen.height - edgeBounds - padding - 2 * size, size, size); } }
	public static Rect bottomZone { get { return new Rect(edgeBounds + padding + size, Screen.height - edgeBounds - size, size, size); } }
	public static Rect topZone { get { return new Rect(edgeBounds + padding + size, Screen.height - edgeBounds - 2 * padding - 3 * size, size, size); } }
}
