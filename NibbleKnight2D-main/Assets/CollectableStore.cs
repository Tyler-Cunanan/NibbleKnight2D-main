using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectableStore", menuName = "AmountOfCheese")]
public class CollectableStore : ScriptableObject
{
    public int cheeseAmount = 0;

    public void AddCheese()
    {
        cheeseAmount++;
    }

    // This will reset the number when the game starts or when the scriptable object is loaded
    private void OnEnable()
    {
        ResetNumber();
    }

    // This will reset the number when the game stops or the object is unloaded
    private void OnDisable()
    {
        ResetNumber();
    }

    // Reset the number to its default value
    public void ResetNumber()
    {
        cheeseAmount = 0; // Set to whatever your default reset value should be
        Debug.Log("Number reset to: " + cheeseAmount);
    }
}
