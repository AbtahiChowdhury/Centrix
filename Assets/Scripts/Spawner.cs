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
        Vector3 position = new Vector3(4.85f * Mathf.Cos(theta), 4.85f * Mathf.Sin(theta));

        Vector3 direction = new Vector3(-4.85f * Mathf.Cos(theta), -4.85f * Mathf.Sin(theta));
        direction.Normalize();
        direction = Quaternion.Euler(0, 0, deltaTheta) * direction;

        GameObject bullet = Instantiate(GameManager.Instance.bulletPrefab);
        bullet.transform.position = position;
        bullet.GetComponent<BulletScroller>().direction = direction;
    }
}
