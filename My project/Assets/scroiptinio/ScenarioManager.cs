using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 
using Newtonsoft.Json;

public class ScenarioManager : MonoBehaviour
{
    [Header("Scenario JSON File")]
    public TextAsset scenarioFile;

    [Header("Patient Info UI - Λίστες για όλες τις σελίδες")]
    [Tooltip("Προσθέστε εδώ όλα τα Text components για το Name από όλες τις σελίδες")]
    public List<TextMeshProUGUI> nameTextList;

    [Tooltip("Προσθέστε εδώ όλα τα Text components για το Surname από όλες τις σελίδες")]
    public List<TextMeshProUGUI> surnameTextList;

    [Tooltip("Προσθέστε εδώ όλα τα Text components για το SSN από όλες τις σελίδες")]
    public List<TextMeshProUGUI> ssnTextList;

    [Tooltip("Προσθέστε εδώ όλα τα Text components για το Admission Date από όλες τις σελίδες")]
    public List<TextMeshProUGUI> admissionDateTextList;

    [Tooltip("Προσθέστε εδώ όλα τα Text components για το Reason από όλες τις σελίδες")]
    public List<TextMeshProUGUI> reasonTextList;

    [Header("Vitals UI (Κάρτες 2ης Σελίδας)")]
    public TextMeshProUGUI hrText;
    public TextMeshProUGUI tempText;
    public TextMeshProUGUI rrText;
    public TextMeshProUGUI spo2Text;

    [Header("Live Fluctuation Settings")]
    public float updateInterval = 2.0f;

    private ScenarioData scenarioData;
    private Vitals baseVitals;
    
    private int currentHR;
    private float currentTemp;
    private int currentRR;
    private int currentSpO2;

    void Start()
    {
        LoadScenarioData();
        StartCoroutine(VitalsFluctuationLoop());
    }

    void LoadScenarioData()
    {
        if (scenarioFile == null)
        {
            Debug.LogError("Δεν έχει επιλεγεί JSON αρχείο στο ScenarioManager!");
            return;
        }

        scenarioData = JsonConvert.DeserializeObject<ScenarioData>(scenarioFile.text);
        
        // Γέμισμα των δημογραφικών σε όλες τις σελίδες
        PopulatePatientInfo();
        
        if (scenarioData != null && scenarioData.initial_state != null && scenarioData.initial_state.vitals != null)
        {
            baseVitals = scenarioData.initial_state.vitals;
            currentHR = baseVitals.hr;
            currentTemp = baseVitals.temp;
            currentRR = baseVitals.rr;
            currentSpO2 = baseVitals.spo2;
            
            UpdateVitalsDisplay();
        }
    }

    void PopulatePatientInfo()
    {
        if (scenarioData.patient_info == null) return;

        // Ενημέρωση όλων των Name Text components
        foreach (var textItem in nameTextList)
        {
            if (textItem != null) textItem.text = scenarioData.patient_info.name;
        }

        // Ενημέρωση όλων των Surname Text components
        foreach (var textItem in surnameTextList)
        {
            if (textItem != null) textItem.text = scenarioData.patient_info.surname;
        }

        // Ενημέρωση όλων των SSN Text components
        foreach (var textItem in ssnTextList)
        {
            if (textItem != null) textItem.text = scenarioData.patient_info.ssn;
        }

        // Ενημέρωση όλων των Admission Date Text components
        foreach (var textItem in admissionDateTextList)
        {
            if (textItem != null) textItem.text = scenarioData.patient_info.admission_date;
        }

        // Ενημέρωση όλων των Reason Text components
        foreach (var textItem in reasonTextList)
        {
            if (textItem != null) textItem.text = scenarioData.patient_info.reason;
        }
    }

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
        if (tempText) tempText.text = currentTemp.ToString("F1");
        if (rrText) rrText.text = currentRR.ToString();
        if (spo2Text) spo2Text.text = currentSpO2 + "%";
    }
}

// ================================
// DATA CLASSES FOR JSON PARSING
// ================================

[System.Serializable]
public class ScenarioData
{
    public PatientInfo patient_info;
    public InitialState initial_state;
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