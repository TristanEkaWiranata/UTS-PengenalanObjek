using UnityEngine;

public class ExitScene : MonoBehaviour
{
    public void ExitToDesktop()
    {
        Debug.Log("Exit button clicked! Application will quit in build.");
        Application.Quit();
    }
}