using UnityEngine;
using UnityEngine.Video; // Don't forget this line!
/* Σε αυτό το σκριπτ, ξεικνάμε με το να δεχόμαστε ένα αντικείμενο-παιδί, το οποίο πατώντας E είτε το ενεργοποιούμε είτε όχι. Κάνοντας αυτό, συνεχίζουμε με το να θέτουμε σαν 
active και όχι active το αντικείμενο αυτό. Το child έμεινε ως ονομασία καθώς η πρώτη προσπάθεια είχε γίνει με την δοκιμή τοποθέτησης κάμερας-παιδί. 
*/
public class MonitorControl : MonoBehaviour
{
    //Use this for getting the toggle data
    [SerializeField] private GameObject child;
    void Start()
    {

    }

    void Update()
    {
        // Example: Press the 'E' key to turn on the monitor
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleMonitor();
            
        }
    }

    public void ToggleMonitor()
    {
        if(child.activeSelf)
        {
            // If the monitor is currently active, turn it off
            child.SetActive(false);
        }
        else
        {
            // If the monitor is currently inactive, turn it on
            child.SetActive(true);
        } 
    }
}
