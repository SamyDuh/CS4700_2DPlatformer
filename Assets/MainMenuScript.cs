using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{


    // Start is called before the first frame update
    public void StartGame()
    {
        print("Starting game...");
        SceneManager.LoadScene("Level1");
    }


    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
