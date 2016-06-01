using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using System;

public class OctoControllerMapper : MonoBehaviour {

    private Dictionary<int, string> controllerPortMapping;
    private string[] ports;
    private bool started = false;

	// Use this for initialization
	void Start () {
        ports = SerialPort.GetPortNames();
        Debug.Log(ports.Length);
        foreach (var port in ports)
        {
            Debug.Log(port);
        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine("PortId");
        }
    }

    IEnumerator PortId()
    {
        for (int portId = 0; portId < ports.Length; ++portId)
        {
            bool success = false;
            int id = -1;
            string portName = "";
            try
            {
                portName = ports[portId];
                Debug.Log(portName);
                var port = new SerialPort(portName, 38400, Parity.Even, 8, StopBits.One);
                port.NewLine = "\n";
                port.Open();
                port.ReadTimeout = 1000;
                port.WriteTimeout = 1000;
                port.Write("v\n");
                Debug.Log(port.ReadLine());
                port.Write("i\n");
                string s = port.ReadLine();
                s = port.ReadLine();
                Debug.Log(s);
                id = Convert.ToInt32(s.Split('i')[1], 16);
                Debug.Log(id);
                port.Close();
                success = true;
            }
            catch (System.Exception ex)
            {
                Debug.Log(ex.Message);
            }

            if (success)
            {
                int button;

                while ((button = CheckButtons()) < 0)
                {
                    yield return null;
                }

                Debug.Log(String.Format("Position: {0} Serial: {1} Joystick: {2}", id, portName, button));

                while ((button = CheckButtons()) > 0)
                {
                    yield return null;
                }

            }
        }

        Debug.Log("Done");
    }

    int CheckButtons()
    {
        if (Input.GetButton("debug1"))
            return 1;
        if (Input.GetButton("debug2"))
            return 2;
        if (Input.GetButton("debug3"))
            return 3;
        if (Input.GetButton("debug4"))
            return 4;
        if (Input.GetButton("debug5"))
            return 5;
        if (Input.GetButton("debug6"))
            return 6;
        if (Input.GetButton("debug7"))
            return 7;
        if (Input.GetButton("debug8"))
            return 8;

        return -1;
    }
}
