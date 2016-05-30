using UnityEngine;
using System.Collections;
using System.IO.Ports;

public class JoypadController : MonoBehaviour
{
    public string comPort;

    private SerialPort serial;

    // Use this for initialization
    void Awake()
    {
        try
        {
            serial = new SerialPort(comPort, 38400, Parity.Even, 8, StopBits.One);
            serial.Open();
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

    public void SetColorState(Color[] colors)
    {
        if (serial == null)
            return;

        for (int i = 0; i < 4; i++)
        {
            if (colors.Length > i)
            {
                Color color = colors[i];
                string command = System.String.Format("s{0:X1}s{1:X2}{2:X2}{3:X2}\n", i, (byte)(color.r * 255), (byte)(color.g * 255), (byte)(color.b * 255));
                serial.Write(command);
            }
        }
    }
}

