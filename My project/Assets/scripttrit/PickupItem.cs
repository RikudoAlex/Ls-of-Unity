using UnityEngine;
using TMPro; // <-- Added so the script can talk to UI text

public class PickupItem : MonoBehaviour
{
    [Header("Item Settings")]
    public string itemName = "Syringe";
    
    [Tooltip("The name of the object you must click on to use this item (e.g., 'charlie' or 'pole')")]
    public string targetColliderName = "charlie";

    [Header("Optional: Swap Object on Use")]
    [Tooltip("Place the duplicate 'hooked up' version here. It will enable when used.")]
    public GameObject hookedUpVersion;

    private bool isHeld = false;
    private TMP_Text hoverUIText; // <-- Added memory for the Hover Text

    // Variables to remember where the item started
    private Vector3 startPos;
    private Quaternion startRot;
    private Transform startParent;

    void Awake() 
    {
        startPos = transform.position;
        startRot = transform.rotation;
        startParent = transform.parent;

        // Find the HoverText UI automatically when the game starts
        GameObject uiObj = GameObject.Find("HoverText");
        if (uiObj != null)
        {
            hoverUIText = uiObj.GetComponent<TMP_Text>();
        }

        if (hookedUpVersion != null)
        {
            hookedUpVersion.SetActive(false);
        }
    }

    // --- NEW HOVER LOGIC ---
    void OnMouseEnter()
    {
        // If we are looking at the item, and we aren't already holding it, show its name!
        if (!isHeld && hoverUIText != null)
        {
            hoverUIText.text = itemName;
        }
    }

    void OnMouseExit()
    {
        // Clear the text when we look away
        if (!isHeld && hoverUIText != null)
        {
            hoverUIText.text = "";
        }
    }
    // -----------------------

    void OnMouseDown()
    {
        if (isHeld) return; 

        GameObject hand = GameObject.Find("HoldPoint");
        if (hand != null)
        {
            transform.SetParent(hand.transform);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            GetComponent<Collider>().enabled = false;
            isHeld = true;

            // Clear the hover text the moment we pick it up
            if (hoverUIText != null) hoverUIText.text = "";
        }
    }

    void Update()
    {
        if (isHeld)
        {
            if (Input.GetMouseButtonDown(1)) // Right-Click
            {
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    string hitName = hit.collider.gameObject.name.ToLower();
                    string targetNameLower = targetColliderName.ToLower();

                    if (hitName.Contains(targetNameLower)) 
                    {
                        Debug.Log("SUCCESS: Used " + itemName + " on " + targetColliderName + "!");
                        
                        ScenarioManager sm = FindObjectOfType<ScenarioManager>();
                        if (sm != null)
                        {
                            sm.TryTriggerOptionByID(itemName);
                        }
                        
                        if (hookedUpVersion != null)
                        {
                            hookedUpVersion.SetActive(true);
                        }
                        
                        gameObject.SetActive(false); 
                        isHeld = false;
                    }
                    else if (hitName.Contains("instrument_trolley"))
                    {
                        transform.SetParent(null); 
                        transform.position = hit.point + (Vector3.up * 0.05f); 
                        GetComponent<Collider>().enabled = true; 
                        isHeld = false;
                    }
                }
            }
        }
    }

    public void ResetToHome()
    {
        transform.SetParent(startParent);
        transform.position = startPos;
        transform.rotation = startRot;
        GetComponent<Collider>().enabled = true;
        isHeld = false;
        gameObject.SetActive(true); 

        if (hookedUpVersion != null)
        {
            hookedUpVersion.SetActive(false);
        }
    }
}