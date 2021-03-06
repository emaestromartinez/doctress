﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{

    public PlayerMovementInput movementInput;
    private BoxCollider2D boxCollider;

    public float movementSpeed = 1f;
    public float jumpSpeed = 1f;

    Rigidbody2D rbody;

    Vector2 movementVector;
    public bool jumpPending;
    public bool isGrounded;
    private float coyoteTime;

    private bool areInputsEnabled = false;
    public bool isTouchingWall;
    private Transform wallCheck;
    private bool wallSliding;
    private float wallSlidingSpeed;

    Camera mainCamera;


    private void OnCollisionEnter2D(Collision2D other)
    { // Colisionamos con algo.
        if (other.collider.tag == "Ground")
        { // Ese algo es el suelo, estamos en el suelo (o en una pared omg!)
            isGrounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D other)
    { // Deja de colisionar con algo.
        if (other.collider.tag == "Ground")
        { // Ese algo es el suelo, quiere decir que estamos volando;
            isGrounded = false;

        }
    }


    void Awake()
    {
        movementInput = new PlayerMovementInput();

        rbody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        mainCamera = Camera.main;

    }

    void OnEnable()
    {
        movementInput.Enable();
        areInputsEnabled = true;
    }

    void OnDisable()
    {
        areInputsEnabled = false;
        movementInput.Disable();
    }
    void disableInputs()
    {
        areInputsEnabled = false;
        // movementInput.Disable();
    }
    void enableInputs()
    {
        areInputsEnabled = true;
        // movementInput.Enable();
    }

    private void Start()
    {
        movementInput.Keyboard.Movement.performed += (context) =>
        {
            movementVector = context.ReadValue<Vector2>();
        };
        movementInput.Keyboard.Movement.canceled += (_) =>
        {
            movementVector = Vector2.zero;
        };
        movementInput.Keyboard.Jump.performed += (_) =>
        {
            if (isGrounded || isTouchingWall || coyoteTime > 0) jumpPending = true;

        };
    }

    private void Update()
    {
        coyoteTime -= Time.deltaTime;
    }
    void FixedUpdate()
    {
        isTouchingWall = Physics2D.Raycast((new Vector2(rbody.position.x - 0.375f, rbody.position.y)), Vector2.right, 0.75f, 1 << LayerMask.NameToLayer("Ground"));
        isGrounded = Physics2D.Raycast((new Vector2(rbody.position.x, rbody.position.y - 0.03f)), Vector2.down, 0.6f, 1 << LayerMask.NameToLayer("Ground"));
        if (isGrounded) coyoteTime = 0.15f;

        if (areInputsEnabled)
        {
            if (jumpPending && isGrounded)
            {
                rbody.velocity = new Vector2(rbody.velocity.x, 1 * jumpSpeed);

                jumpPending = false;
            }
            else if (jumpPending && coyoteTime > 0f)
            {
                rbody.velocity = new Vector2(rbody.velocity.x, 1 * jumpSpeed);
                Debug.Log("COYOTE JUMP");
                jumpPending = false;
            }
            else if (jumpPending && isTouchingWall)
            {
                if (movementVector.x == 1.0f)
                {
                    // Apply force to the left;
                    rbody.AddForce(new Vector2(-1.2f * jumpSpeed, 1 * jumpSpeed), ForceMode2D.Impulse);
                    if (areInputsEnabled)
                    {
                        Invoke("disableInputs", 0f);
                        Invoke("enableInputs", 0.2f);
                    }
                }
                if (movementVector.x == -1.0f)
                {
                    // Apply force to the right;
                    rbody.AddForce(new Vector2(1.2f * jumpSpeed, 1 * jumpSpeed), ForceMode2D.Impulse);
                    if (areInputsEnabled)
                    {
                        Invoke("disableInputs", 0f);
                        Invoke("enableInputs", 0.2f);
                    }
                }
                // Disable movement input for 0.5 seconds and re-enable it.
                jumpPending = false;
            }
            else rbody.velocity = new Vector2(movementVector.x * movementSpeed, rbody.velocity.y);

        }

    }
}

