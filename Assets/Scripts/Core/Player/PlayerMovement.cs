using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerMovement : NetworkBehaviour
{
    
    [Header("References")]
    [SerializeField] private InputReader _inputReader;
    [SerializeField] private Transform _tankBody;
    [SerializeField] private Rigidbody2D rb;

    [Header("Settings")] 
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float rotationSpeed = 200;
    
    
    private Vector2 currentLocation;


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        _inputReader.MoveEvent += HandleMove;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsOwner) return;
        _inputReader.MoveEvent -= HandleMove;
    }

    private void HandleMove(Vector2 movementVector)
    {
        currentLocation = movementVector;
    }
    

    void Update()
    {
        if(!IsOwner) {return;}
        
        float zRotation = currentLocation.x * (-rotationSpeed) * Time.deltaTime;
        _tankBody.Rotate(0f,0f,zRotation); 
    }

    //Fixed Update method is useful for including physics operation.
    //Bc it is in sync with the physics engine.
    //In here, we are dealing with the rigidbody, so do the arithmetic here instead of normal Update
    private void FixedUpdate()
    {
        if (!IsOwner) return;
        
        //Why not using Vector2.up ? => because we are moving according to tank body.Rotation changes it pivot.
        //Why not using Time.deltaTime ? => because it is already in the FixedUpdate
      rb.velocity = (Vector2)_tankBody.up * (moveSpeed * currentLocation.y);


    }
}
