using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScroller : MonoBehaviour
{
    public Vector3 direction { get; set; }
    private bool hit;

    // Start is called before the first frame update
    void Start()
    {
        hit = false;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (GameManager.Instance.bulletSpeed * Time.deltaTime) * direction;
        transform.Find("Square").transform.Rotate(0, 0, 100f * GameManager.Instance.bulletSpeed * Time.deltaTime);

        if (transform.position.sqrMagnitude > 25f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!hit)
            {
                transform.Find("Circle2").GetComponent<SpriteRenderer>().color = Color.red;
                GameManager.Instance.bulletsHit++;
                hit = true;
            }
        }
        else if (collision.tag == "Bomb")
        {
            Destroy(this.gameObject);
        }
    }
}
