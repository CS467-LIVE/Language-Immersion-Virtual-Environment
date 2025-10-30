using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void StartGame()
    {   
        // Additional Option: SceneManager.LoadScene("GameSceneName");
        SceneManager.LoadScene("TestScene"); 
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
