using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;  // include the System.IO namespace


public class JSONToObject : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var txt = GetJSONText();
        Debug.Log(txt);
        /*
        var info = JSON2Object(txt);
        Debug.Log(info.attribA);
        */

        // Try to load a file
        var info_from_file = JSON2Object(File.ReadAllText("./Assets/test.json"));
        Debug.Log(info_from_file.attribA);
    }

    public string GetJSONText()
    {
        var json_txt=""; // Έστω ότι το json_txt είναι ένα άδειο string
        DataInfo info = new DataInfo(); // Κάθε αντικείμενο της C# έχει by default έναν
        // default κονστρακτορά ο οποίος δημιουργεί ένα αντικείμενο (object) μιας κλάσης.

        // Ανακαλώντας γνώσεις C++, ένα αντικείμενο είναι απλά ένα snapshot μιας κλάσης ,
        // δηλαδή μιας αφηρημένης περιγραφής.
        info.attribA = "fortnite";
        info.attribB = "skibidi";

        json_txt = JsonConvert.SerializeObject(info);
        // File.WriteAllText("./Assets/test.json",json_txt); // Εδώ θα γράψουμε serialization
        return json_txt;
    }

    public DataInfo JSON2Object(string json)
    {
        return JsonConvert.DeserializeObject<DataInfo>(json);
    }
}

[System.Serializable]
    public class DataInfo
    {
       // public string json;
       public string attribA;
       public string attribB;

    }
