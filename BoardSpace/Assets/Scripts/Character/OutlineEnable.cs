//Attach this to any object to enable an outline when the pointer enters it
//and disable the outline when it exits
using UnityEngine;
using UnityEngine.EventSystems;
public class OutlineEnable : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Outline outline;

    void Start()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        outline.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        outline.enabled = false;
    }
}