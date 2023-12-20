using UnityEngine;
using System.IO.Ports;
using System.Globalization;
using UnityEngine.UI;

public class Arduino : MonoBehaviour
{
    SerialPort arduino;

    [SerializeField]
    private Slider slider;
    void Start()
    {
        arduino = new SerialPort("COM9", 9600);
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
                float t = float.Parse(dataString, CultureInfo.InvariantCulture.NumberFormat);
                slider.value = t;
                River.Instance.T = t;

            }
            catch (System.Exception)
            {
                //Debug.Log("timeout");
            }
        }
    }
}