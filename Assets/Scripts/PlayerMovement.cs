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
    public float jumpStrength;
    public int jumpLimit;
    


    [Header("Ground Check")]
    public Transform groundCheckPos;
    public Vector2 groundCheckSize;
    public LayerMask groundCheckLayerMask;

    [Header("Gravity")]
    public float baseGravity;
    public float maxFallSpeed;
    public float fallSpeedMultiplier;

    private float horizontalMovement;
    private int jumpsRemaining;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriterenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        GroundCheck();
        
      
        Debug.Log(jumpsRemaining);
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        Gravity();
        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
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

    }

    private void GroundCheck()
    {
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0 , groundCheckLayerMask))
        {
            jumpsRemaining = jumpLimit;
        }
       
    }

    private void Gravity() {

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

    /*private void Flip()
    {
        if (horizontalMovement < 0 )
        {

        }
    */
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);
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
