using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;

public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    private GameObject spawnedPlayerPrefab;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // Spawn player prefab for the local player
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("Network Player", transform.position, transform.rotation);

        // Get the PhotonView component
        var photonView = spawnedPlayerPrefab.GetComponent<PhotonView>();

        // Get the camera and audio listener components (ensure only local player's camera is enabled)
        Camera cam = spawnedPlayerPrefab.GetComponentInChildren<Camera>();
        AudioListener listener = spawnedPlayerPrefab.GetComponentInChildren<AudioListener>();

        if (photonView.IsMine)
        {
            // Enable camera and audio listener for local player only
            if (cam != null)
            {
                cam.enabled = true;  // Enable camera for local player
            }

            if (listener != null)
            {
                listener.enabled = true;  // Enable audio listener for local player
            }

            // Enable global EventSystem and InputModule for the local player
            EventSystem eventSystem = Object.FindFirstObjectByType<EventSystem>();
            if (eventSystem != null)
            {
                eventSystem.enabled = true;
            }

            StandaloneInputModule inputModule = Object.FindFirstObjectByType<StandaloneInputModule>();
            if (inputModule != null)
            {
                inputModule.enabled = true;
            }
        }
        else
        {
            // Disable camera and audio listener for remote players
            if (cam != null)
            {
                cam.enabled = false; // Disable camera for remote players
            }
            if (listener != null)
            {
                listener.enabled = false; // Disable audio listener for remote players
            }
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        // Destroy the player prefab when leaving the room
        if (spawnedPlayerPrefab != null)
            PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }
}
