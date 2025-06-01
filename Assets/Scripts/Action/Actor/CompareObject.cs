using System.Collections.Generic;
using UnityEngine;

public class CompareObject : MonoBehaviour
{
    [SerializeField] private int hasObject ;
    private void Start()
    {
        hasObject = 3;
    }
    private void Update()
    {
        if (hasObject <= 0)
        {
            Notice();
        }
    }
    public void ObjectFit(IEnumerable<Trigger> objects)
    {
        hasObject--;
    }
    public void ObjectNotFit(IEnumerable<Trigger> objects)
    {
        hasObject++;
    }
    public void Notice()
    {
        Debug.Log("Complete!");
    }
}
