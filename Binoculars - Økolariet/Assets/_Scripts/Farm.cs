using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : MonoBehaviour
{
    [SerializeField]
    private GameObject[] fields;



    void Start()
    {
        GenerateFields();
    }

    private void GenerateFields()
    {
        for (int i = -70; i <= 50; i += 10)
        {
            for(int j = 14; j <= 44; j += 10) 
            {
                Instantiate(fields[0], new Vector3(i, 1, j), Quaternion.identity, this.transform);
            }
        }

        for (int i = 50; i >= -70; i -= 10)
        {
            for (int j = -4; j >= -34; j -= 10)
            {
                Instantiate(fields[1], new Vector3(i, 1, j), Quaternion.identity, this.transform);
            }
        }
    }

}
