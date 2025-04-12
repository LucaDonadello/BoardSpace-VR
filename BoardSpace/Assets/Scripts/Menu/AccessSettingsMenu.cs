using UnityEngine;

public class AccessSettingsMenu : MonoBehaviour
{
    public GameObject settingsMenuRoot;

    void Start()
    {
        if (settingsMenuRoot == null)
        {
            Debug.LogError("Settings menu root not assigned!");
            return;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            if (settingsMenuRoot != null)
            {
                settingsMenuRoot.SetActive(true);
            }
            else
            {
                Debug.LogWarning("Settings menu not found!");
            }
        }
    }
}
