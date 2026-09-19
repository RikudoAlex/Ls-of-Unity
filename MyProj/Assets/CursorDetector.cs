using UnityEngine;
using UnityEngine.InputSystem; // Required for the New Input System

public class CursorDetector : MonoBehaviour
{
    // ΤΟ ΚΑΤΑΦΕΡΑΜΕ ΑΦΟΤΟΥ ΕΠΡΕΠΕ ΕΓΩ ΝΑ ΚΑΘΟΔΗΓΩ ΤΟ ΤΖΕΜΙΝΑΙ ΔΕΚΑ ΩΡΕΣ
    [SerializeField] private GameObject targetObject;
// Serialize the camera so you can pick exactly which one shoots the ray
    [SerializeField] private Camera raycastCamera;
    public string objectState = "Null";

    void Update()
    {
        // Get the exact screen pixel coordinates using the New Input System
        Vector2 mousePixelPos = Mouse.current.position.ReadValue(); // ΔΕΣ ΠΟΥ ΒΡΙΣΚΕΣΑΙ
        
        Ray ray = raycastCamera.ScreenPointToRay(mousePixelPos); // ΔΕΙΞΕ ΠΡΟΣ ΤΑ ΠΟΥ
        // ΠΑΕΙ Η ΥΠΟΤΙΘΕΜΕΝΗ ΑΚΤΙΝΑ ΑΠΟ ΤΗΝ ΚΑΜΕΡΑ ΣΤΟ ΠΟΝΤΙΚΙ ΣΟΥ ΑΝΑ ΚΑΡΕ
        
        // This red line should now flawlessly follow your mouse in the Scene view
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
        
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Tells you exactly what the ray is touching
            Debug.Log("Ray is touching: " + hit.collider.gameObject.name);
            objectState = "Null"; // Reset the state to "Null" before checking for a hit
            if (hit.collider.gameObject == targetObject)
            {
                // Debug.Log("SUCCESS: Hit the monitor!");
                objectState = "Hit";
            }
        }
    }
}