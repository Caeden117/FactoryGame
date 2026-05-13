using UnityEngine;          // For functionality. 
using System;               // For WeakReference.



// Our interface for enforcing building data uniformity. All buildings will utilize these variables and methods.
public abstract class AbstractBuilding : MonoBehaviour
{


    // ##### DATA #####


    // ###### Constants ######
    protected const int MinResourceID = 0;                                              // Minimum resource ID. Likely never to be changed.
    protected const int MaxResourceID = 15;                                             // Maximum resource ID (inclusive). Only changed in case maximum number of resources increases.
    protected const int ProcessorStackSize = 100;                                       // Maximum stack size for all non-belt buildings.
    // ###### Virtual Variables ######
    protected virtual bool[] AcceptedResources { get; set; } = new bool[MaxResourceID - MinResourceID + 1];   // An integer list of accepted resources. Primarily used for Receive() and primarily set by the Recipe struct.
    protected virtual int[] Inventory { get; set; } = new int[MaxResourceID - MinResourceID + 1];              // Currently-stored items accessed via resourceID.
    protected virtual int InventoryTotal { get; set; } = 0;                                                   // The total number of items stored combined across all items.
    protected virtual int OutputResource { get; set; } = -1;                                                  // The integer ID of the resource being output. Must fit range [minResourceID, maxResourceID].
    protected virtual int MaxStackSize { get; set; } = ProcessorStackSize;                                    // Maximum inventory size for any one resource (in and out).
    protected virtual int MaxInventorySize { get; set; } = -1;                                                // Maximum combined inventory size for all resources.
    protected virtual float Cooldown { get; set; } = -1.0f;                                                   // Cooldown in seconds between operations. Values below 1/60 (~0.016667) will be treated as 1/60.
    protected virtual float Progress { get; set; } = -1.0f;                                                   // [0.0, 1.0] as a normalized scalar of progress.
    protected virtual bool IsRunning { get; set; } = false;                                                   // Is the building operating (has input need met and output capacity available).
    protected virtual System.WeakReference Sender { get; set; } = null;                                       // The building sending outputs to this building. null when empty or deleted.
    protected virtual System.WeakReference Receiver { get; set; } = null;                                     // The building receiving outputs from this building. null when empty or deleted.


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

    /**
      * @brief Sends a resource.
      * @param resourceID The integer value corresponding to a resource's ID.
      * @returns A boolean of whether or not the receive action succeeded.
      */
    protected virtual bool Send(in int resourceID)
    {
        bool validReceiver = Receiver != null && Receiver.IsAlive && Receiver.Target != null && Receiver.GetType() == typeof(AbstractBuilding);
        bool canSend = OutputResource != -1 && 0 < Inventory[resourceID];
        AbstractBuilding receiverProxy = null;
        if(canSend && validReceiver)
        {
            receiverProxy = (AbstractBuilding)Receiver.Target;
            if (receiverProxy.Receive(resourceID, this))
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
        bool validSender = Sender != null && Sender.IsAlive && Sender.Target != null && Sender.GetType() == typeof(AbstractBuilding);
        bool validInputSender = inputSender != null;
        if(canReceive && validSender && validInputSender && inputSender == (AbstractBuilding)Sender.Target)
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
    abstract protected bool Act();
}
