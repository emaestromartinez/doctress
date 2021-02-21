using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterJump : MonoBehaviour
{
    public PlayerMovementInput movementInput;

    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public bool holdingJump;

    Rigidbody2D rb;
    private void Awake()
    {
        movementInput = new PlayerMovementInput();
        rb = GetComponent<Rigidbody2D>();
    }

    void OnEnable()
    {
        movementInput.Enable();
    }

    void OnDisable()
    {
        movementInput.Disable();
    }

    private void Start()
    {
        movementInput.Keyboard.Jump.performed += (_) =>
        {
            holdingJump = true;
        };
        movementInput.Keyboard.Jump.canceled += (_) =>
        {
            holdingJump = false;
        };
    }

    private void Update()
    {
        if (rb.velocity.y < 0)
        { // We are falling;
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !holdingJump)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }
}
