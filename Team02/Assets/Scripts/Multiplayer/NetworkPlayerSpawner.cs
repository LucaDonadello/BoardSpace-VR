using UnityEngine;
using Photon.Pun;
using UnityEngine.EventSystems;

public class NetworkPlayerSpawner : MonoBehaviourPunCallbacks
{
    private GameObject spawnedPlayerPrefab;

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // Spawn the player prefab at spawner position
        spawnedPlayerPrefab = PhotonNetwork.Instantiate("Network Player", transform.position, transform.rotation);

        var photonView = spawnedPlayerPrefab.GetComponent<PhotonView>();

        if (photonView.IsMine)
        {
            // Enable only this player's camera/audio
            EnableLocalPlayerComponents(spawnedPlayerPrefab);
        }
        //else
        //{
        //    // Disable camera/audio for remote players
        //    DisableRemotePlayerComponents(spawnedPlayerPrefab);
        //}
    }

    private void EnableLocalPlayerComponents(GameObject player)
    {
        Camera cam = player.GetComponentInChildren<Camera>(true);
        AudioListener listener = player.GetComponentInChildren<AudioListener>(true);

        if (cam) cam.enabled = true;
        if (listener) listener.enabled = true;

        EventSystem eventSystem = Object.FindFirstObjectByType<EventSystem>();
        if (eventSystem) eventSystem.enabled = true;

        StandaloneInputModule inputModule = Object.FindFirstObjectByType<StandaloneInputModule>();
        if (inputModule) inputModule.enabled = true;
    }

    //private void DisableRemotePlayerComponents(GameObject player)
    //{
    //    Camera cam = player.GetComponentInChildren<Camera>(true);
    //    AudioListener listener = player.GetComponentInChildren<AudioListener>(true);

    //    if (cam) cam.enabled = false;
    //    if (listener) listener.enabled = false;
    //}

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();

        if (spawnedPlayerPrefab != null)
            PhotonNetwork.Destroy(spawnedPlayerPrefab);
    }
}
