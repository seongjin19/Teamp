using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    
    public void GoToMain()
    {
        SceneManager.LoadScene(1);
    }

    public void ReturnToGame()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void GoToStage1()
    {
        SceneManager.LoadScene(2);
    }
    public void GoToStage2()
    {
        SceneManager.LoadScene(3);
    }
    public void GoToStage3()
    {
        SceneManager.LoadScene(4);
    }

}
