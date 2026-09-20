using UnityEngine;
using TMPro;

public class DynamicListManager : MonoBehaviour
{
    public TMP_InputField inputField;
    public Transform contentBox; // The Content object inside your Scroll View
    public GameObject itemPrefab; // The template we will create

    public void AddNewItem(string typedText)
    {
        // Prevent adding completely blank entries
        if (string.IsNullOrWhiteSpace(typedText)) return;

        // 1. Spawn a copy of the prefab directly inside the Content box
        GameObject newItem = Instantiate(itemPrefab, contentBox);

        // 2. Find the Text component inside the new item and set it to what you typed
        TMP_Text itemText = newItem.GetComponentInChildren<TMP_Text>();
        if (itemText != null)
        {
            itemText.text = typedText;
        }

        // 3. Clear the input box so it's ready for the next word
        inputField.text = "";
        
        // 4. Force the input box to stay selected so you can type rapidly
        inputField.ActivateInputField(); 
    }
}