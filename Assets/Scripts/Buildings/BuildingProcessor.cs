using UnityEngine;          // For functionality. 



// The main processor that controls building updates.
public class BuildingProcessor : MonoBehaviour
{
    void Update()
    {
        Object[] buildings = Object.FindObjectsByType<AbstractBuilding>();
        foreach (Object building in buildings)
        {
            AbstractBuilding tempBuilding = (AbstractBuilding)building;
            tempBuilding.Act();
        }
    }
}
