using UnityEngine;
using Photon.Pun;

public class RotateWithCam : MonoBehaviourPun
{
    void Update()
    {
        if (!photonView.IsMine) return;

        Quaternion targetRotation = Quaternion.Euler(0, Camera.main.transform.rotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 5f); // 5f = smoothing speed
    }
}
