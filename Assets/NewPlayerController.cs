using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;


public class NewPlayerController : MonoBehaviour
{

    public GameObject swordProjectile;

    public Vector2 velocity;
    public bool grounded;

    public float dashMultiplier;

    public float bounceMultiplier;

    public float gravityJumpMultiplier;

    public float gravityMultiplier;

    public float movementMultiplier;

    public float jumpResistance;

    public float jumpMultiplier;

    public bool wasGrounded;

    public bool isJumping;

    public bool isFalling;

    public bool hasSword = true;

    public bool isDashing;

    public bool holdingJump;

    public bool hasThrownSword;

    public bool hasDashed;

    public int facingDirection = 1;

    public bool groundLock = false;

    private GameObject swordOutThere;

    Animator animator;
    Rigidbody2D rb;
    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        grounded = false;
        hasSword = true;
    }

    public void grabSword()
    {
        hasSword = true;
    }

    void Gravity()
    {

        // uses a capsule cast to check if there is a collision directly below the player
        grounded = Physics2D.CapsuleCast(transform.position, new Vector2(1f,2.1f), CapsuleDirection2D.Vertical, 0f, Vector2.down, 0.05f);


        if ((grounded) && (!wasGrounded))
        {
            animator.SetTrigger("HitGround");
            isJumping = false;
            isFalling = false;

            hasThrownSword = false;
            hasDashed = false;


        }
        
    
    }
    
    void Movement()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        if (grounded)
        {

            rb.velocity = new Vector2(horizontalInput * movementMultiplier, rb.velocity.y);
        }
        else
        {
            //if (facingDirection == 1 && horizontalInput > 0) horizontalInput *= .3f;
            //else if (facingDirection == -1 && horizontalInput < 0) horizontalInput *= .3f;
            rb.velocity += new Vector2(horizontalInput * movementMultiplier / jumpResistance, 0);
        }

        if (horizontalInput > 0)
            facingDirection = 1;
        else if (horizontalInput < 0)
            facingDirection = -1;

        sr.flipX = facingDirection == -1;

        if (Input.GetButton("Jump") && (grounded == true) && (isJumping == false))
        {
            isJumping = true;
            animator.SetTrigger("Jump");
            rb.velocity = new Vector2(rb.velocity.x, jumpMultiplier);
        }
    }

   private async void Dash(Vector2 direction)
    {
        hasDashed = true;
        if (isDashing) return;

        rb.gravityScale = 0.0f;

        // reduces vertical intensity on the dash
        direction.y *= 0.0f;

        isDashing = true;
        animator.SetTrigger("Dash");
        animator.SetBool("Dashing", true);

        rb.velocity = direction.normalized * dashMultiplier;
        
        facingDirection = direction.x >= 0 ? 1 : -1;
        sr.flipX = facingDirection == -1;

        await Task.Delay(175);

        rb.velocity = new Vector2(rb.velocity.x * 0.5f, rb.velocity.y); 

        animator.SetTrigger("DashOver");
        animator.SetBool("Dashing", false);
        isDashing = false;

        rb.gravityScale = 4.0f;
    }
    void CheckThrow()
    {

        if (Input.GetMouseButtonDown(1) && (!isDashing) && (!hasDashed)) 
        {

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f; 

            Vector2 direction = (mousePosition - transform.position).normalized;
            Dash(direction);
            

            return;
        }

        if (Input.GetMouseButtonDown(0) && (!hasThrownSword))
        {
            hasThrownSword = true;
            if (hasSword)
            {
                hasSword = false;
                Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Vector2 direction = mousePosition - transform.position;
                print(mousePosition);

                swordOutThere = Instantiate(swordProjectile, transform.position, Quaternion.identity);
                SwordProjectile sword = swordOutThere.GetComponent<SwordProjectile>();
                sword.setAngle(direction);
            }
            else
            {
               
                SwordProjectile sword = swordOutThere.GetComponent<SwordProjectile>();
                sword.returnToPlayer();
            }
        }


    }
    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isDashing)
        {
            //Gravity();
            grounded = Physics2D.CapsuleCast(transform.position, new Vector2(1f, 2.1f), CapsuleDirection2D.Vertical, 0f, Vector2.down, 0.03f);


            if ((grounded) && (!wasGrounded))
            {
                
                //animator.SetTrigger("HitGround");
                isJumping = false;
                isFalling = false;

                hasThrownSword = false;
                hasDashed = false;


            }
            Movement();
        }

        if (rb.velocity.y > 0 && isJumping && !holdingJump)
        {
            //float gravity = Vector2.down * gravityJumpMultiplier * Time.fixedDeltaTime;
            //rb.velocity += Vector2.down * gravityJumpMultiplier * Time.fixedDeltaTime;
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * .5f);
        }

        if (grounded && !wasGrounded)
        {
            animator.SetBool("Grounded", true);
        }

        if (!grounded && wasGrounded)
        {
            animator.SetBool("Grounded", false);
        }

        wasGrounded = grounded;
        //animator.SetFloat("Velocity", Mathf.Abs(rb.velocity.x));


    }

    public void SwordBounce()
    {
        animator.SetTrigger("Bounce");
        isJumping = false;
        rb.velocity = new Vector2(rb.velocity.x, bounceMultiplier);
    }

    private void Update()
    {


        if (grounded) hasThrownSword = false;

        holdingJump = Input.GetButton("Jump");

        animator.SetFloat("Velocity", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("Fall Velocity", rb.velocity.y);
        CheckThrow();

        if ((Mathf.Abs(rb.velocity.y) < .5) && (!isFalling) && (!grounded))
        {
            
            isFalling = true;
            animator.SetTrigger("Falling");
        }


       

       
    }
}
