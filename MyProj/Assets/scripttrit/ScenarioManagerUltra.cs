using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using Newtonsoft.Json;

public class ScenarioManager : MonoBehaviour
{
    [Header("Scenario Playlist")]
    [Tooltip("Σύρετε εδώ όλα τα JSON σενάρια με τη σειρά που θέλετε να παίξουν")]
    public List<TextAsset> scenarioPlaylist;
    private int currentScenarioIndex = 0;

    [Header("Patient Info UI Lists")]
    public List<TextMeshProUGUI> nameTextList;
    public List<TextMeshProUGUI> surnameTextList;
    public List<TextMeshProUGUI> ssnTextList;
    public List<TextMeshProUGUI> admissionDateTextList;
    public List<TextMeshProUGUI> reasonTextList;

    [Header("Vitals UI Cards")]
    public TextMeshProUGUI hrText;
    public TextMeshProUGUI hrDisplay;
    public TextMeshProUGUI tempText;
    public TextMeshProUGUI rrText;
    public TextMeshProUGUI spo2Text;

    [Header("Scenario UI Page Controls")]
    public Button startFirstScenarioButton;     // Το πάνω κουμπί "Please select..."
    public TextMeshProUGUI scenarioMessageText; // Το "scenario text"
	[Header("Action Log")]
    public TextMeshProUGUI actionLogText;
    public Button option1Button;                // option 1
    public TextMeshProUGUI option1Text;
    public Button option2Button;                // option 2
    public TextMeshProUGUI option2Text;
    public Button nextButton;                   // Next Scenario
    public TextMeshProUGUI nextButtonText;     

    [Header("Timing Settings")]
    public float updateInterval = 2.0f;

    private ScenarioData scenarioData;
    private Vitals baseVitals;
    private Node currentNode;
    private int currentScore = 100;

    private int currentHR;
    private float currentTemp;
    private int currentRR;
    private int currentSpO2;

    private bool hasScenarioStarted = false;

    void Start()
    {
        // Αρχικά κρύβουμε τις επιλογές και το Next button
        if (option1Button) option1Button.gameObject.SetActive(false);
        if (option2Button) option2Button.gameObject.SetActive(false);
        if (nextButton) nextButton.gameObject.SetActive(false);
        if (startFirstScenarioButton) startFirstScenarioButton.gameObject.SetActive(true);

        if (scenarioPlaylist != null && scenarioPlaylist.Count > 0)
        {
            LoadCurrentScenario();
        }
        else
        {
            Debug.LogError("Η λίστα scenarioPlaylist είναι άδεια στο ScenarioManager!");
        }

        StartCoroutine(VitalsFluctuationLoop());
    }

    void LoadCurrentScenario()
    {
        if (scenarioPlaylist == null || scenarioPlaylist.Count == 0 || currentScenarioIndex >= scenarioPlaylist.Count) return;

        TextAsset currentJson = scenarioPlaylist[currentScenarioIndex];
        scenarioData = JsonConvert.DeserializeObject<ScenarioData>(currentJson.text);
        
        PopulatePatientInfo();
        
        if (scenarioData != null && scenarioData.initial_state != null && scenarioData.initial_state.vitals != null)
        {
            baseVitals = scenarioData.initial_state.vitals;
            currentScore = scenarioData.initial_state.current_score;

            currentHR = baseVitals.hr;
            currentTemp = baseVitals.temp;
            currentRR = baseVitals.rr;
            currentSpO2 = baseVitals.spo2;
            
            UpdateVitalsDisplay();
        }
    }

    // Καλείται ΑΜΕΣΩΣ όταν πατηθεί το κουμπί "Please select the first scenario..."
    public void StartFirstScenario()
    {
        hasScenarioStarted = true;
        if (startFirstScenarioButton) startFirstScenarioButton.gameObject.SetActive(false);

        if (scenarioData != null && scenarioData.nodes != null && scenarioData.nodes.Count > 0)
        {
            GoToNode("n1_start");
        }
    }

    void PopulatePatientInfo()
    {
        if (scenarioData == null || scenarioData.patient_info == null) return;

        foreach (var t in nameTextList) if (t) t.text = scenarioData.patient_info.name;
        foreach (var t in surnameTextList) if (t) t.text = scenarioData.patient_info.surname;
        foreach (var t in ssnTextList) if (t) t.text = scenarioData.patient_info.ssn;
        foreach (var t in admissionDateTextList) if (t) t.text = scenarioData.patient_info.admission_date;
        foreach (var t in reasonTextList) if (t) t.text = scenarioData.patient_info.reason;
    }

    // ==========================================
    // SCENARIO ENGINE & NODE SYSTEM
    // ==========================================

    void GoToNode(string nodeID)
    {
        foreach (var node in scenarioData.nodes)
        {
            if (node.id == nodeID)
            {
                currentNode = node;
                ShowCurrentNode();
                return;
            }
        }
        Debug.LogError("Δεν βρέθηκε Node με ID: " + nodeID);
    }

    void ShowCurrentNode()
    {
        if (currentNode == null) return;

        if (scenarioMessageText) scenarioMessageText.text = currentNode.text;

        if (option1Button) option1Button.gameObject.SetActive(false);
        if (option2Button) option2Button.gameObject.SetActive(false);
        if (nextButton) nextButton.gameObject.SetActive(false);

        if (currentNode.type == "decision" && currentNode.options != null)
        {
            if (currentNode.options.Count > 0 && option1Button)
            {
                option1Button.gameObject.SetActive(true);
                if (option1Text) option1Text.text = currentNode.options[0].label;
            }
            if (currentNode.options.Count > 1 && option2Button)
            {
                option2Button.gameObject.SetActive(true);
                if (option2Text) option2Text.text = currentNode.options[1].label;
            }
        }
        else if (currentNode.type == "message" || currentNode.type == "gate")
        {
            if (nextButton && !string.IsNullOrEmpty(currentNode.next_node_id))
            {
                nextButton.gameObject.SetActive(true);
                if (nextButtonText) nextButtonText.text = "Next";
            }
        }
        else if (currentNode.type == "end")
        {
            if (nextButton)
            {
                nextButton.gameObject.SetActive(true);
                if (nextButtonText) 
                {
                    nextButtonText.text = (currentScenarioIndex < scenarioPlaylist.Count - 1) ? "Next Scenario" : "End";
                }
            }
        }
    }

    public void OnOptionSelected(int optionIndex)
    {
        if (currentNode == null || currentNode.options == null || optionIndex >= currentNode.options.Count) return;

        Option selectedOption = currentNode.options[optionIndex];
        currentScore += selectedOption.score_delta;

        if (selectedOption.vitals_update != null)
        {
            if (selectedOption.vitals_update.hr > 0) baseVitals.hr = selectedOption.vitals_update.hr;
            if (selectedOption.vitals_update.spo2 > 0) baseVitals.spo2 = selectedOption.vitals_update.spo2;
            if (selectedOption.vitals_update.rr > 0) baseVitals.rr = selectedOption.vitals_update.rr;
            if (selectedOption.vitals_update.temp > 0) baseVitals.temp = selectedOption.vitals_update.temp;
        }
	// Append the chosen action to the log
    if (actionLogText != null)
    {
        actionLogText.text += "\n> " + selectedOption.label;
    }
        GoToNode(selectedOption.next_node_id);
    }

    public void OnNextPressed()
    {
        if (currentNode == null) return;

        if (currentNode.type == "end")
        {
            LoadNextScenarioInPlaylist();
        }
        else if (!string.IsNullOrEmpty(currentNode.next_node_id))
        {
            GoToNode(currentNode.next_node_id);
        }
    }

    void LoadNextScenarioInPlaylist()
    {
        currentScenarioIndex++;
        if (currentScenarioIndex < scenarioPlaylist.Count)
        {
            hasScenarioStarted = false;
            LoadCurrentScenario();
            StartFirstScenario(); // Ξεκινάει αμέσως το επόμενο σενάριο
        }
        else
        {
            if (scenarioMessageText) scenarioMessageText.text = "All scenarios completed!";
            if (nextButton) nextButton.gameObject.SetActive(false);
        }
    }
public void ResetCurrentScenario()
{
    // 1. Tell the system the scenario is restarting
    hasScenarioStarted = false;

    // 2. Re-read the JSON file to reset all vitals and scores to their starting values
    LoadCurrentScenario();

    // 3. Jump back to the very first text node
    StartFirstScenario();
}
public void RestartEntirePlaylist()
{
    // 1. Reset the playlist index back to the very first JSON file
    currentScenarioIndex = 0;
    hasScenarioStarted = false;

    // 2. Hide the active scenario buttons
    if (option1Button) option1Button.gameObject.SetActive(false);
    if (option2Button) option2Button.gameObject.SetActive(false);
    if (nextButton) nextButton.gameObject.SetActive(false);

    // 3. Show the main starting button again
    if (startFirstScenarioButton) startFirstScenarioButton.gameObject.SetActive(true);

    // 4. Clear the dialogue text
    if (scenarioMessageText) scenarioMessageText.text = "System Reset. Ready for new patient...";

    // 5. Reload the patient data and vitals for the first scenario
    if (scenarioPlaylist != null && scenarioPlaylist.Count > 0)
		
	if (actionLogText != null) actionLogText.text = "Log Initialized...\n";
    {
        LoadCurrentScenario();
    }
}
    // ==========================================
    // FLUCTUATION LOOP
    // ==========================================

    IEnumerator VitalsFluctuationLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(updateInterval);

            if (baseVitals != null)
            {
                currentHR = baseVitals.hr + Random.Range(-2, 3);
                currentTemp = baseVitals.temp + Random.Range(-1, 2) * 0.1f;
                currentRR = baseVitals.rr + Random.Range(-1, 2);
                currentSpO2 = Mathf.Min(100, baseVitals.spo2 + Random.Range(-1, 2));

                UpdateVitalsDisplay();
            }
        }
    }

    void UpdateVitalsDisplay()
    {
        if (hrText) hrText.text = currentHR.ToString();
        if (hrText) hrDisplay.text = currentHR.ToString();
        if (tempText) tempText.text = currentTemp.ToString("F1") + "°C";
        if (rrText) rrText.text = currentRR.ToString();
        if (spo2Text) spo2Text.text = currentSpO2 + "%";
    }
}

// ==========================================
// DATA CLASSES FOR FULL JSON STRUCTURE
// ==========================================

[System.Serializable]
public class ScenarioData
{
    public PatientInfo patient_info;
    public InitialState initial_state;
    public List<Node> nodes;
}

[System.Serializable]
public class PatientInfo
{
    public string name;
    public string surname;
    public string ssn;
    public string admission_date;
    public string reason;
}

[System.Serializable]
public class InitialState
{
    public int current_score;
    public Vitals vitals;
}

[System.Serializable]
public class Vitals
{
    public int hr;
    public float temp;
    public int rr;
    public int spo2;
}

[System.Serializable]
public class Node
{
    public string id;
    public string type;
    public string text;
    public string next_node_id;
    public List<Option> options;
}

[System.Serializable]
public class Option
{
    public string id;
    public string label;
    public string next_node_id;
    public int score_delta;
    public Vitals vitals_update;
}