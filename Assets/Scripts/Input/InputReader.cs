using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static Controls;


[CreateAssetMenu(menuName = "Input/InputReader", fileName = "New Input Reader")]
public class InputReader : ScriptableObject, IPlayerActions
{
    //Events
    public event Action<bool> PrimaryFiredEvent;
    public event Action<Vector2> MoveEvent;
    
    //Varible
    //Why this is not event ? => if we are making this a event, it will trigger very often so it is not performant.
    //In the keyboard movement, even if we are holding W button for a time , it will trigger the event once in that specific situtaion.
    
    public Vector2 MousePosition { get; private set; }    
    
    
    private Controls _controls;
    private void OnEnable()
    {
        
        if (_controls == null)
        {
            _controls = new Controls();
            _controls.Player.SetCallbacks(this);
        }
        _controls.Player.Enable();
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Vector2 movementVector = context.ReadValue<Vector2>();
        MoveEvent?.Invoke(movementVector);
    }

    public void OnPrimaryFire(InputAction.CallbackContext context)
    {

        if (context.performed)
        {
            PrimaryFiredEvent?.Invoke(true);
        } 
        else if (context.canceled)
        {
            PrimaryFiredEvent?.Invoke(false);
        }
    }

    public void OnAim(InputAction.CallbackContext context)
    {
        MousePosition = context.ReadValue<Vector2>();
    }
}
