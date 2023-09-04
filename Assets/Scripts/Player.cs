using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    private bool buttonOne = false;
    private bool buttonTwo = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnButtonOne(InputAction.CallbackContext context)
    {
        buttonOne = context.action.triggered;
    }

    public void OnButtonTwo(InputAction.CallbackContext context)
    {
        buttonTwo = context.action.triggered;
    }
}
