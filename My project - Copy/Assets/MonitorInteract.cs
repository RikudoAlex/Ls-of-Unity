using UnityEngine;
using SojaExiles; // This allows this script to "see" your MouseLook script

public class MonitorInteract : MonoBehaviour
{
    public GameObject patientFilePanel; 
    public MouseLook mouseLookScript; // Drag your Camera (with MouseLook on it) here
    public float interactDistance = 3f; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            if (patientFilePanel.activeSelf)
            {
                ClosePanel();
            }
            else
            {
                TryOpenPanel();
            }
        }
    }

    void TryOpenPanel()
    {
        // This shoots the "laser" directly from your crosshair
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("monitoshit"))
            {
                patientFilePanel.SetActive(true);
                
                // 1. Disable the MouseLook script so the head stops spinning
                mouseLookScript.enabled = false;

                // 2. Unlock the cursor so you can click the UI
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    public void ClosePanel()
    {
        patientFilePanel.SetActive(false);
        
        // 1. Re-enable the MouseLook script
        mouseLookScript.enabled = true;

        // 2. Re-lock the cursor to the crosshair
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}