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
            serial.Write("s0s0000ff\n");
            serial.Write("s1s0000ff\n");
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

        for (int i = 0; i < 16; i++)
        {
            if (ledStates.Length > i)
            {
                serial.Write(string.Format("{0}{1:X}\n", ledStates[i] ? 'o' : 'f', i));
            }
        }
    }

    public void SetArrowState(float left, float horiz, float right, float up, float vert, float down)
    {
        if (serial == null)
            return;

        serial.Write(string.Format("s0s{0:X2}{1:X2}{2:X2}\n", (byte)(right * 127), (byte)(left * 127), (byte)(horiz * 127)));
        serial.Write(string.Format("s1s{0:X2}{1:X2}{2:X2}\n", (byte)(up * 127), (byte)(down * 127), (byte)(vert * 127)));
    }
}
