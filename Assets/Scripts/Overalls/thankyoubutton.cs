using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonSceneLoader : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene(0);
    }
}