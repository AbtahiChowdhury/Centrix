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

    //Attempt to dynamically sync Bullet Spawning to Beat Detection for unique audio visualization.
    public IEnumerator BeatSpawner() // This method is called by AudioSyncer on every beat
    {
        //Random color for a randomly selected spawner
        Color color = Random.ColorHSV(0f, 1f, 1f, 1f, .75f, 1f);        
        System.Random random = new System.Random();
        GameObject[] spawners = GameManager.getSpawners();
        int index = random.Next(0, spawners.Length);
        spawners[index].GetComponentInChildren<SpriteRenderer>().color = color;
        spawners[index].transform.Find("Triangle").GetComponent<SpriteRenderer>().color = color;

        //Trying to spawn multiple bullets for the selected spawner circle
        for(int i= -20; i<20; i+=10)
        {
            spawners[index].GetComponent<Spawner>().SpawnBullet(i);
        }

        yield return new WaitForSeconds(0.5f);
        //Reset Color
        spawners[index].GetComponentInChildren<SpriteRenderer>().color = Color.black;
        spawners[index].transform.Find("Triangle").GetComponent<SpriteRenderer>().color = Color.black;
    }

    public IEnumerator BeatAnimation() //// This method is called by AudioSyncer on every beat
    {
        Color color = Random.ColorHSV(0f, 1f, 1f, 1f, .75f, 1f);
       
        //OuterCircleSpawn
        loopPrefab.GetComponent<SpriteRenderer>().color = color;
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
