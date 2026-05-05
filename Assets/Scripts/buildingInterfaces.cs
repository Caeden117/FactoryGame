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
    static protected const int minResourceID = 0;                           // Minimum resource ID. Likely never to be changed.
    static protected const int maxResourceID = 15;                          // Maximum resource ID (inclusive). Only changed in case maximum number of resources increases.
    static protected const int processorStackSize = 100;                    // Maximum stack size for all non-belt buildings.
    static protected const int beltStackSize = 1;                           // Maximum stack size for all belts.
    // ###### Component Data ######
    protected int[] acceptedResources;                                      // An integer list of accepted resources. Primarily used for Receive() and primarily set by the Recipe struct.
    protected int[] inventory = [0] * (maxResourceID - minResourceID + 1);  // Currently-stored items accessed via resourceID.
    protected int inventoryTotal = 0;                                       // The total number of items stored combined across all items.
    protected int outputResouce = -1;                                       // The integer ID of the resource being output. Must fit range [minResourceID, maxResourceID].
    protected int maxStackSize;                                             // Maximum inventory size for any one resource (in and out).
    protected int maxInventorySize;                                         // Maximum combined inventory size for all resources.
    protected float cooldown;                                               // Cooldown in seconds between operations. Values below 1/60 (~0.016667) will be treated as 1/60.
    protected bool running = false;                                         // Is the building operating (has input need met and output capacity available).
    protected Vector2 inputLocation;                                        // Where the building should receive resources from.
    protected Vector2 outputLocation;                                       // Where the building should send resources to.
}



// Our interface for enforcing building method uniformity. All buildings will utilize these methods.
interface IBuildingSystem
{
    // ##### Static Methods ######

    /**
      * @brief Toggles the building on/off using a boolean parameter.
      * @param building The building component to be turned off.
      * @param running  The new state for the building to be in. true enables acting, false disables acting.
      */
    static protected void TogglePower(ref IBuildingComponent building, in bool running)
    {
        sender.running = running;
        return;
    }

    /**
      * @brief attempts to send a resource to a neighboring building.
      * @param sender   The component that is sending the resource. 
      * @param sender   The component that is receiving the resource.
      * @returns A boolean of whether or not the send action succeeded.
      */
    static protected bool Send(ref IBuildingComponent sender, ref IBuildingComponent receiver)
    {
        bool canSend = sender.outputResouce != -1 && 0 < sender.inventory[resourceID];
        bool canReceive = receiver.acceptedResources.contains(resourceID) && receiver.inventory[resourceID] < maxStackSize && receiver.inventoryTotal < receiver.maxInventorySize;
        if(canSend && canReceive)
        {
            receiver.inventory[resourceID]++;
            receiver.inventoryTotal++;
            sender.inventory[resourceID]--;
            sender.inventoryTotal--;
            return true;
        }
        return false;
    }

    // ##### Non-Static Methods #####

    /**
      * @brief The function called during the onUpdate() override.
      * @param The component to act upon. Should only act upon the paired component (i.e. miner components for miner systems).
      * @returns 0 or -1: 0 if succeeds, -1 if fails.
      */
    protected bool Act(ref IBuildingComponent actor);
    
}
