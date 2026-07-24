using UnityEngine;

[CreateAssetMenu(fileName = "NewGameTask", menuName = "System/Game Task")]
public class GameTask : ScriptableObject
{
    public string taskName;
    [TextArea]
    public string description;
    
    // This is the core variable we will check
    public bool isCompleted = false;

    // Optional: A quick way to reset tasks when the game starts/quits
    private void OnEnable()
    {
        isCompleted = false; 
    }
}