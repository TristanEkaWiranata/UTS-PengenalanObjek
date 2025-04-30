using UnityEngine;

public class SettingsMenuToggle : MonoBehaviour
{
    public GameObject settingsPanel;

    public void ToggleSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(!settingsPanel.activeSelf);
        }
    }
}
