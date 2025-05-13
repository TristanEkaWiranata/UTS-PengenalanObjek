using UnityEngine;
using UnityEngine.UI;

public class ExitConfirmation : MonoBehaviour
{
    [Header("UI References")]
    public GameObject confirmationPanel;
    public Button yesButton;
    public Button noButton;

    // Action yang akan dilakukan jika user menekan "Yes"
    private System.Action onConfirmAction;

    void Start()
    {
        confirmationPanel.SetActive(false);

        // Tombol "Tidak"
        if (noButton != null)
        {
            noButton.onClick.RemoveAllListeners();
            noButton.onClick.AddListener(() => confirmationPanel.SetActive(false));
        }

        // Tombol "Ya"
        if (yesButton != null)
        {
            yesButton.onClick.RemoveAllListeners();
            yesButton.onClick.AddListener(() =>
            {
                confirmationPanel.SetActive(false);
                onConfirmAction?.Invoke();
            });
        }
    }

    // Fungsi umum untuk membuka konfirmasi dan menerima aksi yang akan dilakukan
    public void ShowConfirmation(System.Action confirmAction)
    {
        onConfirmAction = confirmAction;
        confirmationPanel.SetActive(true);
    }
}
