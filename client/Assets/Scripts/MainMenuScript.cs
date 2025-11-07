using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void StartGame()
    {   
        // Additional Option: SceneManager.LoadScene("GameSceneName");
        SceneManager.LoadScene(1); 
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
