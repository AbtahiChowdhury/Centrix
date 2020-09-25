using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public float theta;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, Time.deltaTime * GameManager.Instance.spawnerRotationSpeed);
        theta = transform.localEulerAngles.z * (Mathf.PI / 180f);
    }

    public void SpawnBullet(float deltaTheta)
    {
        //Spawn bullet
        Vector3 position = new Vector3(4.85f * Mathf.Cos(theta), 4.85f * Mathf.Sin(theta));

        Vector3 direction = new Vector3(-4.85f * Mathf.Cos(theta), -4.85f * Mathf.Sin(theta));
        direction.Normalize();
        direction = Quaternion.Euler(0, 0, deltaTheta) * direction;

        GameObject bullet = Instantiate(GameManager.Instance.bulletPrefab);
        bullet.transform.position = position;
        bullet.GetComponent<BulletScroller>().direction = direction;

        UpdateColors();
    }

    IEnumerator UpdateColors()
    {
        //Random color for a randomly selected spawner
        Color color = Random.ColorHSV(0f, 1f, 1f, 1f, .75f, 1f);
        System.Random random = new System.Random();
        transform.Find("Circle1").GetComponent<SpriteRenderer>().color = color;
        transform.Find("Triangle").GetComponent<SpriteRenderer>().color = color;

        yield return new WaitForSeconds(0.5f);

        //Reset Color
        transform.Find("Circle1").GetComponent<SpriteRenderer>().color = Color.black;
        transform.Find("Triangle").GetComponent<SpriteRenderer>().color = Color.black;
    }
}
