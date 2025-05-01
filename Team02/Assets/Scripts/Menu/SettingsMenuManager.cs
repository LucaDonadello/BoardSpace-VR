using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class SettingsMenuManager : MonoBehaviourPun
{
    public Button[] buttons; // Assign in Inspector
    private int selectedIndex = 0;

    private float masterVolume = 1f;
    private int textSizeIndex = 1;
    private int rayLength = 10;
    private readonly string[] textSizes = { "Small", "Medium", "Large" };

    private bool verticalInUse = false;
    private bool horizontalInUse = false;

    private GameObject player;
    private GameObject characterSelection;
    private MonoBehaviour[] componentsToDisable;
    private PlayerData playerData;

    void Start()
    {
        if (!photonView.IsMine) return;
        UpdateAllButtonLabels();
        HighlightSelectedButton();

        player = GameObject.FindGameObjectWithTag("Player");
        playerData = player.GetComponent<PlayerData>();

        componentsToDisable = new MonoBehaviour[]
        {
            player.GetComponent<CharacterMovement>(),
            player.GetComponent<Teleport>(),
            player.GetComponent<TeleportToRooms>(),
            player.GetComponent<SitOnSofa>()
        };
        characterSelection = player.transform.Find("CharacterSelection").gameObject;
    }

    void Update()
    {
        if (!photonView.IsMine) return;

        DisablePlayerControls();
        float v = Input.GetAxis("Vertical");
        float h = Input.GetAxis("Horizontal");

        // Vertical movement (Up/Down)
        if (!verticalInUse)
        {
            if (v > 0.5f || Input.GetKeyDown(KeyCode.UpArrow))
            {
                MoveSelection(-1); // Up
                verticalInUse = true;
            }
            else if (v < -0.5f || Input.GetKeyDown(KeyCode.DownArrow))
            {
                MoveSelection(1); // Down
                verticalInUse = true;
            }
        }
        else if (Mathf.Abs(v) < 0.2f)
        {
            verticalInUse = false;
        }

        // Horizontal adjustment (Left/Right)
        if (!horizontalInUse)
        {
            if (h < -0.5f || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                AdjustValue(-1); // Left
                horizontalInUse = true;
            }
            else if (h > 0.5f || Input.GetKeyDown(KeyCode.RightArrow))
            {
                AdjustValue(1); // Right
                horizontalInUse = true;
            }
        }
        else if (Mathf.Abs(h) < 0.2f)
        {
            horizontalInUse = false;
        }

        if (Input.GetKeyDown(KeyCode.B) || Input.GetButtonDown("js10")) // Press A on the controller
        {
            ActivateCurrentButton();
        }
    }

    void DisablePlayerControls()
    {
        foreach (var comp in componentsToDisable)
        {
            if (comp != null) comp.enabled = false;
        }
    }

    void EnablePlayerControls()
    {
        foreach (var comp in componentsToDisable)
        {
            if (comp != null) comp.enabled = true;
        }
    }

    void MoveSelection(int dir)
    {
        selectedIndex = (selectedIndex + dir + buttons.Length) % buttons.Length;
        HighlightSelectedButton();
    }

    void HighlightSelectedButton()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            var colors = buttons[i].colors;
            colors.normalColor = (i == selectedIndex) ? Color.yellow : Color.white;
            buttons[i].colors = colors;
        }
    }

    void AdjustValue(int dir)
    {
        string btnName = buttons[selectedIndex].name;
        var label = buttons[selectedIndex].GetComponentInChildren<TextMeshProUGUI>();
        switch (btnName)
        {
            case "MasterVolumeButton":
                masterVolume = Mathf.Clamp01(masterVolume + dir * 0.1f);
                label.text = $"Master Volume: {Mathf.RoundToInt(masterVolume * 100)}%";
                break;

            case "TextSizeButton":
                textSizeIndex = (textSizeIndex + dir + textSizes.Length) % textSizes.Length;
                label.text = $"Text Size: {textSizes[textSizeIndex]}";
                break;

            case "RayLengthButton":
                rayLength = Mathf.Clamp(rayLength + dir * 5, 5, 20);
                label.text = $"Ray Length: {rayLength}m";
                SetRayLength(rayLength);
                break;
        }
    }

    void SetRayLength(float length)
    {
        playerData.playerRayLength = length;
    }

    void ActivateCurrentButton()
    {
        string btnName = buttons[selectedIndex].name;

        switch (btnName)
        {
            case "SkinSelectionButton":
                characterSelection.SetActive(true);
                EnablePlayerControls();
                gameObject.SetActive(false);
                break;

            case "ResumeButton":
                EnablePlayerControls();
                selectedIndex = 0; // Reset selection index
                HighlightSelectedButton();
                gameObject.SetActive(false);
                break;

            case "QuitButton":
                EnablePlayerControls();
                gameObject.SetActive(false);
                PhotonNetwork.LeaveRoom();
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
                break;
        }
    }

    void UpdateAllButtonLabels()
    {
        foreach (var button in buttons)
        {
            var label = button.GetComponentInChildren<TextMeshProUGUI>();
            switch (button.name)
            {
                case "MasterVolumeButton":
                    label.text = $"Master Volume: {(int)(masterVolume * 100)}%";
                    break;
                case "TextSizeButton":
                    label.text = $"Text Size: {textSizes[textSizeIndex]}";
                    break;
                case "RayLengthButton":
                    label.text = $"Ray Length: {rayLength}m";
                    break;
            }
        }
    }
}
