using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{
    public Transform playerTransform;

    public int moonCount = 0;
    public Text moonText;

    public AudioClip collectMoon;

    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    public void incrementMoon()
    {
        moonCount++;
        moonText.text = moonCount.ToString();
        audioSource.PlayOneShot(collectMoon);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        
        Vector3 cameraPosition = transform.position;
        cameraPosition.x = playerTransform.position.x;
        transform.position = cameraPosition;    
    }
}
