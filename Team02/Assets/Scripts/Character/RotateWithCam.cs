using UnityEngine;
using Photon.Pun;

public class RotateWithCam : MonoBehaviourPun
{
    void Update()
    {
        if (!photonView.IsMine) return;

        // Rotate the object to match the camera's rotation in y direction only
        transform.rotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
    }
}
