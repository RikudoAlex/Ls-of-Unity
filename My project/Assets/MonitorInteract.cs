using UnityEngine;
using SojaExiles; // Required if you are using the hospital asset scripts

public class MonitorInteract : MonoBehaviour
{
    public GameObject tabletCanvas; 
    
    public MouseLook mouseLookScript; 
    private PlayerMovement playerMovementScript; 

    void Start()
    {
        playerMovementScript = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        bool isCtrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (isCtrlHeld && Input.GetKeyDown(KeyCode.T))
        {
            if (tabletCanvas != null && tabletCanvas.activeSelf) 
            {
                CloseTablet();
            }
            else 
            {
                TryOpenTablet();
            }
        }
    }

    void TryOpenTablet()
    {
        if (tabletCanvas != null) tabletCanvas.SetActive(true); 
        
        if (mouseLookScript != null) mouseLookScript.enabled = false;
        if (playerMovementScript != null) playerMovementScript.enabled = false;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseTablet()
    {
        if (tabletCanvas != null) tabletCanvas.SetActive(false);
        
        if (mouseLookScript != null) mouseLookScript.enabled = true;
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}