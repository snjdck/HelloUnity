using UnityEngine;
using UnityEditor;
using System.Collections;

public class ExtMenu {

	[MenuItem("Extension/Test")]
	static void Test () {
		Debug.Log ("Test");
	}
}
