using UnityEngine;      // For functionality. 
using Unity.Entities;   // For ECS utilization.



// ##### COMPONENT SEGMENT #####

// Author Class
public class SplitterAuthoring : MonoBehaviour, IBuildingComponent
{
    // Defining member variables.
    protected int[] acceptedResources = [];                 // Splitters are output-only; no inputs are accepted.
    protected int maxStackSize = processorStackSize;        // Non-belt building stack size assignment.
    protected int totalInventorySize = processorStackSize;  // 1 output slot = 1 * stack size.
}

// Baker Class
public class SplitterBaker : Baker<SplitterAuthoring>
{
    public override void Bake(SplitterAuthoring authoring)
    {
        // Assigning an ECS-created GameObject to "entity".
        var entity = GetEntity(authoring);
        // Making an instance of the SplitterComponent struct and assigning it to "entity".
        var splitter = new SplitterComponent
        {
            acceptedResources = authoring.acceptedResources,
            maxStackSize = authoring.processorStackSize,
            totalInventorySize = authoring.totalInventorySize
        };
        AddComponent(entity, splitter);
    }
}

// Component Struct
public struct SplitterComponent : IComponentData, IBuildingComponent
{
    protected int[] acceptedResources;  // Splitters are output-only; no inputs are accepted.
    protected int maxStackSize;         // Non-belt building stack size assignment.
    protected int totalInventorySize;   // 1 output slot = 1 * stack size.
}



// ##### SYSTEM SEGMENT #####

// System Partial Struct
public partial struct SplitterSystem : ISystem, IBuildingSystem
{
    public void OnUpdate(ref SystemState state)
    {
        // Collecting DeltaTime property from Entities.SystemAPI.Time.
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Selecting all enttiies w/ a SplitterComponent using read-write access.
        foreach (SplitterComponent splitter in SystemAPI.Query<RefRW<SplitterComponent>>())
        {
            // TO-DO: Fill contents here for what splitters should do when they update.
        }
    }
}
