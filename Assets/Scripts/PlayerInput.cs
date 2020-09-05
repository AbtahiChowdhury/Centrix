using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInput : MonoBehaviour
{//Get all user inputs
    private PlayerControls controls;
    private bool isMouseControl = true;
    public Vector2 move;

    private void Awake()
    {
        controls = new PlayerControls();
    }
    private void Start()
    {

        controls.Gameplay.Move.performed += ctx => JoyStickMove(ctx);
        controls.Gameplay.Move.canceled += ctx => JoyStickStop();
    

    }
    // Update is called once per frame
    void Update()
    {
        if (isMouseControl)
        {
            move = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }
    }

    void JoyStickMove(InputAction.CallbackContext ctx)
    {
        isMouseControl = false;
        move = ctx.ReadValue<Vector2>();
    }

    void JoyStickStop()
    {
        isMouseControl = true;
        move = Vector2.zero;
    }

    void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
    }
}

