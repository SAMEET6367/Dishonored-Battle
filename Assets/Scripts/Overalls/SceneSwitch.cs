using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public void Back()
    {
        SceneManager.LoadScene(0);
    }
}
