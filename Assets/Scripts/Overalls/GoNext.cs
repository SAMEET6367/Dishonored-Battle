using UnityEngine;
using UnityEngine.SceneManagement;

public class GoNext : MonoBehaviour
{
    public void Next()
    {
        SceneManager.LoadScene(2);
    }
}