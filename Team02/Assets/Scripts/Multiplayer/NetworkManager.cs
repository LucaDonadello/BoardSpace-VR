using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon.Voice.Unity; // Required for Photon Voice

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public VoiceConnection voiceConnection; // Assign this in the Inspector

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Connecting to server...");
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to server");
        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = 10,
            IsVisible = true,
            IsOpen = true
        };
        PhotonNetwork.JoinOrCreateRoom("Room1", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room");

        // Safely connect Photon Voice using the same region as PUN
        if (voiceConnection != null && !voiceConnection.Client.IsConnected)
        {
            string currentRegion = PhotonNetwork.CloudRegion;
            Debug.Log($"Connecting Photon Voice to region: {currentRegion}");

            voiceConnection.Client.AppId = voiceConnection.Settings.AppIdVoice;
            voiceConnection.Client.ConnectToRegionMaster(currentRegion);
        }
        else
        {
            Debug.LogWarning("VoiceConnection not assigned or already connected.");
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("Player joined room");
    }
}
