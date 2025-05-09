using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void ExitToDesktop()
    {
        Debug.Log("Exit button clicked! Application will quit in build.");
        Application.Quit();
    }
}