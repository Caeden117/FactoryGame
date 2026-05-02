using UnityEngine;      // For functionality. 
using Unity.Entities;   // For ECS utilization.



// ##### COMPONENT SEGMENT #####

// Author Class
public class FurnaceAuthoring : MonoBehaviour
{
    
}

// Baker Class
public class FurnaceBaker : Baker<FurnaceAuthoring>
{
    
}

// Component Struct
public struct FurnaceComponent : IComponentData, IBuildingComponent
{
    
}



// ##### SYSTEM SEGMENT #####

// System Partial Struct
public partial struct FurnaceSystem : ISystem, IBuildingSystem
{
    
}
