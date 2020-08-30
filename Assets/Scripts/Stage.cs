using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject loopPrefab;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(OuterCircleSpawner());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator OuterCircleSpawner()
    {
        while(true)
        {
            Instantiate(loopPrefab);
            yield return new WaitForSeconds(0.25f);
        }
    }
}
