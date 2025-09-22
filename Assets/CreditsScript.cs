using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public class CreditsScript : MonoBehaviour
{
    public float speed = 1f;   // units per second


    async void Start()
    {
        await Task.Delay(50000);
        returnToTitle();
    }

    private void returnToTitle()
    {
        SceneManager.LoadScene("Menu");
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            returnToTitle();
        }
        // Move camera downward over time
        transform.position += Vector3.down * speed * Time.deltaTime;
    }
}