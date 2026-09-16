using UnityEngine;
using SojaExiles; 

public class MonitorInteract : MonoBehaviour
{
    public GameObject loginPanel;        // Slot 1: Your login screen
    public GameObject mainProgramPanel;  // Slot 2: Your main program
    
    public MouseLook mouseLookScript; 
    private PlayerMovement playerMovementScript; 
    
    public float interactDistance = 300f; 

    void Start()
    {
        playerMovementScript = FindObjectOfType<PlayerMovement>();
    }

    void Update()
    {
        bool isCtrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (isCtrlHeld && Input.GetKeyDown(KeyCode.T))
        {
            // If EITHER screen is turned on, shut the tablet down
            if (loginPanel.activeSelf || mainProgramPanel.activeSelf)
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
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactDistance))
        {
            if (hit.collider.CompareTag("monitoshit"))
            {
                // Always start at the login screen
                loginPanel.SetActive(true);
                mainProgramPanel.SetActive(false);
                
                if (mouseLookScript != null) mouseLookScript.enabled = false;
                if (playerMovementScript != null) playerMovementScript.enabled = false;
                
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }

    // Changed to "public" so a UI Button can use it!
    public void CloseTablet()
    {
        // Turn both screens off
        loginPanel.SetActive(false);
        mainProgramPanel.SetActive(false);
        
        // Give movement and camera back
        if (mouseLookScript != null) mouseLookScript.enabled = true;
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}