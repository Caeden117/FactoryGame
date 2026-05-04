using UnityEngine;          // For functionality. 
using Unity.Entities;       // For ECS utilization.
// using Unity.Mathematics; // Optimized for Burst-compiled code.
// using Unity.Burst;       // For ECS BurstCompile attribute.
// using Unity.Transforms;  // For accessing transforms.



// Recipe implementation for Furnaces and Crafters.
struct Recipe
{
    private int[] acceptedResources;
    private float cooldown;
    private int outputResource;    
}



// Our interface for enforcing building data uniformity. All buildings will utilize these variables.
interface IBuildingComponent
{
    // ###### Static Constants ######
    protected static const int minResourceID = 0;                           // Minimum resource ID. Likely never to be changed.
    protected static const int maxResourceID = 15;                          // Maximum resource ID (inclusive). Only changed in case maximum number of resources increases.
    protected static const int processorStackSize = 100;                    // Maximum stack size for all non-belt buildings.
    protected static const int beltStackSize = 1;                           // Maximum stack size for all belts.
    // ###### Resource Lists ######
    protected int[] acceptedResources;                                      // An integer list of accepted resources. Primarily used for Receive() and primarily set by the Recipe struct.
    protected int[] inventory = [0] * (maxResourceID - minResourceID + 1);  // Currently-stored items accessed via resourceID.
    protected int outputResouce = -1;
    // ###### Plain Old Data ######
    protected int maxStackSize;                                             // Maximum inventory size for any one resource (in and out).
    protected int totalInventorySize;                                       // Maximum combined inventory size for all resources.
    protected float cooldown;                                               // Cooldown in seconds between operations. Restraining maximum precision to milliseconds would be preferable (excl. fractions).
    protected bool running = false;                                         // Is the machine operating (has input need met and output capacity available).
    protected bool open = true;                                             // Is the machine able to receive resources?
    protected Recipe recipe;                                    
}



// Our interface for enforcing building method uniformity. All buildings will utilize these methods.
interface IBuildingSystem
{
    // ##### Static Methods ######

    /**
      * @brief Disables the building, preventing it from acting and receiving inputs.
      */
    protected static void TurnOff()
    {
        running = false;
        open = false;
        return;
    }

    /**
      * @brief Enables the building, restarting operations, but not necessarily opening it to inputs.
      */
    protected static void TurnOn()
    {
        running = true;
        return;
    }

    // ##### Non-Static Methods #####

    /**
      * @brief attempts to send a resource to a neighboring building.
      * @param direction    A shorthand Vector2 for cardinal directions (i.e. Vector2.down).
      * @param resourceID   An integer in the range [0, 15] determining the sent resource type.
      * @returns 0 or -1: 0 if succeeds, -1 if fails.
      */
    protected int Send(in Vector2 direction, in int resourceID);

    /**
      * @brief attempts to receive a resource from a neighboring building. Should be exclusively called from other buildings via Send().
      * @param resourceID   An integer in the range [0, 15] determining received resource type.
      * @returns 0 or -1: 0 if succeeds, -1 if fails.
      */
    protected int Receive(in Vector2 direction, in int resourceID);

    
}
