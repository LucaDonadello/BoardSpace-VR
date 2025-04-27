using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

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
    private int currentCharacterIndex = 0;

    private Button leftArrowButton;
    private Button rightArrowButton;
    private Button selectButton;
    private Button exitButton;
    private int selectedOption = 0; // 0 = Arrows, 1 = Select, 2 = Exit

    private Color leftArrowOriginalColor;
    private Color rightArrowOriginalColor;
    private Color selectButtonOriginalColor;
    private Color exitButtonOriginalColor;

    private bool verticalInUse = false;
    private bool horizontalInUse = false;

    void Start()
    {
        if (!photonView.IsMine) return;
        UpdateCharacterDisplay();
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        playerMovement = player.GetComponent<CharacterMovement>();
        playerController = player.GetComponent<CharacterController>();
        playerTeleport = player.GetComponent<Teleport>();
        playerTeleportToRooms = player.GetComponent<TeleportToRooms>();
        cam = player.transform.Find("XRCardboardRig/HeightOffset/Main Camera");
        characterSkin = player.transform.Find("CharacterSkin").gameObject;
        defaultSkin = characterSkin.transform.Find("DefaultBro").gameObject;

        leftArrowButton = GameObject.Find("LeftArrow").GetComponent<Button>();
        rightArrowButton = GameObject.Find("RightArrow").GetComponent<Button>();
        selectButton = GameObject.Find("SelectButton").GetComponent<Button>();
        exitButton = GameObject.Find("ExitButton").GetComponent<Button>();

        leftArrowOriginalColor = leftArrowButton.GetComponent<Image>().color;
        rightArrowOriginalColor = rightArrowButton.GetComponent<Image>().color;
        selectButtonOriginalColor = selectButton.GetComponent<Image>().color;
        exitButtonOriginalColor = exitButton.GetComponent<Image>().color;

        // Try to restore skin if any
        if (PhotonNetwork.LocalPlayer.CustomProperties.ContainsKey("SelectedSkin"))
        {
            string skinName = PhotonNetwork.LocalPlayer.CustomProperties["SelectedSkin"] as string;

            if (!string.IsNullOrEmpty(skinName))
            {
                RestoreSelectedSkin(skinName);
            }
        }

        UpdateButtonHighlights();
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
            HandleMenuNavigation();
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
        currentCharacterIndex = (currentCharacterIndex + 1) % characters.Length;
        UpdateCharacterDisplay();
    }

    public void PreviousCharacter()
    {
        currentCharacterIndex = (currentCharacterIndex - 1 + characters.Length) % characters.Length;
        UpdateCharacterDisplay();
    }

    void UpdateCharacterDisplay()
    {
        displayer.sprite = characters[currentCharacterIndex].icon;
        characterNameText.text = characters[currentCharacterIndex].characterName;
    }

    public void ExitMenu()
    {
        selectedOption = 0; // Reset selected option
        UpdateButtonHighlights();
        gameObject.SetActive(false);
        EnablePlayerControls();
    }

    public void SelectCharacter()
    {
        if (!photonView.IsMine) return;

        CharacterData selectedCharacterData = characters[currentCharacterIndex];

        if (currentPlayerInstance != null)
        {
            PhotonNetwork.Destroy(currentPlayerInstance); // Destroy the previous instance if it exists
        }

        currentPlayerInstance = PhotonNetwork.Instantiate(
            selectedCharacterData.characterPrefab.name,
            characterSkin.transform.position,
            Quaternion.identity
        );

        currentPlayerInstance.transform.SetParent(characterSkin.transform, false); // Set the parent to the character skin

        // Locally parent it so it looks good immediately
        currentPlayerInstance.transform.SetParent(characterSkin.transform, true);
        currentPlayerInstance.transform.localPosition = Vector3.zero;
        currentPlayerInstance.transform.localRotation = Quaternion.identity;

        // Then notify everyone including yourself
        photonView.RPC(
            "SetParentToCharacterSkin",
            RpcTarget.AllBuffered,
            currentPlayerInstance.GetComponent<PhotonView>().ViewID,
            characterSkin.GetComponent<PhotonView>().ViewID
        );

        photonView.RPC(
            "DisableDefaultSkin",
            RpcTarget.AllBuffered,
            defaultSkin.GetComponent<PhotonView>().ViewID
        );

        ExitMenu();

        // Save selected skin name in Custom Properties
        Hashtable playerProperties = new Hashtable();
        playerProperties["SelectedSkin"] = selectedCharacterData.characterPrefab.name;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperties);
    }

    void RestoreSelectedSkin(string prefabName)
    {
        if (currentPlayerInstance != null)
        {
            PhotonNetwork.Destroy(currentPlayerInstance);
        }

        currentPlayerInstance = PhotonNetwork.Instantiate(
            prefabName,
            characterSkin.transform.position,
            Quaternion.identity
        );

        currentPlayerInstance.transform.SetParent(characterSkin.transform, true);
        currentPlayerInstance.transform.localPosition = Vector3.zero;
        currentPlayerInstance.transform.localRotation = Quaternion.identity;

        // Send parent sync to others
        photonView.RPC(
            "SetParentToCharacterSkin",
            RpcTarget.AllBuffered,
            currentPlayerInstance.GetComponent<PhotonView>().ViewID,
            characterSkin.GetComponent<PhotonView>().ViewID
        );
    }

    void HandleMenuNavigation()
    {

        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        if (!verticalInUse)
        {
            if (v > 0.5f || Input.GetKeyDown(KeyCode.UpArrow))
            {
                selectedOption = (selectedOption - 1 + 3) % 3; // Move up
                UpdateButtonHighlights();
            }
            else if (v < -0.5f || Input.GetKeyDown(KeyCode.DownArrow))
            {
                selectedOption = (selectedOption + 1) % 3; // Move down
                UpdateButtonHighlights();
            }
        }
        else if (Mathf.Abs(v) < 0.2f)
        {
            verticalInUse = false;
        }

        if (selectedOption == 0)
        {
            if (!horizontalInUse)
            {
                if (h < -0.5f || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    PreviousCharacter(); // Move left
                    horizontalInUse = true;
                }
                else if (h > 0.5f || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    NextCharacter(); // Move right
                    horizontalInUse = true;
                }
            }
            else if (Mathf.Abs(h) < 0.2f)
            {
                horizontalInUse = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("js10"))
        {
            switch (selectedOption)
            {
                case 1:
                    SelectCharacter();
                    break;
                case 2:
                    ExitMenu();
                    break;
                default:
                    // Do nothing for arrows
                    break;
            }
        }
    }

    void UpdateMenuOrientation()
    {
        Vector3 targetPosition = cam.position + cam.forward * 5f;
        targetPosition.y = 2.5f; // always 5 units above ground level (y = 0)
        transform.position = targetPosition;
        transform.LookAt(cam);
        transform.Rotate(0, 180f, 0);
    }

    void UpdateButtonHighlights()
    {
        // Reset to original colors
        ColorBlock leftColors = leftArrowButton.colors;
        ColorBlock rightColors = rightArrowButton.colors;
        ColorBlock selectColors = selectButton.colors;
        ColorBlock exitColors = exitButton.colors;

        leftColors.normalColor = leftArrowOriginalColor;
        rightColors.normalColor = rightArrowOriginalColor;
        selectColors.normalColor = selectButtonOriginalColor;
        exitColors.normalColor = exitButtonOriginalColor;

        // Highlight selected buttons
        Color selectedColor = Color.yellow;

        switch (selectedOption)
        {
            case 0:
                leftColors.normalColor = selectedColor;
                rightColors.normalColor = selectedColor;
                break;
            case 1:
                selectColors.normalColor = selectedColor;
                break;
            case 2:
                exitColors.normalColor = selectedColor;
                break;
        }

        // Apply the color changes
        leftArrowButton.colors = leftColors;
        rightArrowButton.colors = rightColors;
        selectButton.colors = selectColors;
        exitButton.colors = exitColors;
    }
}
