using UnityEngine;

//used to easily switch between Android and Windows controls with the click of a button
//X on controller = Y on keyboard
//Y on controller = G on keyboard
//OK on controller = 0 on keyboard
//A on controller = B on keyboard
public class ButtonMapping : MonoBehaviour
{
    public enum Platform { Windows, Android }
    public Platform currentPlatform = Platform.Windows;

    public static ButtonMapping Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this.gameObject);
        else
            Instance = this;
    }

    public bool GetActionDown(string button)
    {
        // Check both keyboard and joystick button
        return Input.GetKeyDown(GetKeyCode(button)) || Input.GetButtonDown(GetButton(button));
    }

    private KeyCode GetKeyCode(string key)
    {
        return key switch
        {
            "X" => KeyCode.Y,         
            "Y" => KeyCode.G,         
            "OK" => KeyCode.Alpha0,   
            "A" => KeyCode.B,         
            _ => KeyCode.None
        };
    }

    private string GetButton(string button)
    {
        return currentPlatform switch
        {
            Platform.Android => button switch
            {
                "OK" => "js0",
                "X" => "js2",
                "Y" => "js3",
                "A" => "js10",
                _ => button
            },
            Platform.Windows => button switch
            {
                "OK" => "js3",
                "X" => "js1",
                "Y" => "js0",
                "A" => "js8",
                _ => button
            },
            _ => button
        };
    }
}
