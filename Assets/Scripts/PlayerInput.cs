using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerInput : MonoBehaviour
{//Get all user inputs
    private PlayerControls controls;
    public bool isGamepad = false;
    public Vector2 move;
    public bool pausing;
    public bool exit;
    public bool bombClear;

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
        bool ps4_x = controls.Gameplay.PS4_X.triggered;
        bool ps4_options = controls.Gameplay.PS4_OPTIONS.triggered;
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");
        float horizontal = Input.GetAxis("Vertical");
        float vertical = Input.GetAxis("Horizontal");
        if (!isGamepad && vertical == 0 && horizontal == 0)
        {
            move = new Vector2(mouseX, mouseY);
        }
        else if (!isGamepad && mouseX == 0 && mouseY == 0)
        {
            move = new Vector2(vertical, horizontal);
        }
        pausing = Input.GetKeyDown(KeyCode.Escape) || ps4_options;
        bombClear = Input.GetKeyDown(KeyCode.Z) || ps4_x;

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

   
    void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    void OnDisable()
    {
        controls.Gameplay.Disable();
    }
}

