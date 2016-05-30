using UnityEngine;
using System.Collections;

public class JoyDebugger : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        for (int i = 1; i <= 8; ++i)
        {
            if (Input.GetButtonDown(string.Format("debug{0}", i)))
            {
                Debug.Log(string.Format("Joystick {0}", i));
            }
        }
	}
}
