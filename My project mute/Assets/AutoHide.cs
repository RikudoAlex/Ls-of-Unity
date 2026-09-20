using UnityEngine;
using UnityEngine.UIElements;

public class AutoHide : MonoBehaviour
{
    [SerializeField] private GameObject objectToPraise; // The GameObject to hide
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (objectToPraise.activeInHierarchy)
        {
            gameObject.GetComponent<UIDocument>().enabled=false; // Hide the GameObject if it's active
        }
        else
        {
            gameObject.GetComponent<UIDocument>().enabled=true; // Show the GameObject if it's not active
        }
    }
}
