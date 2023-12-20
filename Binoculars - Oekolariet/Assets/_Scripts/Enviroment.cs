using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public class Enviroment : MonoBehaviour
{
    [SerializeField]
    private GameObject[] preFabs;
    private List<GameObject> fieldsList = new List<GameObject>();
    private List<GameObject> grassList = new List<GameObject>();

    void Start()
    {
        GenerateEnviroment();
        ForestActive(0.5f);
    }

    private void GenerateEnviroment()
    {
        if (preFabs.Length <= 0)
            return;


        for (int j = 15; j <= 55; j += 10)
        {
            for (int i = -70; i <= 50; i += 10)
            {
                GameObject obj = Instantiate(preFabs[0], new Vector3(i, 1, j), Quaternion.identity, this.transform);
                obj.AddComponent<BoxCollider>();
                fieldsList.Add(obj);
            }
        }

        for (int j = -45; j <= -5; j += 10)
        {
            for (int i = 50; i >= -70; i -= 10)
            {
                GameObject obj = Instantiate(preFabs[1], new Vector3(i, 1, j), Quaternion.identity, this.transform);
                obj.AddComponent<BoxCollider>();
                fieldsList.Add(obj);
            }
        }

        for (int i = -70; i <= 50; i += 10)
        {
            for (int j = -35; j <= 45; j += 10)
            {
                GameObject obj = Instantiate(preFabs[2], new Vector3(i, 1.05f, j), Quaternion.identity, this.transform);
                obj.AddComponent<BoxCollider>();
                grassList.Add(obj);
                obj.SetActive(false);
            }
        }

        //for(79-37, -45)
        for (int j = 52; j >= -48; j -= 3)
        {
            //print(j);
            if (j <= 46 && j > -42)
            {
                j -= 88;
                continue;
            }
            for (int i = -79; i <= 37; i += 3)
            {
                int randPrefab = Random.Range(3, 6);
                float offsetPosition = Random.Range(-1.5f, 1.5f);
                Quaternion offsetRotation = Quaternion.Euler(0, Random.Range(0f, 90f), 0);
                GameObject obj = Instantiate(preFabs[randPrefab], new Vector3(i + offsetPosition, 0.5f, j), offsetRotation, this.transform);
            }
        }

        for (int i = -82; i <= 42; i += 3)
        {
            //print(i);
            if (i >= -78 && i < 34)
            {
                i += 112;
                continue;
            }
            for (int j = -45; j <= 45; j += 3)
            {
                if (j < 7 && j > -5)
                    continue;
                int randPrefab = Random.Range(3, 6);
                float offsetPosition = Random.Range(-1.5f, 1.5f);
                Quaternion offsetRotation = Quaternion.Euler(0, Random.Range(0f, 90f), 0);
                GameObject obj = Instantiate(preFabs[randPrefab], new Vector3(i, 0.5f, j + offsetPosition), offsetRotation, this.transform);
            }
        }
    }

  

    public void ForestActive(float t)
    {

    }

    private void ActiveAll(Spline spline, int knotIndex, SplineModification modificationType)
    {
        foreach (GameObject obj in fieldsList)
        {
            obj.SetActive(false);
        }

        float amount = fieldsList.Count * River.Instance.T;
        for (int i = (int)amount; i < fieldsList.Count - (int)amount; i++)
        {
            fieldsList[i].SetActive(true);
        }


        foreach (GameObject obj in grassList)
        {
            obj.SetActive(true);
        }
    }
    void OnEnable()
    {
        Spline.Changed += ActiveAll;
    }

    void OnDisable()
    {
        Spline.Changed -= ActiveAll;
    }
}
