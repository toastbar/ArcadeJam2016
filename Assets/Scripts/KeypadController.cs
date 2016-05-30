using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class KeypadController : MonoBehaviour {

    public string comPort;

    private SerialPort serial;

    // Use this for initialization
    void Awake () {
        try
        {
            serial = new SerialPort(comPort, 38400, Parity.Even, 8, StopBits.One);
            serial.Open();
            serial.Write("o*\n");
            serial.Write("s0s00ff00\n");
            serial.Write("s1s00ffff\n");
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
            serial = null;
        }
	}

    void OnDestroy()
    {
        if (serial != null)
        {
            serial.Close();
        }
    }

    public void SetLedState(bool[] ledStates)
    {
        if (serial == null)
            return;

        Debug.Log(ledStates);
        for (int i = 0; i < 16; i++)
        {
            if (ledStates.Length > i)
            {
                serial.Write(string.Format("{0}{1:X}\n", ledStates[i] ? 'o' : 'f', i));
            }
        }
    }
}
