using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class KeypadController : MonoBehaviour {

    public string comPort;

    private SerialPort serial;

    // Use this for initialization
    void Start () {
        serial = new SerialPort(comPort, 38400, Parity.Even, 8, StopBits.One);
        serial.Open();
        serial.Write("o*\n");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
