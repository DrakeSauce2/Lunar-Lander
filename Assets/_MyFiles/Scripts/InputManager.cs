using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    IA_Player playerInput;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);

        playerInput = new IA_Player();
        playerInput.Enable();

        playerInput.Main.StartButton.performed += StartGame;
        playerInput.Main.Quit.performed += QuitGame;
    }

    private void QuitGame(InputAction.CallbackContext context)
    {
        Debug.Log("Quitting Game!");
        Application.Quit();
    }

    private void StartGame(InputAction.CallbackContext context)
    {
        GameManager.Instance.StartGame();
    }

    public float GetRollInput()
    {
        if (GameManager.Instance.GetHasLanded())
        {
            return 0;
        }

        return playerInput.Main.Roll.ReadValue<float>(); 
    }

    public float GetThrustInput()
    {
        if (GameManager.Instance.GetHasLanded())
        {
            return 0;
        }

        return playerInput.Main.Thrust.ReadValue<float>();
    }

}
