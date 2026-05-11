// using UnityEngine;      // For functionality. 
// using Unity.Entities;   // For ECS utilization.



// // ##### COMPONENT SEGMENT #####

// // Author Class
// public class BeltAuthoring : IBuildingComponent
// {
//     // Building Component Interface Variables.
//     protected int[] acceptedResources;                      // Belts are output-only; no inputs are accepted.
//     protected int maxStackSize = processorStackSize;        // Non-belt building stack size assignment.
//     protected int totalInventorySize = processorStackSize;  // 1 output slot = 1 * stack size.
//     // Belt-specific Variables.
//     private readonly Vector2 receivingDirection = (0, 0);     // Static Vector2 (i.e. Vector2.down) used for receiving items.
//     private readonly Vector2 sendingDirection = (0, 0);      // Static Vector2 (i.e. Vector2.up) used for sending items.
//     private int[] items;                               // The bet implemntation from Factorio.   
// }

// // Baker Class
// public class BeltBaker : Baker<BeltAuthoring>
// {
//     public override void Bake(BeltAuthoring authoring)
//     {
//         // Assigning an ECS-created GameObject to "entity".
//         var entity = GetEntity(authoring);
//         // Making an instance of the BeltComponent struct and assigning it to "entity".
//         var belt = new BeltComponent
//         {
//             acceptedResources = authoring.acceptedResources,
//             maxStackSize = authoring.processorStackSize,
//             totalInventorySize = authoring.totalInventorySize,

//             receivingDirection = authoring.receivingDirection,
//             sendingDirection = authoring.sendingDirection,
//             items = authoring.items
//         };
//         AddComponent(entity, belt);
//     }
// }

// // Component Struct
// public struct BeltComponent
// {
//     // Building Component Interface Variables.
//     private int[] acceptedResources;              // Belts are output-only; no inputs are accepted.
//     private readonly int maxStackSize;            // Non-belt building stack size assignment.
//     private readonly int totalInventorySize;      // 1 output slot = 1 * stack size.
//     // Belt-specific Variables.
//     private readonly Vector2 receivingDirection;    // Static Vector2 (i.e. Vector2.down) used for receiving items.
//     private readonly Vector2 sendingDirection;      // Static Vector2 (i.e. Vector2.up) used for sending items.
//     private int[] items;                            // The belt implemntation from Factorio.   
// }



// // ##### SYSTEM SEGMENT #####

// // System Partial Struct
// public partial struct BeltSystem : ISystem, IBuildingSystem
// {
//     public void OnUpdate(ref SystemState state)
//     {
//         // Collecting DeltaTime property from Entities.SystemAPI.Time.
//         float deltaTime = SystemAPI.Time.DeltaTime;

//         // Selecting all enttiies w/ a BeltComponent using read-write access.
//         foreach (BeltComponent belt in SystemAPI.Query<RefRW<BeltComponent>>())
//         {
//             // TO-DO: Fill contents here for what belts should do when they update.
//         }
//     }
// }
