using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class Grab : MonoBehaviour
{
    private bool grabbed;
    private bool onObject;
    public GameObject itemToGrab; 
    private Transform grabPosition; //set to OutlineEffect 

    void Start()
    {
        grabbed = false;
        onObject = false;
        grabPosition = OutlineEffect.grabPosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) || Input.GetButtonDown("js3"))
        {
            if(!grabbed && onObject)
            {
                grabbed = true;
                return;
            }
        }

        if (Input.GetKeyDown(KeyCode.F) || Input.GetButtonDown("js4"))
        {
            if(grabbed)
            {
                UnlockFromCamera();
                grabbed = false;
                return;            
            }
        }

        if(grabbed)
        {
            //update grabPosition every frame
            grabPosition = OutlineEffect.grabPosition;          
            LockToCamera();  
            //keep object upright (great for game pieces)
            Vector3 forward = Vector3.ProjectOnPlane(grabPosition.forward, Vector3.up).normalized;
            itemToGrab.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
        }

    }

    void LockToCamera()
    {
        //player grabs it, position relative to camera
        Debug.Log("Inside LockToCamera");
        itemToGrab.transform.SetParent(grabPosition);
        itemToGrab.transform.localPosition = Vector3.zero;
        itemToGrab.transform.localRotation = Quaternion.identity;
        itemToGrab.GetComponent<Rigidbody>().isKinematic= true;
    }

    void UnlockFromCamera()
    {
        //put back on ground
        Debug.Log("Inside UnlockFromCamera");
        itemToGrab.transform.SetParent(null);
        itemToGrab.GetComponent<Rigidbody>().useGravity = true;
        itemToGrab.GetComponent<Rigidbody>().isKinematic= false;   
    }

    public void hoverin()
    {
        onObject = true;
    }

    public void hoverout()
    {
        onObject = false;
    }
}