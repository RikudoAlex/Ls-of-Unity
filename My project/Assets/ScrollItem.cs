using UnityEngine;

public class ScrollItem : MonoBehaviour
{
    public void DeleteItem()
    {
        // Destroys this specific item in the list
        Destroy(gameObject);
    }
}