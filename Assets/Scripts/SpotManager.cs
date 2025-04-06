using System.Collections.Generic;
using UnityEngine;

public class SpotManager : MonoBehaviour
{
    public static SpotManager Instance;
    public List<Spot> usedSpots;
    public GameObject spot;

    private void Awake()
    {
        Instance = this;
    }
    public GameObject SpawnSpot()
    {
        if(usedSpots.Count > 0)
        {
            GameObject i = usedSpots[0].gameObject;
            i.SetActive(true);
            usedSpots.RemoveAt(0);
            return i;
        }
        else
        {
            return Instantiate(spot);
        }
    }
}