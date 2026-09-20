using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#region Data Models

[Serializable]
public class ScenarioWrapper
{
    public string schema_version;
    public ScenarioMeta scenario_meta;
    public PatientInfo patient_info;
    public InitialState initial_state;
    public List<GlobalRule> global_rules;
    public List<Node> nodes;
}

[Serializable]
public class ScenarioMeta
{
    public string id;
    public string title;
    public string description;
    public int estimated_duration_minutes;
    public string difficulty;
}

[Serializable]
public class PatientInfo
{
    public string name;
    public string surname;
    public string ssn;
    public string admission_date;
    public string reason;
}

[Serializable]
public class InitialState
{
    public int current_score;
    public Vitals vitals;
}

[Serializable]
public class Vitals
{
    public int hr;
    public float temp;
    public int rr;
    public int spo2;
}

[Serializable]
public class GlobalRule
{
    public string id;
    public int condition_spo2_lt = -1;
    public int condition_hr_lt = -1;
    public int condition_hr_gt = -1;
    public string toast_message;
    public string monitor_color;
}

[Serializable]
public class Node
{
    public string id;
    public string type; 
    public string text;
    public string next_node_id;
    
    public int timeout_seconds = 0;
    public string timeout_next_node_id;
    public Vitals timeout_vitals_update;
    public int timeout_score_delta;
    public string timeout_toast;

   
    public string required_form_id;
    public string required_keyword; 
    public string feedback_blocked;
    public string feedback_success;

    public int score_delta;
    public List<Option> options;
}

[Serializable]
public class Option
{
    public string id;
    public string label;
    public int score_delta;
    public string toast;
    public Vitals vitals_update;
    public string next_node_id;
}

[Serializable]
public class LogEntry
{
    public string timestamp;
    public string nodeId;
    public string actionType;
    public string details;
    public int scoreAfterAction;
    public int hr;
    public float temp;
    public int rr;
    public int spo2;
}

#endregion

public class ScenarioManager : MonoBehaviour
{
    [Header("Login UI")]
    public GameObject loginPanel;
    public Button loginButton;
    public TMP_InputField usernameInput; 
    public TMP_InputField passwordInput; 

    [Tooltip("The required username and password to enter the simulation")]
	public string correctUsername = "sata"; 
	public string correctPassword = "andagi"; 

private bool isLoggedIn = false;

    [Header("JSON Files / Scenarios")]
    public TextAsset[] scenarioJsonFiles;
    public TMP_Dropdown scenarioDropdown;
	
    [Header("Default State")]
    public TextAsset restingScenarioFile;

    [Header("Start Screen UI")]
    [Tooltip("Το κεντρικό/γονικό Panel της αρχικής οθόνης που περιέχει το Dropdown και το Start Button")]
    public GameObject initialMenuPanel; 
    public GameObject startButton;
    [Tooltip("Επιπλέον GameObjects που θέλετε να κρύβονται στο Reset")]
    public List<GameObject> objectsToHideOnReset;
    [Tooltip("Επιπλέον GameObjects που θέλετε να εμφανίζονται στο Reset")]
    public List<GameObject> objectsToShowOnReset;

    [Header("UI Text Displays")]
    public TMP_Text scenarioTitleText;
    public TMP_Text scoreText;
    public TMP_Text timerText;

    [Header("Patient Info UI Lists")]
    public List<TextMeshProUGUI> nameTextList;
    public List<TextMeshProUGUI> surnameTextList;
    public List<TextMeshProUGUI> ssnTextList;
    public List<TextMeshProUGUI> admissionDateTextList;
    public List<TextMeshProUGUI> reasonTextList;

    [Header("EHR / Gate UI")]
    public TMP_InputField ehrInputField;

    [Header("Scenario Decision Panel UI")]
    public GameObject scenarioPanel;
    public TMP_Text scenarioMessageText;
    
    [Header("Action Log UI")]
    public TextMeshProUGUI actionLogText;

    public Button option1Button;
    public TMP_Text option1Text;
    public Button option2Button;
    public TMP_Text option2Text;
    public Button nextButton;
    public TMP_Text nextButtonText;

    [Header("Vitals Display UI")]
    public TMP_Text hrText;
    public TMP_Text tempText;
    public TMP_Text rrText;
    public TMP_Text spo2Text;
    public Image monitorStatusBorder;
    [SerializeField] public TextMeshProUGUI hrDisplay;
	
    [Header("Notifications UI")]
    public GameObject toastPanel;
    public TMP_Text toastText;

    [Header("Debriefing UI")]
    public GameObject debriefPanel;
    public TMP_Text debriefReportText;
	
    [Header("Reset Buttons")]
    public GameObject resetButton;
    public GameObject resetAllButton;

    
    private ScenarioWrapper currentScenario;
    private Dictionary<string, Node> nodeMap = new Dictionary<string, Node>();
    private Node currentNode;
    
    private int currentScore;
    private Vitals currentVitals;
    private bool isGateSatisfied = false;
    
    private Coroutine timerCoroutine;
    private Coroutine vitalsFluctuationCoroutine;
    private List<LogEntry> actionLogs = new List<LogEntry>();
	
    private void Start()
    {
        InitializeDropdown();

        if (ehrInputField != null)
        {
            ehrInputField.onValueChanged.AddListener(OnEHRInputChanged);
        }

        if (loginButton != null)
        {
            loginButton.onClick.RemoveAllListeners();
            loginButton.onClick.AddListener(OnLoginButtonClicked);
        }

        isLoggedIn = false;

        
        if (restingScenarioFile != null)
        {
            LoadScenarioFromJSON(restingScenarioFile.text);
        }
        else
        {
            ResetToInitialState(); 
        }

        
        if (loginPanel != null) loginPanel.SetActive(true);
        if (scenarioPanel != null) scenarioPanel.SetActive(false);
        if (initialMenuPanel != null) initialMenuPanel.SetActive(false);
    }

    public void OnLoginButtonClicked()
{
    if (usernameInput != null && passwordInput != null)
    {
        if (usernameInput.text != correctUsername || passwordInput.text != correctPassword)
        {
            ShowToast("Λάθος Όνομα Χρήστη ή Κωδικός", Color.red);
            return; 
        }
    }

    isLoggedIn = true;
    
    if (usernameInput != null) usernameInput.text = "";
    if (passwordInput != null) passwordInput.text = "";
    
    if (loginPanel != null) loginPanel.SetActive(false);
    
    if (restingScenarioFile != null)
    {
        if (scenarioPanel != null) scenarioPanel.SetActive(true);
    }
    else
    {
        if (initialMenuPanel != null) initialMenuPanel.SetActive(true);
    }
}

    private void InitializeDropdown()
    {
        if (scenarioDropdown == null) return;
        
        scenarioDropdown.ClearOptions();
        List<string> options = new List<string>();
        foreach (var file in scenarioJsonFiles)
        {
            if (file != null)
                options.Add(file.name);
        }
        scenarioDropdown.AddOptions(options);
    }

    public void OnStartButtonClicked()
    {
        if (scenarioDropdown == null || scenarioJsonFiles == null || scenarioJsonFiles.Length == 0) return;

        int selectedIndex = scenarioDropdown.value;
        if (selectedIndex >= 0 && selectedIndex < scenarioJsonFiles.Length)
        {
            LoadScenarioByIndex(selectedIndex);
        }
    }

    public void LoadScenarioByIndex(int index)
    {
        if (index < 0 || index >= scenarioJsonFiles.Length) return;
        LoadScenarioFromJSON(scenarioJsonFiles[index].text);
    }

    public void LoadScenarioFromJSON(string jsonContent)
    {
        currentScenario = JsonUtility.FromJson<ScenarioWrapper>(jsonContent);
        
        nodeMap.Clear();
        foreach (var node in currentScenario.nodes)
        {
            nodeMap[node.id] = node;
        }

        currentScore = currentScenario.initial_state.current_score;
        currentVitals = currentScenario.initial_state.vitals;
        actionLogs.Clear();
        
        if (actionLogText != null) actionLogText.text = "";
       
        if (initialMenuPanel != null) initialMenuPanel.SetActive(false);
        if (scenarioDropdown != null) scenarioDropdown.gameObject.SetActive(false);
        if (startButton != null) startButton.SetActive(false);
      
        if (scenarioPanel != null) scenarioPanel.SetActive(true);
        if (debriefPanel != null) debriefPanel.SetActive(false);
        
        bool isResting = (currentScenario != null && currentScenario.scenario_meta.id == "scenario_resting");
        if (resetButton != null) resetButton.SetActive(!isResting);
        if (resetAllButton != null) resetAllButton.SetActive(!isResting);

        SetMonitorColor("none");
        if (timerText != null) timerText.text = "";
        if (ehrInputField != null) ehrInputField.text = "";

        if (scenarioTitleText != null && currentScenario.scenario_meta != null) 
            scenarioTitleText.text = currentScenario.scenario_meta.title;

        if (currentScenario.patient_info != null)
        {
            if (nameTextList != null)
                foreach (var txt in nameTextList) if (txt != null) txt.text = currentScenario.patient_info.name;

            if (surnameTextList != null)
                foreach (var txt in surnameTextList) if (txt != null) txt.text = currentScenario.patient_info.surname;

            if (ssnTextList != null)
                foreach (var txt in ssnTextList) if (txt != null) txt.text = currentScenario.patient_info.ssn;

            if (admissionDateTextList != null)
                foreach (var txt in admissionDateTextList) if (txt != null) txt.text = currentScenario.patient_info.admission_date;

            if (reasonTextList != null)
                foreach (var txt in reasonTextList) if (txt != null) txt.text = currentScenario.patient_info.reason;
        }

        LogAction("SYSTEM", "Scenario Loaded: " + currentScenario.scenario_meta.title);
        
        UpdateUI();

        if (vitalsFluctuationCoroutine != null) StopCoroutine(vitalsFluctuationCoroutine);
        vitalsFluctuationCoroutine = StartCoroutine(FluctuateVitalsRoutine());
        
        if (currentScenario.nodes.Count > 0)
        {
            SetCurrentNode(currentScenario.nodes[0]);
        }
    }

    public void ResetToInitialState()
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        if (vitalsFluctuationCoroutine != null) StopCoroutine(vitalsFluctuationCoroutine);

        currentScenario = null;
        currentNode = null;
        nodeMap.Clear();
        actionLogs.Clear();

        if (scenarioPanel != null) scenarioPanel.SetActive(false);
        if (debriefPanel != null) debriefPanel.SetActive(false);
        if (toastPanel != null) toastPanel.SetActive(false);
        if (option1Button != null) option1Button.gameObject.SetActive(false);
        if (option2Button != null) option2Button.gameObject.SetActive(false);
        if (nextButton != null) nextButton.gameObject.SetActive(false);
        
        if (resetButton != null) resetButton.SetActive(false);
        if (resetAllButton != null) resetAllButton.SetActive(false);

        if (objectsToHideOnReset != null)
        {
            foreach (var go in objectsToHideOnReset)
            {
                if (go != null) go.SetActive(false);
            }
        }

        if (initialMenuPanel != null) initialMenuPanel.SetActive(true);
        if (scenarioDropdown != null) scenarioDropdown.gameObject.SetActive(true);
        if (startButton != null) startButton.SetActive(true);

        if (objectsToShowOnReset != null)
        {
            foreach (var go in objectsToShowOnReset)
            {
                if (go != null) go.SetActive(true);
            }
        }

        if (scenarioTitleText != null) scenarioTitleText.text = "";
        if (scoreText != null) scoreText.text = "";
        if (timerText != null) timerText.text = "";
        if (hrText != null) hrText.text = "--";
        if (tempText != null) tempText.text = "--";
        if (rrText != null) rrText.text = "--";
        if (spo2Text != null) spo2Text.text = "--";
        if (ehrInputField != null) ehrInputField.text = "";
        if (actionLogText != null) actionLogText.text = "";

        ClearTextList(nameTextList);
        ClearTextList(surnameTextList);
        ClearTextList(ssnTextList);
        ClearTextList(admissionDateTextList);
        ClearTextList(reasonTextList);

        SetMonitorColor("none");
        ResetAllItems();
    }

    public void RecallAll()
    {
        ResetToInitialState();
    }

    public void ResetAllItems()
    {
        PickupItem[] allItems = Resources.FindObjectsOfTypeAll<PickupItem>();
        foreach (PickupItem item in allItems)
        {
            if (item.gameObject.scene.isLoaded)
            {
                item.ResetToHome();
            }
        }
    }

    private void ClearTextList(List<TextMeshProUGUI> list)
    {
        if (list == null) return;
        foreach (var txt in list)
        {
            if (txt != null) txt.text = "";
        }
    }

    private void SetCurrentNode(Node node)
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);
        
        currentNode = node;
        isGateSatisfied = false;

        if (currentNode.timeout_seconds <= 0 && timerText != null)
        {
            timerText.text = "";
        }

        if (scenarioMessageText != null)
            scenarioMessageText.text = currentNode.text;

        LogAction("NODE_ENTER", "Entered Node: " + currentNode.id);

        EvaluateGlobalRules();

        switch (currentNode.type)
        {
            case "message":
                SetupMessageNode();
                break;

            case "decision":
                SetupDecisionNode();
                break;

            case "gate":
                SetupGateNode();
                break;

            case "end":
                SetupEndNode();
                break;
        }

        UpdateUI();
    }

    #region Node Handlers

    private void SetupMessageNode()
    {
        bool isResting = (currentScenario != null && currentScenario.scenario_meta.id == "scenario_resting");
        bool hasNextNode = !string.IsNullOrEmpty(currentNode.next_node_id);
        
        SetButtonsState(showNext: hasNextNode || isResting, showOptions: false);
        
        if (nextButtonText != null) 
            nextButtonText.text = isResting ? "Επιλογή Σεναρίου" : "Next";

        if (nextButton != null)
        {
            nextButton.interactable = true;
            nextButton.onClick.RemoveAllListeners();
            
            if (isResting)
            {
                nextButton.onClick.AddListener(OpenScenarioMenu);
            }
            else if (hasNextNode)
            {
                nextButton.onClick.AddListener(() => {
                    if (nodeMap.ContainsKey(currentNode.next_node_id))
                        SetCurrentNode(nodeMap[currentNode.next_node_id]);
                });
            }
        }
    }

    private void SetupDecisionNode()
    {
        SetButtonsState(showNext: false, showOptions: true);

        if (currentNode.options != null && currentNode.options.Count > 0)
        {
            if (option1Button != null)
            {
                option1Button.gameObject.SetActive(true);
                if (option1Text != null) option1Text.text = currentNode.options[0].label;
                
                Option opt1 = currentNode.options[0];
                option1Button.onClick.RemoveAllListeners();
                option1Button.onClick.AddListener(() => OnUIOptionClicked(opt1));
            }
        }
        else if (option1Button != null)
        {
            option1Button.gameObject.SetActive(false);
        }

        if (currentNode.options != null && currentNode.options.Count > 1)
        {
            if (option2Button != null)
            {
                option2Button.gameObject.SetActive(true);
                if (option2Text != null) option2Text.text = currentNode.options[1].label;
                
                Option opt2 = currentNode.options[1];
                option2Button.onClick.RemoveAllListeners();
                option2Button.onClick.AddListener(() => OnUIOptionClicked(opt2));
            }
        }
        else if (option2Button != null)
        {
            option2Button.gameObject.SetActive(false);
        }

        if (currentNode.timeout_seconds > 0)
        {
            timerCoroutine = StartCoroutine(NodeTimeoutTimer(currentNode.timeout_seconds));
        }
    }

    private void SetupGateNode()
    {
        SetButtonsState(showNext: true, showOptions: false);
        if (nextButtonText != null) nextButtonText.text = "Next";

        ValidateGateInput(ehrInputField != null ? ehrInputField.text : "");

        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() => {
                if (isGateSatisfied && !string.IsNullOrEmpty(currentNode.next_node_id) && nodeMap.ContainsKey(currentNode.next_node_id))
                {
                    SetCurrentNode(nodeMap[currentNode.next_node_id]);
                }
            });
        }
    }

    private void SetupEndNode()
    {
        SetButtonsState(showNext: true, showOptions: false);
        
        if (nextButtonText != null) 
            nextButtonText.text = "End of Scenario";

        if (nextButton != null)
        {
            nextButton.interactable = true;
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(() => 
            {
                if (currentScenario != null && currentScenario.scenario_meta.id == "scenario_resting")
                {
                    OpenScenarioMenu();
                }
                else
                {
                    LoadRestingScenario();
                }
            });
        }

        ShowToast("Το σενάριο ολοκληρώθηκε!", Color.green);
        ShowDebriefingScreen();
    }

    private void SetButtonsState(bool showNext, bool showOptions)
    {
        if (nextButton != null) nextButton.gameObject.SetActive(showNext);
        if (option1Button != null) option1Button.gameObject.SetActive(showOptions);
        if (option2Button != null) option2Button.gameObject.SetActive(showOptions);
    }

    #endregion

    #region Option & Gate Callbacks

    private void OnUIOptionClicked(Option option)
    {
        PickupItem[] allItems = Resources.FindObjectsOfTypeAll<PickupItem>();
        bool physicalItemExists = false;

        foreach (PickupItem item in allItems)
        {
            if (item.gameObject.scene.isLoaded && item.itemName == option.id)
            {
                physicalItemExists = true;
                break;
            }
        }

        if (physicalItemExists)
        {
            ShowToast("Πρέπει να χορηγήσετε την αγωγή πριν συνεχίσετε", new Color32(255, 140, 0, 255));
            return; 
        }

        OnOptionSelected(option);
    }

    private void OnOptionSelected(Option option)
    {
        if (timerCoroutine != null) StopCoroutine(timerCoroutine);

        currentScore += option.score_delta;
        currentScore = Mathf.Clamp(currentScore, 0, 100);
        
        if (option.vitals_update != null && option.vitals_update.hr > 0)
        {
            ApplyVitalsUpdate(option.vitals_update);
        }

        if (!string.IsNullOrEmpty(option.toast))
        {
            ShowToast(option.toast, Color.cyan);
        }

        LogAction("DECISION", "Selected: " + option.label);

        UpdateUI();

        if (!string.IsNullOrEmpty(option.next_node_id) && nodeMap.ContainsKey(option.next_node_id))
        {
            SetCurrentNode(nodeMap[option.next_node_id]);
        }
    }

    private void OnEHRInputChanged(string text)
    {
        if (currentNode != null && currentNode.type == "gate")
        {
            ValidateGateInput(text);
        }
    }

    private void ValidateGateInput(string text)
    {
        if (currentNode == null || currentNode.type != "gate") return;

        string normalizedInput = NormalizeGreekText(text);
        string normalizedKeyword = NormalizeGreekText(currentNode.required_keyword);

        bool hasValidKeyword = string.IsNullOrEmpty(normalizedKeyword) 
            ? !string.IsNullOrWhiteSpace(text)
            : normalizedInput.Contains(normalizedKeyword);

        if (hasValidKeyword)
        {
            if (!isGateSatisfied)
            {
                isGateSatisfied = true;
                currentScore += currentNode.score_delta;
                currentScore = Mathf.Clamp(currentScore, 0, 100);
                
                ShowToast(string.IsNullOrEmpty(currentNode.feedback_success) ? "Έγκυρη καταγραφή!" : currentNode.feedback_success, Color.green);
                LogAction("GATE_COMPLETED", "EHR Entry Validated");
                UpdateUI();
            }

            if (nextButton != null) nextButton.interactable = true;
        }
        else
        {
            isGateSatisfied = false;
            if (nextButton != null) nextButton.interactable = false;

            if (!string.IsNullOrWhiteSpace(text))
            {
                ShowToast(string.IsNullOrEmpty(currentNode.feedback_blocked) ? "Συμπληρώστε σωστά το EHR." : currentNode.feedback_blocked, Color.orange);
            }
        }
    }

    private string NormalizeGreekText(string input)
    {
        if (string.IsNullOrEmpty(input)) return "";

        string text = input.ToLowerInvariant();
        text = text.Replace('ά', 'α').Replace('έ', 'ε').Replace('ή', 'η')
                   .Replace('ί', 'ι').Replace('ό', 'ο').Replace('ύ', 'υ')
                   .Replace('ώ', 'ω').Replace('ϊ', 'ι').Replace('ϋ', 'υ')
                   .Replace('ΐ', 'ι').Replace('ΰ', 'υ');

        return text;
    }

    #endregion

    #region Timeout & Rules Mechanics

    private IEnumerator NodeTimeoutTimer(int seconds)
    {
        int timeRemaining = seconds;
        while (timeRemaining > 0)
        {
            if (timerText != null) timerText.text = $"Χρόνος: {timeRemaining}s";
            yield return new WaitForSeconds(1.0f);
            timeRemaining--;
        }

        if (timerText != null) timerText.text = "Χρόνος: 0s";

        currentScore += currentNode.timeout_score_delta;
        currentScore = Mathf.Clamp(currentScore, 0, 100);
        if (currentNode.timeout_vitals_update != null && currentNode.timeout_vitals_update.hr > 0)
        {
            ApplyVitalsUpdate(currentNode.timeout_vitals_update);
        }

        ShowToast(currentNode.timeout_toast, Color.red);
        LogAction("TIMEOUT", $"Timeout expired at node: {currentNode.id}");
        
        UpdateUI();

        if (!string.IsNullOrEmpty(currentNode.timeout_next_node_id) && nodeMap.ContainsKey(currentNode.timeout_next_node_id))
        {
            SetCurrentNode(nodeMap[currentNode.timeout_next_node_id]);
        }
    }

    private IEnumerator FluctuateVitalsRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f);

            if (currentVitals != null)
            {
                int hrOffset = UnityEngine.Random.Range(-2, 3);
                int spo2Offset = UnityEngine.Random.Range(-1, 2);
                int rrOffset = UnityEngine.Random.Range(-1, 2);

                int displayHR = Mathf.Clamp(currentVitals.hr + hrOffset, 40, 180);
                int displaySpO2 = Mathf.Clamp(currentVitals.spo2 + spo2Offset, 70, 100);
                int displayRR = Mathf.Clamp(currentVitals.rr + rrOffset, 6, 40);

                if (hrText != null) hrText.text = $"{displayHR} bpm";
				if (hrText != null) hrDisplay.text = $"{displayHR}";
                if (spo2Text != null) spo2Text.text = $"{displaySpO2} %";
                if (rrText != null) rrText.text = $"{displayRR} /min";
                if (tempText != null) tempText.text = $"{currentVitals.temp:F1} °C";
            }
        }
    }

    private void EvaluateGlobalRules()
    {
        if (currentScenario == null || currentScenario.global_rules == null) return;

        foreach (var rule in currentScenario.global_rules)
        {
            bool triggered = false;

            if (rule.condition_spo2_lt > 0 && currentVitals.spo2 < rule.condition_spo2_lt) 
                triggered = true;
            if (rule.condition_hr_lt > 0 && currentVitals.hr < rule.condition_hr_lt) 
                triggered = true;
            if (rule.condition_hr_gt > 0 && currentVitals.hr > rule.condition_hr_gt) 
                triggered = true;

            if (triggered)
            {
                ShowToast(rule.toast_message, Color.red);
                SetMonitorColor(rule.monitor_color);
            }
        }
    }

    #endregion

    #region Helper & UI Methods

    private void ApplyVitalsUpdate(Vitals newVitals)
    {
        if (newVitals.hr > 0) currentVitals.hr = newVitals.hr;
        if (newVitals.temp > 0) currentVitals.temp = newVitals.temp;
        if (newVitals.rr > 0) currentVitals.rr = newVitals.rr;
        if (newVitals.spo2 > 0) currentVitals.spo2 = newVitals.spo2;
    }

    private void UpdateUI()
    {
        if (currentVitals == null) return;

        if (scoreText != null) scoreText.text = $"Score: {currentScore}";
        
        if (hrText != null) hrText.text = $"{currentVitals.hr} bpm";
        if (hrDisplay != null) hrDisplay.text = $"{currentVitals.hr}";
        if (tempText != null) tempText.text = $"{currentVitals.temp:F1} °C";
        if (rrText != null) rrText.text = $"{currentVitals.rr} /min";
        if (spo2Text != null) spo2Text.text = $"{currentVitals.spo2} %";
    }

    private void SetMonitorColor(string colorName)
    {
        if (monitorStatusBorder == null) return;

        switch (colorName.ToLower())
        {
            case "red":
                monitorStatusBorder.color = Color.red;
                break;
            case "yellow":
                monitorStatusBorder.color = Color.yellow;
                break;
            default:
                monitorStatusBorder.color = new Color32(235, 235, 235, 255);
                break;
        }
    }

    public void ShowToast(string message, Color bgColor)
    {
        if (toastPanel == null || toastText == null) return;
        
        toastText.text = message;
        toastPanel.GetComponent<Image>().color = bgColor;
        toastPanel.SetActive(true);
        
        CancelInvoke(nameof(HideToast));
        Invoke(nameof(HideToast), 3.5f);
    }

    private void HideToast()
    {
        if (toastPanel != null) toastPanel.SetActive(false);
    }

    private void LogAction(string actionType, string details)
{
    string time = DateTime.Now.ToString("HH:mm:ss");

    actionLogs.Add(new LogEntry
    {
        timestamp = time,
        nodeId = currentNode != null ? currentNode.id : "START",
        actionType = actionType,
        details = details,
        scoreAfterAction = currentScore,
        hr = currentVitals != null ? currentVitals.hr : 0,
        temp = currentVitals != null ? currentVitals.temp : 0f,
        rr = currentVitals != null ? currentVitals.rr : 0,
        spo2 = currentVitals != null ? currentVitals.spo2 : 0
    });

    if (actionLogText != null)
    {
        actionLogText.text += $"[{time}] {actionType}: {details}\n";
    }
}

    #endregion

    #region Debrief & Export

    private void ShowDebriefingScreen()
    {
        if (vitalsFluctuationCoroutine != null) StopCoroutine(vitalsFluctuationCoroutine);
        if (debriefPanel == null) return;

        debriefPanel.SetActive(true);
    
        if (resetButton != null) resetButton.SetActive(false);
        if (resetAllButton != null) resetAllButton.SetActive(false);
        
        string report = $"<b>ΑΠΟΛΟΓΙΣΜΟΣ ΣΕΝΑΡΙΟΥ</b>\n";
        report += $"Σενάριο: {currentScenario.scenario_meta.title}\n";
        report += $"Τελικό Score: <b>{currentScore} / 100</b>\n\n";

        if (debriefReportText != null) debriefReportText.text = report;
        ExportLogsToCSV();
    }

    public void ExportLogsToCSV()
{
    if (currentScenario == null) return;

    string fileName = $"ScenarioLog_{currentScenario.scenario_meta.id}_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
    string path = Path.Combine(Application.persistentDataPath, fileName);

    using (StreamWriter writer = new StreamWriter(path))
    {
        writer.WriteLine("Timestamp;NodeID;ActionType;Details;ScoreAfterAction;HR;Temp;RR;SpO2");
        foreach (var log in actionLogs)
        {
            writer.WriteLine($"{log.timestamp};{log.nodeId};{log.actionType};\"{log.details}\";{log.scoreAfterAction};{log.hr};{log.temp:F1};{log.rr};{log.spo2}");
        }
    }

    Debug.Log($"[CSV Export] Log saved to: {path}");
}

    public void OnRestartOrNextScenarioClicked()
    {
        if (currentScenario != null && currentScenario.scenario_meta.id == "scenario_resting")
        {
            OpenScenarioMenu();
        }
        else
        {
            LoadRestingScenario();
        }
    }

    public void TryTriggerOptionByID(string targetOptionID)
    {
        if (currentNode != null && currentNode.type == "decision" && currentNode.options != null)
        {
            foreach (Option opt in currentNode.options)
            {
                if (opt.id == targetOptionID)
                {
                    Debug.Log("3D Item successfully triggered JSON option: " + opt.label);
                    OnOptionSelected(opt);
                    return;
                }
            }
        }
        
        ShowToast("Αυτό το αντικείμενο δεν απαιτείται αυτή τη στιγμή.", Color.orange);
    }

    public void LoadRestingScenario()
    {
        if (restingScenarioFile != null)
        {
            LoadScenarioFromJSON(restingScenarioFile.text);
            ResetAllItems();
        }
        else
        {
            ResetToInitialState(); 
        }
    }

    public void OpenScenarioMenu()
    {
        if (scenarioPanel != null) scenarioPanel.SetActive(false);
        if (debriefPanel != null) debriefPanel.SetActive(false);
        if (toastPanel != null) toastPanel.SetActive(false);
        
        if (initialMenuPanel != null) initialMenuPanel.SetActive(true);
        if (scenarioDropdown != null) scenarioDropdown.gameObject.SetActive(true);
        if (startButton != null) startButton.SetActive(true);
    }

    #endregion
}
