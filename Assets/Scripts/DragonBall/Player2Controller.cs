using UnityEngine;
using System.Collections;

public class Player2Controller : MonoBehaviour
{
    private PLayerMovement playerMovement;
    private AIPlayerMovement aiMovement;

   IEnumerator Start()
    {
        // Ambil referensi komponen PlayerMovement dan AIPlayerMovement
        playerMovement = GetComponent<PLayerMovement>();
        aiMovement = GetComponent<AIPlayerMovement>();

        // Pastikan GameManager sudah terinisialisasi
        yield return new WaitUntil(() => DragonBallGameManager.instance != null);

        // Debug log untuk memeriksa mode yang aktif
        Debug.Log("Game Mode saat ini: " + DragonBallGameManager.instance.currentMode);

        // Cek mode permainan dari GameManager
        if (DragonBallGameManager.instance.currentMode == DragonBallGameManager.GameMode.PVP)
        {
            // Mode PVP: Aktifkan kontrol player, nonaktifkan AI
            if (playerMovement != null)
                playerMovement.enabled = true;
            if (aiMovement != null)
                aiMovement.enabled = false;

            Debug.Log("Player 2: PVP mode activated");
        }
        else if (DragonBallGameManager.instance.currentMode == DragonBallGameManager.GameMode.PVE)
        {
            // Mode PVE: Aktifkan AI, nonaktifkan kontrol player
            if (playerMovement != null)
                playerMovement.enabled = false;
            if (aiMovement != null)
                aiMovement.enabled = true;

            Debug.Log("Player 2: PVE mode activated");
        }
        else
        {
            Debug.LogWarning("Game mode tidak dikenali");
        }
    }

}
