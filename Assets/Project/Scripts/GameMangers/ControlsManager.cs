using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ControlsManager : MonoBehaviour
{
    public static ControlsManager Instance;

    public PlayerControls controls;
    private PlayerInput playerInput;

    public string CurrentDevice
    {
        get
        {
            string device = playerInput.currentControlScheme;

            if (device == "Gamepad") device = "Xbox";

            return device;
        }
    }

    private void Awake()
    {
        Instance = this;

        playerInput = GetComponent<PlayerInput>();

        controls = new PlayerControls();
        controls.Enable();
    }
}
