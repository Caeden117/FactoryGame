// using UnityEngine;      // For functionality. 
// using Unity.Entities;   // For ECS utilization.



// // ##### COMPONENT SEGMENT #####

// // Author Class
// public class MergerAuthoring : IBuildingComponent
// {
//     // Defining member variables.
//     private int[] acceptedResources;                                // Miners are output-only; no inputs are accepted.
//     private readonly int maxStackSize = processorStackSize;         // Non-belt building stack size assignment.
//     private readonly int totalInventorySize = processorStackSize;   // 1 output slot = 1 * stack size.
// }

// // Baker Class
// public class MergerBaker : Baker<MergerAuthoring>
// {
//     public override void Bake(MergerAuthoring authoring)
//     {
//         // Assigning an ECS-created GameObject to "entity".
//         var entity = GetEntity(authoring);
//         // Making an instance of the MergerComponent struct and assigning it to "entity".
//         var merger = new MergerComponent
//         {
//             acceptedResources = authoring.acceptedResources,
//             maxStackSize = authoring.processorStackSize,
//             totalInventorySize = authoring.totalInventorySize
//         };
//         AddComponent(entity, merger);
//     }
// }

// // Component Struct
// public struct MergerComponent : IComponentData, IBuildingComponent
// {
//     private int[] acceptedResources;  // Mergers are output-only; no inputs are accepted.
//     private int maxStackSize;         // Non-belt building stack size assignment.
//     private int totalInventorySize;   // 1 output slot = 1 * stack size.
// }



// // ##### SYSTEM SEGMENT #####

// // System Partial Struct
// public partial struct MergerSystem : ISystem, IBuildingSystem
// {
//     public void OnUpdate(ref SystemState state)
//     {
//         // Collecting DeltaTime property from Entities.SystemAPI.Time.
//         float deltaTime = SystemAPI.Time.DeltaTime;

//         // Selecting all enttiies w/ a MergerComponent using read-write access.
//         foreach (MergerComponent merger in SystemAPI.Query<RefRW<MergerComponent>>())
//         {
//             // TO-DO: Fill contents here for what mergers should do when they update.
//         }
//     }
// }
