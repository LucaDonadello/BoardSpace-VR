using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterSelectionMenu : MonoBehaviour
{
    public CharacterData[] characters;
    public Image displayer;
    public TextMeshProUGUI characterNameText;
    public GameObject player;
    private CharacterMovement playerMovement;
    private Teleport playerTeleport;
    private TeleportToRooms playerTeleportToRooms;
    private int currentIndex = 0;

    void Start()
    {
        UpdateCharacterDisplay();
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<CharacterMovement>();
        playerTeleport = player.GetComponent<Teleport>();
        playerTeleportToRooms = player.GetComponent<TeleportToRooms>();
    }

    void Update()
    {
        // If character Selection is active, disable
        // player movement and other controls
        if (gameObject.activeSelf)
        {
            DisablePlayerControls();
        }
    }

    void DisablePlayerControls()
    {
        playerMovement.enabled = false;
        playerTeleport.enabled = false;
        playerTeleportToRooms.enabled = false;
    }

    public void EnablePlayerControls()
    {
        playerMovement.enabled = true;
        playerTeleport.enabled = true;
        playerTeleportToRooms.enabled = true;
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

    public void SelectCharacter()
    {
        // Instantiate the selected character prefab in the game world
        GameObject selectedCharacter = Instantiate(characters[currentIndex].characterPrefab, Vector3.zero, Quaternion.identity);
        Debug.Log("Selected Character: " + characters[currentIndex].characterName);
    }

    public void ExitMenu()
    {
        gameObject.SetActive(false);
        EnablePlayerControls();
    }
}
