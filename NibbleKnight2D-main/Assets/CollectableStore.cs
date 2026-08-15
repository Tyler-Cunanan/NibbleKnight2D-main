using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "CollectableStore", menuName = "AmountOfCheese")]
public class CollectableStore : ScriptableObject
{
    public int cheeseAmount = 0;

    // This will reset the number when the game starts or when the scriptable object is loaded
    private void OnEnable()
    {

    }

    // This will reset the number when the game stops or the object is unloaded
    private void OnDisable()
    {

    }
}
