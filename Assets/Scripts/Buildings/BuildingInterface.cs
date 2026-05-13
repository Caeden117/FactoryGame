using UnityEngine;          // For functionality.



// Our interface for enforcing building data uniformity. All buildings will utilize these variables and methods.
public abstract class AbstractBuilding : MonoBehaviour
{


    // ##### DATA #####


    // ###### Constants ######
    protected const int MinResourceID = 0;                                                                  // Minimum resource ID. Likely never to be changed.
    protected const int MaxResourceID = 15;                                                                 // Maximum resource ID (inclusive). Only changed in case maximum number of resources increases.
    protected const int ProcessorStackSize = 100;                                                           // Maximum stack size for all non-belt buildings.
    // ##### Non-virtual Member Variables #####
    protected bool[] AcceptedResources { get; set; } = new bool[MaxResourceID - MinResourceID + 1];         // An integer list of accepted resources. Primarily used for Receive() and primarily set by the Recipe struct.
    protected int[] Inventory { get; set; } = new int[MaxResourceID - MinResourceID + 1];                   // Currently-stored items accessed via resourceID.
    public AbstractBuilding Sender { get; set; } = null;                                                 // The building sending outputs to this building. null when empty or deleted.
    public AbstractBuilding Receiver { get; set; } = null;                                               // The building receiving outputs from this building. null when empty or deleted.
    // ###### Virtual Variables ######
    protected virtual int InventoryTotal { get; set; } = 0;                                                 // The total number of items stored combined across all items.
    protected virtual int OutputResource { get; set; } = -1;                                                // The integer ID of the resource being output. Must fit range [minResourceID, maxResourceID].
    protected virtual int MaxStackSize { get; set; } = ProcessorStackSize;                                  // Maximum inventory size for any one resource (in and out).
    protected virtual int MaxInventorySize { get; set; } = -1;                                              // Maximum combined inventory size for all resources.
    protected virtual float Cooldown { get; set; } = 1.0f;                                                  // Cooldown in seconds between operations. Values below 1/60 (~0.016667) will be treated as 1/60.
    protected virtual float Progress { get; set; } = 0.0f;                                                  // [0.0f, 1.0f] as a normalized scalar of progress.
    protected virtual bool IsRunning { get; set; } = false;                                                 // Is the building operating (has input need met and output capacity available).
    


    // ##### METHODS #####


    // ##### Constant Methods ######
    /**
      * @brief Toggles the building on/off using a boolean parameter.
      * @param running  The new state for the building to be in. true enables acting, false disables acting.
      */
    protected void TogglePower(in bool running)
    {
        IsRunning = running;
        return;
    }

    // ##### Virtual Methods #####
    /**
      * @brief Sends a resource.
      * @param resourceID The integer value corresponding to a resource's ID.
      * @returns A boolean of whether or not the receive action succeeded.
      */
    protected virtual bool Send(in int resourceID)
    {
        bool validReceiver = Receiver != null;
        bool canSend = OutputResource != -1 && 0 < Inventory[resourceID] && Receiver.AcceptedResources[resourceID];
        if(canSend && validReceiver)
        {
            if (Receiver.Receive(resourceID, this))
            {
                Inventory[resourceID]--;
                InventoryTotal--;
                return true;
            }
        }
        return false;
    }

    /**
      * @brief Receives a resource. Typically invoked via another Building's Send() call. Abstracted to accomodate void terminals.
      * @param resourceID The integer value corresponding to a resource's ID.
      * @param sender       The object sending the resource. Defaults to 'this' in Send().
      * @returns A boolean of whether or not the receive action succeeded.
      */
    protected virtual bool Receive(in int resourceID, in AbstractBuilding inputSender)
    {
        bool canReceive = AcceptedResources[resourceID] && Inventory[resourceID] < MaxStackSize && InventoryTotal < MaxInventorySize;
        bool validSender = inputSender != null && Sender != null && inputSender == Sender;
        if(canReceive && validSender)
        {
            Inventory[resourceID]++;
            InventoryTotal++;
            return true;
        }
        return false;
    }

    // ##### Abstract Methods #####
    
    /**
      * @brief The function called during the onUpdate() override.
      * @returns A boolean of whether or not the send action succeeded.
      */
    abstract public void Act();
}
