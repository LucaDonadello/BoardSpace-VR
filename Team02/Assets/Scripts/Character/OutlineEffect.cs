using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;

public class OutlineEffect : MonoBehaviourPun
{
    [Header("Raycast Settings")]
    public Transform rayOrigin;
    public Transform player;
    public float maxDistance = 10f;

    [Header("Tags to Outline (partial match allowed)")]
    public List<string> targetTags = new List<string> { "Door", "Grabbable", "Interactable",};

    private Outline lastOutlinedObject = null;

    public PlayerData playerData;

    void Start()
    {
        playerData = player.GetComponent<PlayerData>();
        maxDistance = playerData.playerRayLength;
    }

    void Update()
    {
        maxDistance = playerData.playerRayLength;
        if (!photonView.IsMine) return;
        Ray ray = new Ray(rayOrigin.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance))
        {
            Collider hitCollider = hitInfo.collider;
            string tag = hitCollider.tag;

            //Debug.Log($"Hit Object: {hitCollider.name}, Tag: {tag}");

            // Check if tag contains any of the defined target tags
            bool isMatchingTag = targetTags.Exists(t => tag.Contains(t));
            if (isMatchingTag)
            {
                Outline outline = hitCollider.GetComponent<Outline>();
                if (outline == null)
                    return;

                if (lastOutlinedObject != null && lastOutlinedObject != outline)
                {
                    lastOutlinedObject.enabled = false;
                }

                outline.enabled = true;
                lastOutlinedObject = outline;
                return;
            }
        }

        // No valid object hit or tag doesn't match
        if (lastOutlinedObject != null)
        {
            lastOutlinedObject.enabled = false;
            lastOutlinedObject = null;
        }
    }
}
