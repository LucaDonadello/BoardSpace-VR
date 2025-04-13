using UnityEngine;
using Photon.Pun;

public class FaceMenu : MonoBehaviourPun
{
    private float distance = 3.0f; // Distance in front of the camera
    private float fixedY = 3.0f;   // Fixed Y position
    private GameObject settingsPanel; // Reference to the settings menu

    void Start()
    {
        if (!photonView.IsMine) return;
        settingsPanel = transform.GetChild(0).GetChild(0).gameObject;
    }

    void LateUpdate()
    {
        if (!photonView.IsMine) return;
        if (Camera.main == null) return;

        Vector3 newPos = Camera.main.transform.forward * distance + Camera.main.transform.position;
        newPos.y = fixedY; // Set the Y position to fixedY
        settingsPanel.transform.position = newPos; // Set the Y position to fixedY
        settingsPanel.transform.LookAt(Camera.main.transform);
        settingsPanel.transform.Rotate(0, 180f, 0);
    }
}
