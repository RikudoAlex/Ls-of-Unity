using UnityEngine;

public class CursorEventHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject meow;

    [SerializeField] private GameObject heartMenu;

    public GameObject loginPanel;        // Slot 1: Your login screen

    public GameObject loginMenu;        // Slot 1: Your login menu

    
    public GameObject currentPanel;        // Slot 2: Your current stats screen

    public GameObject currentMenu;        // Slot 2: Your current stats menu


    public GameObject currentGraphPanel;        // Slot 3: Your graph screen

    public GameObject currentGraphMenu;        // Slot 3: Your graph menu

    

    public GameObject treatmentPanel;        // Slot 4: Your treatment screen

    public GameObject treatmentMenu;        // Slot 4: Your treatment menu

    public GameObject scenarioPanel;        // Slot 5: Your scenario screen
    public GameObject multipageHolder;     // Slot 5Parent of a lotta pages
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            if (meow.GetComponent<CursorDetector>().objectState == "Hit" && !(heartMenu.activeInHierarchy))
            {
                Debug.Log("RIXTO");
                Debug.Log("STATE IS HIT");
                heartMenu.SetActive(true);

            }
            else if(loginPanel.activeInHierarchy && !(loginMenu.activeInHierarchy)){
                 Debug.Log("STATE IS TABLET LOGIN");
                 loginMenu.SetActive(true);
            }
            else if(currentPanel.activeInHierarchy && !(currentMenu.activeInHierarchy)){
                 Debug.Log("STATE IS CURRENT STATS");
                 currentMenu.SetActive(true);
            }
             else if(currentGraphPanel.activeInHierarchy && !(currentGraphMenu.activeInHierarchy)){
                 Debug.Log("STATE IS CURRENT GRAPH");
                 currentGraphMenu.SetActive(true);
            }

            else if(treatmentPanel.activeInHierarchy && !(treatmentMenu.activeInHierarchy)){
                 Debug.Log("STATE IS TREATMENT");
                 treatmentMenu.SetActive(true);
            }

            else if (scenarioPanel.activeInHierarchy)
            {
                if(!(multipageHolder.activeInHierarchy))
                {
                    multipageHolder.SetActive(true);
                    multipageHolder.transform.GetChild(0).gameObject.SetActive(true);
                }
                else if(multipageHolder.activeInHierarchy)
                {
                    if(multipageHolder.transform.GetChild(0).gameObject.activeInHierarchy)
                    {
                        multipageHolder.transform.GetChild(0).gameObject.SetActive(false);
                        multipageHolder.transform.GetChild(1).gameObject.SetActive(true);
                    }
                    else if(multipageHolder.transform.GetChild(1).gameObject.activeInHierarchy)
                    {
                        multipageHolder.transform.GetChild(1).gameObject.SetActive(false);
                        multipageHolder.transform.GetChild(2).gameObject.SetActive(true);
                        
                    }
                    else if (multipageHolder.transform.GetChild(2).gameObject.activeInHierarchy)
                    {
                       multipageHolder.transform.GetChild(2).gameObject.SetActive(false);
                        multipageHolder.SetActive(false);

                    }

                
            }
            }

            else
            {
                Debug.Log("STATE IS NOT HIT");
                heartMenu.SetActive(false);
                loginMenu.SetActive(false);
                currentMenu.SetActive(false);
                currentGraphMenu.SetActive(false);
                treatmentMenu.SetActive(false);
                multipageHolder.SetActive(false);
                multipageHolder.transform.GetChild(0).gameObject.SetActive(false);
                multipageHolder.transform.GetChild(1).gameObject.SetActive(false);

            }
            
        }
  
    }
}


