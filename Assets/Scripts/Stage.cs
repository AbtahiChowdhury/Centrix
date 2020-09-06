using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject loopPrefab;
    public static Stage instance;
    private Vector3 scale = new Vector3(.02f, .02f, 0f);
    private Vector3 originalSize;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }


    public IEnumerator BeatAnimation()
    {
        //OuterCircleSpawn
        loopPrefab.GetComponent<SpriteRenderer>().color = Random.ColorHSV(0f, 1f, 1f, 1f, .75f, 1f);
        Instantiate(loopPrefab);

        //Scaling animation
        transform.localScale += scale;
        yield return new WaitForSeconds(0.01f);
        transform.localScale -= scale;
    }


    // Start is called before the first frame update
    void Start()
    {
        originalSize = transform.localScale;
        // StartCoroutine(OuterCircleSpawner());
    }

    /*
         IEnumerator OuterCircleSpawner()
         {
             while(true)
             {
                 Instantiate(loopPrefab);
                 yield return new WaitForSeconds(0.25f);
             }
         }*/
}
