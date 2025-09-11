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
    public float gravityMultiplier;
    public float movementMultiplier;
    public float jumpResistance;
    public float jumpMultiplier;

    public bool isJumping;

    public bool isFalling;

    public bool hasSword;

    public bool isDashing;

    public int facingDirection = 1;

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

        if ((grounded == true) && (isFalling == true))
        {
            animator.SetTrigger("HitGround");
            isJumping = false;
            isFalling = false;
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
        if (isDashing) return;

        isDashing = true;
        animator.SetTrigger("Dash");

        rb.velocity = direction.normalized * dashMultiplier;
        
        facingDirection = direction.x >= 0 ? 1 : -1;
        sr.flipX = facingDirection == -1;

        await Task.Delay(50);

        rb.velocity = new Vector2(rb.velocity.x * 0.5f, rb.velocity.y); 

        animator.SetTrigger("DashOver");
        isDashing = false;
    }
    void CheckThrow()
    {

        if (Input.GetMouseButtonDown(1) && (!isDashing))
        {

            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f; 

            Vector2 direction = (mousePosition - transform.position).normalized;
            Dash(direction);
            

            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
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
            Gravity();
            Movement();
        }
        

        

        //animator.SetFloat("Velocity", Mathf.Abs(rb.velocity.x));

       
    }

    private void Update()
    {
        animator.SetFloat("Velocity", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("Fall Velocity", rb.velocity.y);
        CheckThrow();

        if ((Mathf.Abs(rb.velocity.y) > .5) && (!isFalling) && (!grounded))
        {
            isFalling = true;
            animator.SetTrigger("Falling");
        }
    }
}
