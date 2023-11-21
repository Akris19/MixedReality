using System;
using System.Globalization;

using UnityEngine;

public class SerialPortFactory : MonoBehaviour
{
    private SerialPortObject sp1;
    [SerializeField] private GameObject cyl;
    private float w, x, y, z;
    private int potValue = 0;
    private ImageRenderer imageRenderer;
    // Start is called before the first frame update
    void Start()
    {
        sp1 = new SerialPortObject();
        sp1.OpenPort();
        imageRenderer = GameObject.Find("RawImage").GetComponent<ImageRenderer>();
    }
    void Update()
    {
        if (sp1.DataChunk != null)
        {
            w = float.Parse(sp1.DataChunk[0], CultureInfo.InvariantCulture.NumberFormat);
            x = float.Parse(sp1.DataChunk[1], CultureInfo.InvariantCulture.NumberFormat);
            y = float.Parse(sp1.DataChunk[2], CultureInfo.InvariantCulture.NumberFormat);
            z = float.Parse(sp1.DataChunk[3], CultureInfo.InvariantCulture.NumberFormat) * -1;
            potValue = Helpers.Map(Int32.Parse(sp1.DataChunk[4], CultureInfo.InvariantCulture.NumberFormat), 0, 1023, 0, 2);
            
            imageRenderer.ChangeSprite(potValue);
            Quaternion q = new Quaternion(-x, z, -y, w).normalized;
            cyl.transform.rotation = q; 
        }
    }

    void OnApplicationQuit()
    {
        sp1.ClosePort();
    }
}
