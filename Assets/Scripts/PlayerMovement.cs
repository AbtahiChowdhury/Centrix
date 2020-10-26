using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{

    private PlayerInput playerInput;
    public Rigidbody2D rb;
    public float mouseSensitivity = 100f;
    public float speed = 10f;
    private Vector3 move1;

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

        float radius = Mathf.Clamp(moveDirection.magnitude, 0, 3.85f);
        float h = Mathf.Sqrt(Mathf.Pow(transform.position.x + moveDirection.x, 2) + Mathf.Pow(transform.position.y + moveDirection.y, 2));
        if (h  < 3.85f)
        {
            gameObject.transform.position += new Vector3(moveDirection.x,moveDirection.y,0);
        }    
    }
    
    // Update is called once per frame
    void Update()
    { 
        Move();
        float theta = Mathf.Atan2(transform.position.y, transform.position.x);
        float radius = Mathf.Clamp(transform.position.magnitude, 0, 3.85f);
      
    }
}
