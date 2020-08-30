using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopScroller : MonoBehaviour
{
    float time;
    Vector3 startScale;
    Vector3 endScale;

    // Start is called before the first frame update
    void Start()
    {
        time = 0;
        startScale = new Vector3(0.75f, 0.75f, 0f);
        endScale = new Vector3(2.01f, 2.01f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        time += 0.25f * GameManager.Instance.speed * Time.deltaTime;
        transform.localScale = Vector3.Lerp(startScale, endScale, time);

        if(transform.localScale.x > 2f)
        {
            Destroy(this.gameObject);
        }
    }
}
