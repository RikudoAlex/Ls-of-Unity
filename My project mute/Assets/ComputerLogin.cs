using UnityEngine;
using TMPro; 

public class ComputerLogin : MonoBehaviour
{
    public TMP_InputField idInputField;       // NEW: The top box for the Doctor ID
    public TMP_InputField passwordInputField; // The bottom box for the Password
    
    public GameObject loginPanel;             
    public GameObject mainProgramPanel;       
    
    public string correctID = "sata";       // Set your specific ID here
    public string correctPassword = "andagi";  // Set your specific password here

    // We no longer need the (string typedText) parameter. 
    // The script will just look at the boxes directly when the button is clicked.
    public void AttemptLogin()
    {
        // Get the text currently sitting in both boxes
        string typedID = idInputField.text;
        string typedPassword = passwordInputField.text;

        // Check if BOTH the ID and the Password match your secret words
        if (typedID.ToLower() == correctID.ToLower() && typedPassword.ToLower() == correctPassword.ToLower())
        {
            // SUCCESS
            loginPanel.SetActive(false);      
            mainProgramPanel.SetActive(true); 
            
            idInputField.text = ""; 
            passwordInputField.text = ""; 
        }
        else
        {
            // FAILED (Wrong ID or Password)
            // Clear both boxes so they can try again
            idInputField.text = ""; 
            passwordInputField.text = ""; 
        }
    }

    public void LogOut()
    {
        mainProgramPanel.SetActive(false); 
        loginPanel.SetActive(true);        
    }
}