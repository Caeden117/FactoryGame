using UnityEngine;      // For functionality. 
using Unity.Entities;   // For ECS utilization.



// ##### COMPONENT SEGMENT #####

// Author Class
public class CrafterAuthoring : MonoBehaviour, IBuildingComponent
{
    // Defining member variables.
    protected int[] acceptedResources = [];                 // Crafters are output-only; no inputs are accepted.
    protected int maxStackSize = processorStackSize;        // Non-belt building stack size assignment.
    protected int totalInventorySize = processorStackSize;  // 1 output slot = 1 * stack size.
}

// Baker Class
public class CrafterBaker : Baker<CrafterAuthoring>
{
    public override void Bake(CrafterAuthoring authoring)
    {
        // Assigning an ECS-created GameObject to "entity".
        var entity = GetEntity(authoring);
        // Making an instance of the CrafterComponent struct and assigning it to "entity".
        var crafter = new CrafterComponent
        {
            acceptedResources = authoring.acceptedResources,
            maxStackSize = authoring.processorStackSize,
            totalInventorySize = authoring.totalInventorySize
        };
        AddComponent(entity, crafter);
    }
}

// Component Struct
public struct CrafterComponent : IComponentData, IBuildingComponent
{
    protected int[] acceptedResources;  // Crafters are output-only; no inputs are accepted.
    protected int maxStackSize;         // Non-belt building stack size assignment.
    protected int totalInventorySize;   // 1 output slot = 1 * stack size.
}



// ##### SYSTEM SEGMENT #####

// System Partial Struct
public partial struct CrafterSystem : ISystem, IBuildingSystem
{
    public void OnUpdate(ref SystemState state)
    {
        // Collecting DeltaTime property from Entities.SystemAPI.Time.
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Selecting all enttiies w/ a CrafterComponent using read-write access.
        foreach (CrafterComponent crafter in SystemAPI.Query<RefRW<CrafterComponent>>())
        {
            // TO-DO: Fill contents here for what crafters should do when they update.
        }
    }
}
