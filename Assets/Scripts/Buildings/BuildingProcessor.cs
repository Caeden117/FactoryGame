using UnityEngine;          // For functionality. 



// The main processor that triggers all 
public class BuildingProcessor
{
    void Update()
    {
        Object[] buildings = Object.FindObjectsByType<AbstractBuilding>();
        foreach (Object building in buildings)
        {
            building.Act();
        }
    }
}
