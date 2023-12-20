using System;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class SerialPortObject
{
    private SerialPort serialPort;
    public SerialPort SerialPort { get { return serialPort; } }
    private string[] dataChunk;
    public string[] DataChunk { get { return dataChunk; } }

    
    #region SerialPortConfig
    // Current com port and set of default
    private readonly string comPort;

    // Current baud rate and set of default 300, 600, 1200, 2400, 4800, 9600, 14400, 19200, 28800, 38400, 57600, 115200
    private readonly int baudRate;

    // The parity-checking protocol.
    private readonly Parity parity;

    // The standard number of stopbits per byte.
    private readonly StopBits stopBits;

    // The standard length of data bits per byte.
    private readonly int dataBits;

    // The state of the Data Terminal Ready(DTR) signal during serial communication.
    private readonly bool dtrEnable;

    // Whether or not the Request to Send(RTS) signal is enabled during serial communication.
    private readonly bool rtsEnable;

    //Sets the Handshake protocol for serial port transmission
    private readonly Handshake handshake;

    // Read and write timeouts
    private readonly int readTimeout;
    private readonly int writeTimeout;

    //End of line and seperator for notifying line end and where to seperate data
    private readonly string newLine;
    private readonly char seperator;
    #endregion SerialPortConfig

    // Thread for thread version of port
    Thread SerialLoopThread;
    private bool isRunning = false;

    /// <summary>
    /// Constructor with default values for all parameters
    /// </summary>
    /// <param name="ComPort"></param>
    /// <param name="BaudRate"></param>
    /// <param name="Parity"></param>
    /// <param name="DataBits"></param>
    /// <param name="StopBits"></param>
    /// <param name="DtrEnable"></param>
    /// <param name="RtsEnable"></param>
    /// <param name="Handshake"></param>
    /// <param name="ReadTimeout"></param>
    /// <param name="WriteTimeout"></param>
    /// <param name="NewLine"></param>
    /// <param name="Seperator"></param>
    public SerialPortObject(string ComPort = "COM9", int BaudRate = 9600, Parity Parity = Parity.None, int DataBits = 8, 
        StopBits StopBits = StopBits.One, bool DtrEnable = true, bool RtsEnable = true, Handshake Handshake = Handshake.None, 
        int ReadTimeout = 5000, int WriteTimeout = 5000, string NewLine = "\n", char Seperator = ',')
    {
        this.comPort = ComPort;
        this.baudRate = BaudRate;
        this.parity = Parity;
        this.dataBits = DataBits;
        this.stopBits = StopBits;
        this.readTimeout = ReadTimeout;
        this.writeTimeout = WriteTimeout;
        this.handshake = Handshake;
        this.dtrEnable = DtrEnable;
        this.rtsEnable = RtsEnable;
        this.newLine = NewLine;
        this.seperator = Seperator;

        serialPort = new SerialPort(comPort, baudRate, parity, dataBits, stopBits)
        {
            ReadTimeout = this.readTimeout,
            WriteTimeout = this.writeTimeout,

            Handshake = this.handshake,

            DtrEnable = this.dtrEnable,
            RtsEnable = this.rtsEnable,

            NewLine = this.newLine
        };
        Debug.Log("SerialPort at " + this.comPort + " is created");
    }
    
    ///<summary>
    ///Deconstructor - Responsible for closing the Serial Port
    ///</summary>
    ~SerialPortObject()
    {
        try
        {
            // Close the serial port
            SerialPort.Close();
            Debug.Log( this.comPort + " closed");
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
        StopSerialThreading();
    }

    /// <summary>
    /// Opens Serial Port
    /// </summary>
    public void OpenPort()
    {
        try
        {
            this.serialPort.Open();
            Debug.Log(this.comPort + " opened");
        }
        catch (System.Exception ex)
        {
            if(serialPort == null)
            {
                Debug.Log("Error 1A: " + "Serial Port not created!");
            }
            else if(serialPort.IsOpen == true)
            {
                Debug.Log("Error 1B: " + "Serial Port already opened!");
            }
            else
            {
                Debug.Log("Error 1C: " + ex.Message.ToString());
            }
        }

        StartSerialThread();

    }

    /// <summary>
    /// Closes Serial Port
    /// </summary>
    public void ClosePort()
    {
        try
        {
            // Close the serial port
            SerialPort.Close();
            Debug.Log(this.comPort + " closed");
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
        StopSerialThreading();
    }
    private void SerialThreadLoop()
    {
        while (isRunning)
        {
            if (isRunning == false)
                break;

            // Run the generic loop
            GenericSerialLoop();
        }
    }

    private void StartSerialThread()
    {
        isRunning = true;

        SerialLoopThread = new Thread(SerialThreadLoop);
        SerialLoopThread.Start();
    }

    /// <summary>
    /// Function used to stop the thread and "over" kill
    /// off any instance
    /// </summary>
    private void StopSerialThreading()
    {
        isRunning = false;

        // this should timeout the thread

        Thread.Sleep(100);

        // otherwise...

        if (SerialLoopThread != null && SerialLoopThread.IsAlive)
            SerialLoopThread.Abort();

        Thread.Sleep(100);

        if (SerialLoopThread != null)
            SerialLoopThread = null;
    }

    /// <summary>
    /// The serial thread loop & the coroutine loop both utilise
    /// the same code with the exception of the null return on 
    /// the coroutine, so we share it here.
    /// </summary>
    private void GenericSerialLoop()
    {
        try
        {
            // Check that the port is open. If not skip and do nothing
            if (SerialPort.IsOpen)
            {
                string data = string.Empty;
                data = serialPort.ReadLine();
                

                // If the data is valid then do something with it
                if (data != null && data != "")
                {
                    dataChunk = data.Split(this.seperator);
                }
            }
        }
        catch (TimeoutException)
        {
            // This will be triggered lots with the coroutine method
        }
        catch (Exception ex)
        {
            // This could be thrown if we close the port whilst the thread 
            // is reading data. So check if this is the case!
            if (SerialPort.IsOpen)
            {
                // Something has gone wrong!
                Debug.Log("Error 4: " + ex.Message.ToString());
            }
            else
            {
                // Error caused by closing the port whilst in use! This is 
                // not really an error but uncomment if you wish.

                Debug.Log("Error 5: Port Closed Exception!");
            }
        }
    }
}


