using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class CharacterSelectionMenu : MonoBehaviourPun
{
    public CharacterData[] characters; // Assign in Inspector
    public Image displayer;
    public TextMeshProUGUI characterNameText;
    public GameObject player;
    private GameObject currentPlayerInstance;
    private GameObject characterSkin;
    private GameObject defaultSkin;
    private Transform cam;
    private CharacterController playerController;
    private CharacterMovement playerMovement;
    private Teleport playerTeleport;
    private TeleportToRooms playerTeleportToRooms;
    private int currentIndex = 0;

    void Start()
    {
        if (!photonView.IsMine) return;
        UpdateCharacterDisplay();
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<CharacterMovement>();
        playerController = player.GetComponent<CharacterController>();
        playerTeleport = player.GetComponent<Teleport>();
        playerTeleportToRooms = player.GetComponent<TeleportToRooms>();
        cam = player.transform.Find("XRCardboardRig/HeightOffset/Main Camera");
        characterSkin = player.transform.Find("CharacterSkin").gameObject;
        defaultSkin = characterSkin.transform.Find("DefaultBro").gameObject;
    }

    void Update()
    {
        if (!photonView.IsMine) return;
        // If character Selection is active, disable
        // player movement and other controls
        if (gameObject.activeSelf)
        {
            DisablePlayerControls();
            UpdateMenuOrientation();
        }
    }

    void DisablePlayerControls()
    {
        playerMovement.enabled = false;
        playerTeleport.enabled = false;
        playerTeleportToRooms.enabled = false;
        playerController.enabled = false;
    }

    public void EnablePlayerControls()
    {
        playerMovement.enabled = true;
        playerTeleport.enabled = true;
        playerTeleportToRooms.enabled = true;
        playerController.enabled = true;
    }

    public void NextCharacter()
    {
        currentIndex = (currentIndex + 1) % characters.Length;
        UpdateCharacterDisplay();
    }

    public void PreviousCharacter()
    {
        currentIndex = (currentIndex - 1 + characters.Length) % characters.Length;
        UpdateCharacterDisplay();
    }

    void UpdateCharacterDisplay()
    {
        displayer.sprite = characters[currentIndex].icon;
        characterNameText.text = characters[currentIndex].characterName;
    }

    public void ExitMenu()
    {
        gameObject.SetActive(false);
        EnablePlayerControls();
    }

    public void SelectCharacter()
    {
        // Instantiate the selected character prefab in the game world
        CharacterData selectedCharacterData = characters[currentIndex];
        if (currentPlayerInstance != null)
        {
            Destroy(currentPlayerInstance); // Destroy the previous instance if it exists
        }
        currentPlayerInstance = Instantiate(selectedCharacterData.characterPrefab, characterSkin.transform);
        currentPlayerInstance.transform.localRotation = Quaternion.identity;
        currentPlayerInstance.transform.localPosition = new Vector3(0, 0, 0);
        if (defaultSkin.activeSelf)
        {
            defaultSkin.SetActive(false); // Hide the default skin
        }
        ExitMenu();
    }

    void UpdateMenuOrientation()
    {
        Vector3 targetPosition = cam.position + cam.forward * 5f;
        targetPosition.y = 2.5f; // always 5 units above ground level (y = 0)
        transform.position = targetPosition;
        transform.LookAt(cam);
        transform.Rotate(0, 180f, 0);
    }
}
