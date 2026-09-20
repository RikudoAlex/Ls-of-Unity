using System;
using UnityEngine;
using TMPro;

public class TextDetection : MonoBehaviour
{
/*
    Το σκριπτάκι αυτό αποσκοπεί στην αναγνώριση κάποιας υποσυμβολοσειράς μέσα στο Treatment Plan. Τώρα το πως θα αλλάξετε την λειτουργικότητα είναι πάνω σας, καλή τύχη. Το GameObject που θα συνδέσετε είναι στην ιεραρχία το figmanuts > ICU Tablet > Page 1 > Patient.2 > treatment plan > Text Area > Text! 
*/
    [SerializeField] private GameObject textPanel; // Container of pragmatos
    private TextMeshProUGUI textMeshPro; // TextMeshProUGUI 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textMeshPro = textPanel.GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (textMeshPro.text.Length != 0) {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("a.b");

            // Η κανονική έκφραση ορίζει πως θέλουμε να αναγνωρίσουμε το 
            // a και b , ανεξαρτήτως του ΤΙ υπάρχει ενδιάμεσα.

            if (textMeshPro.text.Contains("οξυγόνο",System.StringComparison.OrdinalIgnoreCase) || textMeshPro.text.Contains("οξυγόνου",System.StringComparison.OrdinalIgnoreCase))
            {
                /*
                Εδώ αναγνωρίζονται οι λέξεις "οξυγόνο" και "οξυγόνου" ανεξαρτήτως πεζών-κεφαλαίων χαρακτήρων. Ευχαριστώ VS-CODE που μου προσυμπλήρωσες άδικα αυτό εδώ... 
                */
                Debug.Log("OXYGEN WORD FOUND");

                /* Εναλλακτικά , μπείτε εδώ https://learn.microsoft.com/en-us/dotnet/api/system.text.regularexpressions.regex.match?view=net-10.0
                */

                /*
                Εδώ μπορείτε να χρησιμοποιήσετε την Regex για πιο προχωρημένη αναγνώριση. Ας δείξουμε ένα παράδειγμα.
                */
            }
            else if(regex.Match(textMeshPro.text).Success)
            /* Στο σημείο αυτό λέμε "Κανονική έκφραση, θέλω να δείς αν υπάρχεις μέσα στο τεξτ αυτό ΣΤΗΝ ΠΡΩΤΗ ΓΡΑΜΜΗ. Αν υπάρχεις, αυτό σημαίνει ότι θα έχει μια θετική τιμή στο χαρακτηριστικό Success του αντικειμένου που επιστρέφει η σσυνάρτηση Match. Επομένως, μπαίνουμε εδώ ΜΌΝΟ αν έστω μια φορά κάνει MATCH η κανονική έκφραση" 
            */
            {
                Debug.Log("REGEX 1 FOUND");
                
            }
           else if(System.Text.RegularExpressions.Regex.Match(textMeshPro.text, "meow.*gatito", System.Text.RegularExpressions.RegexOptions.Singleline).Success)
            /* Στο σημείο αυτό λέμε "Κανονική έκφραση, θέλω να δείς αν υπάρχεις μέσα στο τεξτ αυτό. Αν υπάρχεις, αυτό σημαίνει ότι θα έχει μια θετική τιμή στο χαρακτηριστικό Success του αντικειμένου που επιστρέφει η σσυνάρτηση Match. Επομένως, μπαίνουμε εδώ ΜΌΝΟ αν έστω μια φορά κάνει MATCH η κανονική έκφραση". Εδώ απλά χρησιμοποιούμε το option Singleline ώστε να λαμβάνουμε κατά νου όλο το κείμενο σαν ένα single line (μια μοναδική γραμμή).
            */
            {
                Debug.Log("REGEX 2 FOUND");
                
            }
            else
            {
                 Debug.Log("WRITTEN");

            }
        }
        else{
            Debug.Log("EMPTY");
        }
    }
}
