using UnityEngine;      // For functionality. 
using Unity.Entities;   // For ECS utilization.



// ##### COMPONENT SEGMENT #####

// Author Class
public class FurnaceAuthoring : MonoBehaviour, IBuildingComponent
{
    // Defining member variables.
    protected int[] acceptedResources = [];                 // Furnaces are output-only; no inputs are accepted.
    protected int maxStackSize = processorStackSize;        // Non-belt building stack size assignment.
    protected int totalInventorySize = processorStackSize;  // 1 output slot = 1 * stack size.
}

// Baker Class
public class FurnaceBaker : Baker<FurnaceAuthoring>
{
    public override void Bake(FurnaceAuthoring authoring)
    {
        // Assigning an ECS-created GameObject to "entity".
        var entity = GetEntity(authoring);
        // Making an instance of the FurnaceComponent struct and assigning it to "entity".
        var furnace = new FurnaceComponent
        {
            acceptedResources = authoring.acceptedResources,
            maxStackSize = authoring.processorStackSize,
            totalInventorySize = authoring.totalInventorySize
        };
        AddComponent(entity, furnace);
    }
}

// Component Struct
public struct FurnaceComponent : IComponentData, IBuildingComponent
{
    protected int[] acceptedResources;  // Furnaces are output-only; no inputs are accepted.
    protected int maxStackSize;         // Non-belt building stack size assignment.
    protected int totalInventorySize;   // 1 output slot = 1 * stack size.
}



// ##### SYSTEM SEGMENT #####

// System Partial Struct
public partial struct FurnaceSystem : ISystem, IBuildingSystem
{
    public void OnUpdate(ref SystemState state)
    {
        // Collecting DeltaTime property from Entities.SystemAPI.Time.
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Selecting all enttiies w/ a FurnaceComponent using read-write access.
        foreach (FurnaceComponent furnace in SystemAPI.Query<RefRW<FurnaceComponent>>())
        {
            // TO-DO: Fill contents here for what furnaces should do when they update.
        }
    }
}
