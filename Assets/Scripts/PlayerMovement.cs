using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{

    private PlayerInput playerInput;
    public Rigidbody2D rb;
    public bool paused = false;
    public float mouseSensitivity = 50f;
    public float speed = 1.5f;

    private void Awake()
    {
        //Initialize fields
        playerInput = GetComponent<PlayerInput>();
    }

    private void Move()
    {
        Vector2 moveDirectionX = transform.right * playerInput.move.x;
        Vector2 moveDirectionY = transform.up * playerInput.move.y;
        Vector2 moveDirection = moveDirectionX + moveDirectionY;

        float h = Mathf.Sqrt(Mathf.Pow(transform.position.x + moveDirection.x, 2) + Mathf.Pow(transform.position.y + moveDirection.y, 2));
        if (h  < 4.85f)
        {
            gameObject.transform.position +=  new Vector3(moveDirection.x,moveDirection.y,0) * Time.unscaledDeltaTime * speed;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        if (!paused)
        {
            Move();

            float theta = Mathf.Atan2(transform.position.y, transform.position.x);
            float radius = Mathf.Clamp(transform.position.magnitude, 0, 3.85f);
            transform.position = new Vector3(
                radius * Mathf.Cos(theta),
                radius * Mathf.Sin(theta),
                0
            );
        }
    }
}
