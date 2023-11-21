using UnityEngine;
using System;
using System.IO.Ports;

public class Serial : MonoBehaviour
{
    public static string ArduinoInputString { get; set; }
    //Cylinder
    [SerializeField] private GameObject cyl;
    //private float w, x, y, z = 0;
    #region SerialPort
    [Header("SerialPort")]

    public SerialPort SerialPort;

    // Current com port and set of default
    public string ComPort = "COM7";

    // Current baud rate and set of default 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600, 115200
    public int BaudRate = 115200;

    // The parity-checking protocol.
    public Parity Parity = Parity.None;

    // The standard number of stopbits per byte.
    public StopBits StopBits = StopBits.One;

    // The standard length of data bits per byte.
    public int DataBits = 8;

    // The state of the Data Terminal Ready(DTR) signal during serial communication.
    public bool DtrEnable;

    // Whether or not the Request to Send(RTS) signal is enabled during serial communication.
    public bool RtsEnable;

    //Sets the Handshake protocol for serial port transmission
    public Handshake Handshake = Handshake.None;
    
    // Read and write timeouts
    public int ReadTimeout = 10;
    public int WriteTimeout = 10;

    public string NewLine = "\n";
    #endregion SerialPort

    string dataIN;
    void Start()
    {
        // Initialise the serial port
        SerialPort = new SerialPort(ComPort, BaudRate, Parity, DataBits, StopBits)
        {
            ReadTimeout = ReadTimeout,
            WriteTimeout = WriteTimeout,

            Handshake = Handshake,

            DtrEnable = DtrEnable,
            RtsEnable = RtsEnable,

            NewLine = NewLine
    };

        // Open the serial port
        SerialPort.Open();

        if (SerialPort.IsOpen)
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
           if (SerialPort.IsOpen)
           {
               try
               {
                   string dataString = SerialPort.ReadLine();
                   Debug.Log(dataString);
                   /*string[] values = dataString.Split(',');

                   w = float.Parse(values[0], CultureInfo.InvariantCulture.NumberFormat);
                   x = float.Parse(values[1], CultureInfo.InvariantCulture.NumberFormat);
                   y = float.Parse(values[2], CultureInfo.InvariantCulture.NumberFormat);
                   z = float.Parse(values[3], CultureInfo.InvariantCulture.NumberFormat) * -1;

                   Quaternion q = new Quaternion(-x, z, -y, w).normalized;
                   cyl.transform.rotation = q;*/

               }
               catch (System.Exception)
               {
                   Debug.Log("timeout");
               }
           }
       }

    
    void OnApplicationQuit() 
    {
        try
        {
            // Close the serial port
            SerialPort.Close();
        }
        catch (System.Exception ex)
        {
            if (SerialPort == null || SerialPort.IsOpen == false)
            {
                // Failed to close the serial port. Uncomment if
                // you wish but this is triggered as the port is
                // already closed and or null.

                Debug.Log("Error 2A: " + "Port already closed!");
            }
            else
            {
                // Failed to close the serial port
                Debug.Log("Error 2B: " + ex.Message.ToString());
            }
        }

    }
}
