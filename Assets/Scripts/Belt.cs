using UnityEngine;      // For functionality. 
using Unity.Entities;   // For ECS utilization.



// ##### COMPONENT SEGMENT #####

// Author Class
public class BeltAuthoring : MonoBehaviour
{
    
}

// Baker Class
public class BeltBaker : Baker<BeltAuthoring>
{
    
}

// Component Struct
public struct BeltComponent : IComponentData, IBuildingComponent
{
    
}



// ##### SYSTEM SEGMENT #####

// System Partial Struct
public partial struct BeltSystem : ISystem, IBuildingSystem
{
    
}
