using UnityEngine;      // For functionality. 
using Unity.Entities;   // For ECS utilization.



// ##### COMPONENT SEGMENT #####

// Author Class
public class CrafterAuthoring : MonoBehaviour
{
    
}

// Baker Class
public class CrafterBaker : Baker<CrafterAuthoring>
{
    
}

// Component Struct
public struct CrafterComponent : IComponentData, IBuildingComponent
{
    
}



// ##### SYSTEM SEGMENT #####

// System Partial Struct
public partial struct CrafterSystem : ISystem, IBuildingSystem
{
    
}
