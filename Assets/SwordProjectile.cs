using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Threading.Tasks;

public class SwordProjectile : MonoBehaviour
{

    public float tipOffset = 1.4f;

    public float speed = 20f;

    public bool isMoving;
    public bool isStuck;
    public bool isReturning;

    private Vector2 direction;

    private Collider2D swordCollider;
    private Collider2D playerCollider;
    public Transform playerTransform; 

    private Animator animator;

    private GameObject player;
    private NewPlayerController playerScript;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 tip = transform.position + transform.right * tipOffset;
        Gizmos.DrawSphere(tip, .3f);

        // Optional: Draw cast direction line
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(tip, tip + (Vector3)(direction * 0.1f));
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<NewPlayerController>();

        animator = GetComponent<Animator>();
        swordCollider = GetComponent<Collider2D>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        

        Physics2D.IgnoreCollision(swordCollider, playerCollider, true);

        isMoving = true;
        isStuck = false;
        isReturning = false;

        //flyingTimer();


    }

    public void setAngle(Vector2 angle)
    {
        direction = angle.normalized;
        transform.right = direction;
    }


    public void checkStuck()
    {
        Vector2 pointPosition = transform.position + (Vector3)(direction * tipOffset);
        // is supposed to check for horizontal collisions to the right but it doesnt really work lol
        isStuck = Physics2D.BoxCast(pointPosition, new Vector2(.3f, .2f), 0f, direction, .1f);
        if (isStuck) stickToWall();
    }

    public void stickToWall()
    {
        isMoving = false;
        if (direction.x  < 0f) transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        else transform.rotation = Quaternion.identity;

        animator.SetTrigger("Bounce");
        Physics2D.IgnoreCollision(swordCollider, playerCollider, false);
    }

    public void returnToPlayer()
    {
        if (isMoving) return;
        isReturning = true;
        animator.SetTrigger("Spin");
        Physics2D.IgnoreCollision(swordCollider, playerCollider, true);

    }

    private async void flyingTimer()
    {
        await Task.Delay(300);
        if (!isStuck)
        {
            isMoving = false;
            returnToPlayer();
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isStuck)
        {
            //checkStuck();
            transform.position += (Vector3)(direction * speed * Time.fixedDeltaTime);
        }

        if (isReturning)
        {
            Vector2 directionToPlayer = (playerTransform.transform.position - transform.position).normalized;
            transform.position += (Vector3)(directionToPlayer * speed * Time.fixedDeltaTime);

            if (Vector2.Distance(transform.position, playerTransform.position) < 0.5f)
            {
                isReturning = false;
                playerScript.grabSword();
                Destroy(gameObject);
            }
        }
    }

    private void Update()
    {
        if (!isStuck)
        {
            checkStuck();
        }
    }
}
