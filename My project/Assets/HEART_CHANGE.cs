using UnityEngine;
using TMPro; 

public class HEART_CHANGE : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject alarm;
    private TextMeshProUGUI textMeshPro;

    void OnMouseOver()
    {
        //If your mouse hovers over the GameObject with the script attached, output this message
      //  Debug.Log("Mouse is over GameObject.");
    }

    void Start()
    {
       // textMeshPro = GetComponent<TextMeshProUGUI>();
       // textMeshPro.text = "72"; // Example heart rate value
    }

    // Update is called once per frame
    void Update()
    {
       // int a = Random.Range(-9200,118);
     //   if(a >= 0 && a <= 99)
     //   {
     //       textMeshPro.text = a.ToString() ; // Example heart rate value
      //      alarm.SetActive(false); 
     //   }
     //   if(a >= 100)
     //   {
     //       textMeshPro.text = a.ToString() ; // Example heart rate value
     //       alarm.SetActive(true); // Θέλουμε την φωτογραφία να εμφανίζεται ακόμη και αν είναι 
     //       // κλειστό το Heart Monitor, καθώς θα μας δείχνει ότι "όντως , υπάρχει θέμα".

        }

    }

