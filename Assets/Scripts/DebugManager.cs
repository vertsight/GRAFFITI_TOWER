using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public Level level;

    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape))
        {
            // level.Initialize();
        }
    }
}