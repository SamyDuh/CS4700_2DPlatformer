using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour { 

    public Vector2 velocity;
    public float gravityFactor;
    public float desiredx;
    public bool grounded;

    Animator animator;
    Rigidbody2D rb;
    SpriteRenderer sr;

    // Start is called before the first frame update
    void Start()
    {
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

    }

    public void Movement(Vector2 move, bool horizontal)
    {
        if (move.magnitude < .00001f) return;
        grounded = false;
        RaycastHit2D[] results = new RaycastHit2D[16];
        int cnt = GetComponent<Rigidbody2D>().Cast(move.normalized, results, move.magnitude + .001f);

        for (int i = 0; i < cnt; i++)
        {
            if (Mathf.Abs(results[i].normal.x) > .3f && horizontal)
            {
                return;
            }
            if (Mathf.Abs(results[i].normal.y) > .3f && !horizontal)
            {
                if (results[i].normal.y > .3f)
                {
                    grounded = true;
                    animator.SetTrigger("Grounded");
                    velocity.y = 0;
                }
                return;
            }
        }
        transform.position += (Vector3)move;
    }


        // Update is called once per frame
     void FixedUpdate() {

     if (Input.GetAxis("Horizontal") > 0) desiredx = 5;
     else if (Input.GetAxis("Horizontal") < 0) desiredx = -5;
     else desiredx = 0;

     
     if (desiredx < 0) sr.flipX = true;
     else if (desiredx > 0) sr.flipX = false; 

     Vector2 acceleration = 9.81f * Vector2.down * gravityFactor;
     velocity += acceleration * Time.fixedDeltaTime;
     velocity.x = desiredx;
     Vector2 move = velocity * Time.fixedDeltaTime;

     animator.SetFloat("Velocity", Mathf.Abs(move.x));
     animator.SetFloat("Velocity.y", Mathf.Abs(move.y));


        if (Input.GetButton("Jump") && grounded)
        {
            velocity.y = 6.5f;
            animator.SetTrigger("Jump");
            //animator.SetBool("Jumping", false);
        }

     Movement(new Vector2(move.x, 0), true);
     Movement(new Vector2(0, move.y), false);

     }


}
