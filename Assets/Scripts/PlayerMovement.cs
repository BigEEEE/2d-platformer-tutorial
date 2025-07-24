using Unity.VisualScripting;

using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    public Rigidbody2D rb;
    public Sprite[] charSprites = new Sprite[2];

    private SpriteRenderer spriterenderer;

    [Header("Movement")]
    public float moveSpeed;

    [Header("Jumping")]
    public int jumpLimit;
    public float jumpStrength;
    
    [Header("Wall Movement")]
    public float wallSlideSpeed;
    public float wallJumpTime;
    public Vector2 wallJumpPower = new Vector2 (5f, 10f);

    [Header("Ground Check")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize;
    public LayerMask groundCheckLayerMask;

    [Header("Wall Check")]
    public Transform wallCheckPos;
    public Vector2 wallCheckSize;
    public LayerMask wallCheckLayerMask;

    [Header("Gravity")]
    public float baseGravity;
    public float maxFallSpeed;
    public float fallSpeedMultiplier;

    private int jumpsRemaining;

    private float horizontalMovement;
    private float wallJumpDirection;
    private float wallJumpTimer;
    
    private bool isFacingRight = true;
    private bool isGrounded;
    private bool isWallSliding;
    private bool isWallJumping;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriterenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        Debug.Log(isWallSliding);

        GroundCheck();
        Gravity();
        WallSlide();
        WallJumpProcess();

        if (!isWallJumping)
        {
            Flip();
            rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
        }
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;

        if (horizontalMovement != 0)
        {
            spriterenderer.GetComponent<SpriteRenderer>().sprite = charSprites[1];
        }
        else
        {
            spriterenderer.GetComponent<SpriteRenderer>().sprite = charSprites[0];
        }
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (jumpsRemaining > 0) {

            if (context.performed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpStrength);
                jumpsRemaining--;
            }

        }

        //Wall jump

        if (context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0;

            if(transform.localScale.x != wallJumpDirection)
            {
                isFacingRight = !isFacingRight;
                Vector3 ls = transform.localScale;
                ls.x *= -1f;
                transform.localScale = ls;
            }

            Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f);
        }

    }



    private void Gravity() 
    {

        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, - maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundCheckLayerMask))
        {
            jumpsRemaining = jumpLimit;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

    }
    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallCheckLayerMask);
   
    }
    private void WallSlide()
    {
        if (!isGrounded & WallCheck() & horizontalMovement != 0)
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false;        
        }
    }

    private void WallJumpProcess()
    {
        if (isWallSliding)
        {
            isWallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));

        }
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    private void Flip()
    {
        if (isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;

        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if (item != null)
        {
            item.Collect();
        }
    }
}
