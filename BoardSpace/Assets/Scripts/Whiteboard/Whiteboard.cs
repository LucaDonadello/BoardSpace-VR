using UnityEngine;

public class Whiteboard : MonoBehaviour
{
    public Texture2D whiteboardTexture;
    public Vector2 textureSize = new Vector2(2048, 2048);
    public Color backgroundColor = Color.white;

    void Start()
    {
        var r = GetComponent<Renderer>();
        whiteboardTexture = new Texture2D((int)textureSize.x, (int)textureSize.y);
        r.material.mainTexture = whiteboardTexture;
    }
}
