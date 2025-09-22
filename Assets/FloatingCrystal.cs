using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FloatingCrystal : MonoBehaviour
{
    public bool collected;

    private SpriteRenderer spriteRenderer;

    private CameraScript cameraScript;

    private GameObject mainCamera;
    private GameObject player;

    private Collider2D crystalCollider;
    private Collider2D playerCollider;


    private Vector3 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        crystalCollider = GetComponent<Collider2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        spriteRenderer = gameObject.AddComponent<SpriteRenderer>();

        playerCollider = GameObject.FindGameObjectWithTag("Player").GetComponent<Collider2D>();



        cameraScript = mainCamera.GetComponent<CameraScript>();


        Physics2D.IgnoreCollision(crystalCollider, playerCollider, true);
    }

    // Update is called once per frame
    void Update()
    {
        float y = Mathf.Sin(Time.time * 3) * .2f;
        transform.position = startPos + new Vector3(0, y, 0);


        if (!collected)
        {
            Collider2D hit = Physics2D.OverlapBox(
                    crystalCollider.bounds.center,
                    crystalCollider.bounds.size,
                    0f,
                    LayerMask.GetMask("Player")



        );

            if (hit != null)
            {
                collected = true;
                cameraScript.incrementMoon();
                Destroy(gameObject);


            }
        }
    }
}
