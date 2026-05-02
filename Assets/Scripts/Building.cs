using UnityEngine;          // For functionality. 
using Unity.Entities;       // For ECS authoring/baking.
using Unity.Mathematics;    // For ECS authoring/baking(?).





// THIS IS A DEPRECATED FILE USED FOR REFERENCE WHEN IMPLEMENTING INDIVIDUAL BUILDINGS. DO NOT MODIFY.





// The interface for all buildings (Miners, Furnaces, Crafters, Belts(?))
interface IBuilding
{
    // Static Constants
    protected static const int minResourceID = 0;                           // Minimum resource ID. Likely never to be changed.
    protected static const int maxResourceID = 15;                          // Maximum resource ID (inclusive). Only changed in case maximum number of resources increases.
    // Resource Lists
    protected int[] acceptedResources;                                      // An integer list of accepted resources. Primarily used for Receive() and primarily set by the Recipe struct.
    protected int[] inventory = [0] * (maxResourceID - minResourceID + 1);  // Currently-stored items accessed via resourceID.
    // Plain Old Data
    protected int maxStackSize;                                             // Maximum inventory size for any one resource (in and out).
    protected int totalInventorySize;                                       // Maximum combined inventory size for all resources.
    protected float cooldown;                                               // Cooldown in seconds between operations. Restraining maximum precision to milliseconds would be preferable (excl. fractions).
    protected bool running = false;                                         // Is the machine operating (has input need met and output capacity available).
    protected bool open = true;                                             // Is the machine able to receive resources?

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

    protected static void TurnOff()
    {
        running = false;
        return;
    }

    protected static void TurnOn()
    {
        running = true;
        return;
    }

}

private readonly struct Recipe
{
    static const int maxStackSize = 100;    // Maximum stack size per I/O item.
}

class Miner : IBuilding
{
    
}


class Crafter : IBuilding
{
    
}

class Belt : IBuilding
{
    private readonly Vector2 ReceivingDirection;  // Static Vector2 (i.e. Vector2.down) used for receiving items.
    public readonly Vector2 SendingDirection;     // Static Vector2 (i.e. Vector2.up) used for sending items.
    private readonly int CurrentItem;             // Inventory handler integer to account for              

    public void OnUpdate(ref SystemState state)
    {
        if(running)
        {
            
        }
    }
}
