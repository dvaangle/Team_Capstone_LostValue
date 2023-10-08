using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager instance;

    [HideInInspector]
    public Player_InputSettings player_InputSettings;
    [HideInInspector]
    public Vector2 moveInput;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }

        player_InputSettings = new Player_InputSettings();

        player_InputSettings.Movement.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
    }
    private void OnEnable()
    {
        player_InputSettings.Enable();
    }
    private void OnDisable()
    {
        player_InputSettings.Disable();
    }
}
