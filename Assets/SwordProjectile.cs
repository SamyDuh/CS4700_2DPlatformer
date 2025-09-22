using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Rendering;

public class SwordProjectile : MonoBehaviour
{

    public float tipOffset = 1.4f;

    public float speed = 30f;

    public bool isMoving;

    public bool isStuck;

    public bool isReturning;

    public bool bounceCooldown;

    public bool plinkCooldown;

    private bool isCoolingDown = false;


    private Vector2 direction;

    private Collider2D swordCollider;
    private Collider2D playerCollider;
    public Transform playerTransform; 

    private Animator animator;

    private GameObject player;

    private NewPlayerController playerScript;

    public AudioClip swordPlink;
    public AudioClip swordWoosh;
    public AudioClip dirtHit;
    private AudioSource audioSource;



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<NewPlayerController>();

        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
        swordCollider = GetComponent<Collider2D>();
        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        

        Physics2D.IgnoreCollision(swordCollider, playerCollider, true);

        isMoving = true;
        isStuck = false;
        isReturning = false;

        flyingTimer();


    }

    public void setAngle(Vector2 angle)
    {
        direction = angle.normalized;
        transform.right = direction;
    }


    public void checkStuck()
    {
        Vector2 pointPosition = transform.position + (Vector3)(direction * tipOffset);

        Collider2D hitCast = Physics2D.OverlapBox(transform.position + (Vector3)(direction * tipOffset), new Vector2(0.1f, 0.1f), 0f, LayerMask.GetMask("Wall Tiles", "Ground Tiles"));

        if (hitCast != null)
        {


            bool hitGround = hitCast.gameObject.layer == LayerMask.NameToLayer("Ground Tiles");
            bool hitWall = hitCast.gameObject.layer == LayerMask.NameToLayer("Wall Tiles");

            if (hitGround || hitWall)
            {
                if (hitGround)
                {
                    if (!plinkCooldown)
                    {
                        initiatePlinkCooldown();
                        audioSource.PlayOneShot(swordPlink);
                    }
                    returnToPlayer();
                    return;
                }
                transform.position = hitCast.ClosestPoint(transform.position) - direction * (tipOffset - 0.05f);
                isStuck = true;
                stickToWall();
            }
        }
    }

    public void stickToWall()
    {
        audioSource.PlayOneShot(dirtHit);
        isMoving = false;
        if (direction.x  < 0f) transform.rotation = Quaternion.Euler(0f, 0f, 180f);
        else transform.rotation = Quaternion.identity;

        animator.SetTrigger("Bounce");
        //Physics2D.IgnoreCollision(swordCollider, playerCollider, false);
    }

    public void returnToPlayer()
    {
        if (isMoving) isMoving = false;

        if (!plinkCooldown)
        {
            audioSource.clip = swordWoosh;
            audioSource.loop = true;
            audioSource.Play();
        }

        isReturning = true;
        animator.SetTrigger("Spin");
        Physics2D.IgnoreCollision(swordCollider, playerCollider, true);

    }

    private async void flyingTimer()
    {
        await Task.Delay(1500);
        if (!isStuck)
        {
            returnToPlayer();
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isStuck && !isReturning)
        {
            //checkStuck();
            transform.position += (Vector3)(direction * speed * 2 * Time.fixedDeltaTime);
        }


        if (isReturning)
        {
            Vector2 directionToPlayer = (playerTransform.transform.position - transform.position).normalized;
            transform.position += (Vector3)(directionToPlayer * speed * 3 * Time.fixedDeltaTime);

            if (Vector2.Distance(transform.position, playerTransform.position) < 0.5f)
            {
                audioSource.Stop();
                isReturning = false;
                playerScript.grabSword();
                Destroy(gameObject);
            }
        }
    }

    private async void initiateBounceCooldown()
    {
        if (bounceCooldown) return; 

        bounceCooldown = true;
  

        await Task.Delay(200);

        bounceCooldown = false;
  
    }

    private async void initiatePlinkCooldown()
    {
        if (plinkCooldown) return;

        plinkCooldown = true;


        await Task.Delay(300);

        plinkCooldown = false;

    }

    private void Update()
    {
        if (isStuck && !isReturning && !bounceCooldown)
        {
            Collider2D hit = Physics2D.OverlapBox(
                swordCollider.bounds.center,
                swordCollider.bounds.size,
                0f,
                LayerMask.GetMask("Player")
);
            
            if (hit != null && !bounceCooldown)
            {
                //bounceCooldown = true;
                animator.SetTrigger("Bounce");
                initiateBounceCooldown();
                playerScript.SwordBounce();
            }
        }

        if (!isStuck)
        {
            checkStuck();
        }
    }
}
