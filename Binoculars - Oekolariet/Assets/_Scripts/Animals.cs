using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

public class Animals : MonoBehaviour
{
    private List<GameObject> children = new List<GameObject>();
    [SerializeField]
    private List<Animator> animators = new List<Animator>();
    private void Start()
    {
        for(int i = 0; i < transform.childCount;i++) 
        {
            GameObject child = transform.GetChild(i).gameObject;
            children.Add(child);
            animators.Add(child.GetComponent<Animator>());
            child.SetActive(false);
        }
    }

    private void Update()
    {
        int rand = Random.Range(0, children.Count * 10);
        if (rand >= children.Count)
            return;

        if (children[rand].activeSelf == false)
            return;

        if (animators[rand].IsInTransition(0))
            return;

        if(animators[rand].GetBool("Eat_b") == true)
            animators[rand].SetBool("Eat_b", false);
        else
            animators[rand].SetBool("Eat_b", true);

        //print(rand);
        
    }


    private void ActiveAnimals(Spline spline, int knotIndex, SplineModification modificationType)
    {
        foreach (GameObject obj in children)
        {
            obj.SetActive(false);
        }

        float amount = children.Count * River.Instance.T;
        for (int i = 0; i < (int)amount; i++)
        {
            children[i].SetActive(true);
        }
    }

    void OnEnable()
    {
        Spline.Changed += ActiveAnimals;
    }

    void OnDisable()
    {
        Spline.Changed -= ActiveAnimals;
    }
}
