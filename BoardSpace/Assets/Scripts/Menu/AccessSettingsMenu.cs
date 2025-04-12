using UnityEngine;
using Photon.Pun;

public class AccessSettingsMenu : MonoBehaviourPun
{
    public GameObject settingsMenuRoot;

    void Start()
    {
        if (settingsMenuRoot == null)
        {
            Debug.LogError("Settings menu root not assigned!");
            return;
        }
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetKeyDown(KeyCode.O))
        {
            if (settingsMenuRoot != null)
            {
                settingsMenuRoot.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Settings menu not found!");
            }
        }
    }
}
