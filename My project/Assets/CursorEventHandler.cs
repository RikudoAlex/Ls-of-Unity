using UnityEngine;

public class CursorEventHandler : MonoBehaviour
{
    [SerializeField] private GameObject meow;

    [SerializeField] private GameObject heartMenu;

    public GameObject loginPanel;       

    public GameObject loginMenu;        

    
    public GameObject currentPanel;        

    public GameObject currentMenu;       


    public GameObject currentGraphPanel;       

    public GameObject currentGraphMenu;       

    

    public GameObject treatmentPanel;      

    public GameObject treatmentMenu;       

    public GameObject scenarioPanel;      
    public GameObject multipageHolder;    
    void Start()
    {
        
    }

    
    void Update()
    {
		bool isCtrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
		
        if (isCtrlHeld && Input.GetKeyDown(KeyCode.H))
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


