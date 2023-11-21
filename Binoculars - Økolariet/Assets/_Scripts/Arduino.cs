using UnityEngine;
using System.IO.Ports;
using System.Globalization;
using System;

public class Arduino : MonoBehaviour
{
    public static event EventHandler Jump;
    SerialPort arduino;

    void Start()
    {
        arduino = new SerialPort("COM12", 9600);
        arduino.ReadTimeout = 25;
        arduino.Open();
        if (arduino.IsOpen)
        {
            Debug.Log("Opened");
        }
        else
        {
            Debug.Log("Closed");
        }
    }

    void Update()
    {
        if (arduino.IsOpen)
        {
            try
            {
                string dataString = arduino.ReadLine();
                string[] values = dataString.Split(',');
                //Debug.Log(values[0]);
                //Debug.Log(values[1]);
                float x = float.Parse(values[0], CultureInfo.InvariantCulture.NumberFormat);
                float y = float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat);
                int s = int.Parse(values[2]);
                //Debug.Log(x);
                //Debug.Log(y);

                Quaternion q = Quaternion.Euler(x, 0, 0);

                transform.rotation = q;
                if (s == 0)
                    Jump?.Invoke(this, null);
            }
            catch (System.Exception)
            {
                //Debug.Log("timeout");
            }
        }
    }
}