using UnityEngine;      // For functionality. 
using Unity.Entities;   // For ECS utilization.



// ##### COMPONENT SEGMENT #####

// Author Class
public class MinerAuthoring : MonoBehaviour, IBuildingComponent
{
    // Defining member variables.
    private int[] acceptedResources = [];                           // Miners are output-only; no inputs are accepted.
    private readonly int maxStackSize = processorStackSize;         // Non-belt building stack size assignment.
    private readonly int totalInventorySize = processorStackSize;   // 1 output slot = 1 * stack size.
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
            totalInventorySize = authoring.totalInventorySize,
            orientation = authoring.transform.rotation
        };
        AddComponent(entity, miner);
    }
}

// Component Struct
public struct MinerComponent : IComponentData, IBuildingComponent
{
    private int[] acceptedResources;  // Miners are output-only; no inputs are accepted.
    private int maxStackSize;         // Non-belt building stack size assignment.
    private int totalInventorySize;   // 1 output slot = 1 * stack size.
    private Quaternion orientation;   // Orientation for determining output location.
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
            Act(miner);
        }
    }

    private void Act(ref MinerComponent miner)
    {
        // The miner tries to send its output resource.
        if(!Send(miner, 0, miner.outputResource))   // <-- TO-DO: Replace 0 w/ receiver location.
        {
            if(miner.inventory[outputResource] < miner.maxStackSize)
            {
                miner.inventory[outputResource]++;
            }
            else
            {
                togglePower(miner, false);
            }
        }
        else
        {
            togglePower(miner, true);
        }
    }
}
