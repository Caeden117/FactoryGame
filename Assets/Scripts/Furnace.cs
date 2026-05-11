// using UnityEngine;      // For functionality. 
// using Unity.Entities;   // For ECS utilization.



// // ##### COMPONENT SEGMENT #####

// // Author Class
// public class FurnaceAuthoring : IBuildingComponent
// {
//     // Defining member variables.
//     private int[] acceptedResources;                                // Miners are output-only; no inputs are accepted.
//     private readonly int maxStackSize = processorStackSize;         // Non-belt building stack size assignment.
//     private readonly int totalInventorySize = processorStackSize;   // 1 output slot = 1 * stack size.
//     private Recipe recipe;
// }

// // Baker Class
// public class FurnaceBaker : Baker<FurnaceAuthoring>
// {
//     public override void Bake(FurnaceAuthoring authoring)
//     {
//         // Assigning an ECS-created GameObject to "entity".
//         var entity = GetEntity(authoring);
//         // Making an instance of the FurnaceComponent struct and assigning it to "entity".
//         var furnace = new FurnaceComponent
//         {
//             acceptedResources = authoring.acceptedResources,
//             maxStackSize = authoring.processorStackSize,
//             totalInventorySize = authoring.totalInventorySize
//         };
//         AddComponent(entity, furnace);
//     }
// }

// // Component Struct
// public struct FurnaceComponent : IComponentData, IBuildingComponent
// {
//     protected int[] acceptedResources;  // Furnaces are output-only; no inputs are accepted.
//     protected int maxStackSize;         // Non-belt building stack size assignment.
//     protected int totalInventorySize;   // 1 output slot = 1 * stack size.
// }



// // ##### SYSTEM SEGMENT #####

// // System Partial Struct
// public partial struct FurnaceSystem : ISystem, IBuildingSystem
// {
//     public void OnUpdate(ref SystemState state)
//     {
//         // Collecting DeltaTime property from Entities.SystemAPI.Time.
//         float deltaTime = SystemAPI.Time.DeltaTime;

//         // Selecting all enttiies w/ a FurnaceComponent using read-write access.
//         foreach (FurnaceComponent furnace in SystemAPI.Query<RefRW<FurnaceComponent>>())
//         {
//             // TO-DO: Fill contents here for what furnaces should do when they update.
//         }
//     }

//     private void Act(ref FurnaceComponent furnace)
//     {
//         // The miner tries to send its output resource.
//         if (!Send(miner, 0, miner.outputResource))   // <-- TO-DO: Replace 0 w/ receiver location.
//         {
//             if (miner.inventory[outputResource] < miner.maxStackSize)
//             {
//                 miner.inventory[outputResource]++;
//             }
//             else
//             {
//                 togglePower(miner, false);
//             }
//         }
//         else
//         {
//             togglePower(miner, true);
//         }
//     }
// }
