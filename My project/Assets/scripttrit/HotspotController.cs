using UnityEngine;
using UnityEngine.EventSystems;

public class HotspotController : MonoBehaviour, IPointerClickHandler
{
    public enum HotspotType
    {
        Patient,
        Monitor,
        Ventilator,
        EHRTablet,
        EmergencyPhone
    }

    [Header("Hotspot Settings")]
    public HotspotType type;
    public string hotspotName;

    [Header("UI Panels to Toggle")]
    public GameObject targetUIPanel; // Π.χ. Το EHR Panel ή το Monitor Panel

    [Header("References")]
    public ScenarioManager scenarioManager;

    private void Start()
    {
        if (scenarioManager == null)
            scenarioManager = FindObjectOfType<ScenarioManager>();
    }

    // Καταγραφή κλικ είτε είναι 3D Object (με Collider) είτε UI Element
    public void OnPointerClick(PointerEventData eventData)
    {
        OnHotspotClicked();
    }

    private void OnMouseDown()
    {
        // Για 3D Colliders σε VR/Desktop
        OnHotspotClicked();
    }

    public void OnHotspotClicked()
    {
        Debug.Log($"Hotspot Clicked: {hotspotName} ({type})");

        // 1. Άνοιγμα του αντίστοιχου UI Panel αν υπάρχει
        if (targetUIPanel != null)
        {
            targetUIPanel.SetActive(!targetUIPanel.activeSelf);
        }

        // 2. Ενημέρωση του ScenarioManager για καταγραφή στο Log
        if (scenarioManager != null)
        {
            scenarioManager.ShowToast($"Αλληλεπίδραση με: {hotspotName}", Color.cyan);
        }
    }
}