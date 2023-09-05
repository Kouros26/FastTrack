using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class Player : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    private bool buttonOne = false;
    private bool buttonTwo = false;
    private bool buttonThree = false;
    private bool buttonFour = false;

    private bool buttonOneHeld = false;
    private bool buttonTwoHeld = false;
    private bool buttonThreeHeld = false;
    private bool buttonFourHeld = false;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("One : " + buttonOne + " Two : " + buttonTwo + " Three : " + buttonThree + " Four : " + buttonFour);
        //Debug.Log("Press : " + buttonOne + '\n' + "Hold : " + buttonOneHeld);
    }

    public void OnButtonOne(InputAction.CallbackContext context)
    {
        buttonOne = context.action.triggered;
    }

    public void OnButtonTwo(InputAction.CallbackContext context)
    {
        buttonTwo = context.action.triggered;
    }

    public void OnButtonThree(InputAction.CallbackContext context)
    {
        buttonThree = context.action.triggered;
    }

    public void OnButtonFour(InputAction.CallbackContext context)
    {
        buttonFour = context.action.triggered;
    }

    public void OnButtonOneHeld(InputAction.CallbackContext context)
    {
        buttonOneHeld = context.action.triggered;
    }

    public void OnButtonTwoHeld(InputAction.CallbackContext context)
    {
        buttonTwoHeld = context.action.triggered;
    }

    public void OnButtonThreeHeld(InputAction.CallbackContext context)
    {
        buttonThreeHeld = context.action.triggered;
    }

    public void OnButtonFourHeld(InputAction.CallbackContext context)
    {
        buttonFourHeld = context.action.triggered;
    }
}
