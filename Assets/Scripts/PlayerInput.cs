using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInput : MonoBehaviour
{//Get all user inputs
    private PlayerControls controls;
    public bool isGamepad = false;
    private bool ps4_x = false;
    public Vector2 move;
    public bool pausing;

    private void Awake()
    {
        controls = new PlayerControls();
    }
    private void Start()
    {

        controls.Gameplay.Move.performed += ctx => JoyStickMove(ctx);
        controls.Gameplay.Move.canceled += ctx => JoyStickStop();
        controls.Gameplay.PS4_X.performed += ctx => PS4_X_Press(ctx);
        controls.Gameplay.PS4_X.canceled += ctx => PS4_X_UnPress();


    }
    // Update is called once per frame
    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float horizontal = Input.GetAxis("Vertical");
        float vertical = Input.GetAxis("Horizontal");
        bool pause = Input.GetKeyDown(KeyCode.Escape);
        if (!isGamepad && vertical == 0 && horizontal == 0)
        {
            move = new Vector2(mouseX, mouseY);
            pause = Input.GetKeyDown(KeyCode.Escape);

        }
        else if (!isGamepad && mouseX == 0 && mouseY == 0)
        {
            move = new Vector2(vertical, horizontal);

        }
        pausing = pause || ps4_x;

    }

    void JoyStickMove(InputAction.CallbackContext ctx)
    {
        isGamepad = true;
        move = ctx.ReadValue<Vector2>();
    }

    void JoyStickStop()
    {
        isGamepad = false;
        move = Vector2.zero;
    }

    void PS4_X_Press(InputAction.CallbackContext ctx)
    {
        isGamepad = true;
        ps4_x = ctx.ReadValueAsButton();
    }


    void PS4_X_UnPress()
    {
        isGamepad = false;
        ps4_x = false;
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

