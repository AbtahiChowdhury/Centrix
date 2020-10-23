using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{

    private PlayerInput playerInput;
    public Rigidbody2D rb;
    public float mouseSensitivity = 100f;
    public float speed = 10f;

    private void Awake()
    {
        //Initialize fields
        playerInput = GetComponent<PlayerInput>();
    }

    private void Move()
    {//
        Vector2 moveDirectionX = transform.right * playerInput.move.x;
        Vector2 moveDirectionY = transform.up * playerInput.move.y;
        Vector2 moveDirection = moveDirectionX + moveDirectionY;
        
        rb.velocity = (moveDirection * speed);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (playerInput.pausing)
        {
            Debug.Log("Pausing");
        }
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
