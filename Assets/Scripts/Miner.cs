using UnityEngine;      // For functionality. 
using Unity.Entities;   // For ECS utilization.



// ##### COMPONENT SEGMENT #####

// Author Class
public class MinerAuthoring : MonoBehaviour, IBuildingComponent
{
    // Defining member variables.
    protected int[] acceptedResources = [];                 // Miners are output-only; no inputs are accepted.
    protected int maxStackSize = processorStackSize;        // Non-belt building stack size assignment.
    protected int totalInventorySize = processorStackSize;  // 1 output slot = 1 * stack size.
}

// Baker Class
public class MinerBaker : Baker<MinerAuthoring>
{
    public override void Bake(MinerAuthoring authoring)
    {
        // Assigning an ECS-created GameObject to "entity".
        var entity = GetEntity(authoring);
        // Making an instance of the MinerComponent struct and assigning it to "entity".
        var miner = new MinerComponent
        {
            acceptedResources = authoring.acceptedResources,
            maxStackSize = authoring.processorStackSize,
            totalInventorySize = authoring.totalInventorySize
        };
        AddComponent(entity, miner);
    }
}

// Component Struct
public struct MinerComponent : IComponentData, IBuildingComponent
{
    protected int[] acceptedResources;  // Miners are output-only; no inputs are accepted.
    protected int maxStackSize;         // Non-belt building stack size assignment.
    protected int totalInventorySize;   // 1 output slot = 1 * stack size.
}



// ##### SYSTEM SEGMENT #####

// System Partial Struct
public partial struct MinerSystem : ISystem, IBuildingSystem
{
    public void OnUpdate(ref SystemState state)
    {
        // Collecting DeltaTime property from Entities.SystemAPI.Time.
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Selecting all enttiies w/ a MinerComponent using read-write access.
        foreach (MinerComponent miner in SystemAPI.Query<RefRW<MinerComponent>>())
        {
            // TO-DO: Fill contents here for what miners should do when they update.
        }
    }
}
