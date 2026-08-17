using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;  // include the System.IO namespace
using TMPro; 


public class JSONUseObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private GameObject tbc; // TO be changed...
     private TextMeshProUGUI textMeshPro;
    void Start()
    {
        /*
        var info = JSON2Object(txt);
        Debug.Log(info.attribA);
        */

        // Try to load a file
        var info_from_file = JSON2Object(File.ReadAllText("./Assets/test.json"));
        textMeshPro = GetComponent<TextMeshProUGUI>();
        textMeshPro.text = info_from_file.attribA;
    }



    public Note JSON2Object(string json)
    {
        return JsonConvert.DeserializeObject<Note>(json);
    }
}

[System.Serializable]
    public class Note
    {
       // public string json;
       public string attribA;

    }
