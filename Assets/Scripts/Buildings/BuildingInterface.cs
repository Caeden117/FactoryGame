using UnityEngine;          // For functionality. 
using System;               // For WeakReference.



// Our interface for enforcing building data uniformity. All buildings will utilize these variables and methods.
public abstract class AbstractBuilding : MonoBehaviour
{


    // ##### DATA #####


    // ###### Constants ######
    protected const int minResourceID = 0;                                              // Minimum resource ID. Likely never to be changed.
    protected const int maxResourceID = 15;                                             // Maximum resource ID (inclusive). Only changed in case maximum number of resources increases.
    protected const int processorStackSize = 100;                                       // Maximum stack size for all non-belt buildings.
    // ###### Abstracts ######
    protected bool[] acceptedResources = new bool[maxResourceID - minResourceID + 1];   // An integer list of accepted resources. Primarily used for Receive() and primarily set by the Recipe struct.
    protected int[] inventory= new int[maxResourceID - minResourceID + 1];              // Currently-stored items accessed via resourceID.
    protected int inventoryTotal = 0;                                                   // The total number of items stored combined across all items.
    protected int outputResource = -1;                                                  // The integer ID of the resource being output. Must fit range [minResourceID, maxResourceID].
    protected int maxStackSize = processorStackSize;                                    // Maximum inventory size for any one resource (in and out).
    protected int maxInventorySize = -1;                                                // Maximum combined inventory size for all resources.
    protected float cooldown = -1.0f;                                                   // Cooldown in seconds between operations. Values below 1/60 (~0.016667) will be treated as 1/60.
    protected float progress = -1.0f;                                                   // [0.0, 1.0] as a normalized scalar of progress.
    protected bool isRunning = false;                                                   // Is the building operating (has input need met and output capacity available).
    protected System.WeakReference sender = null;                                       // The building sending outputs to this building. null when empty or deleted.
    protected System.WeakReference receiver = null;                                     // The building receiving outputs from this building. null when empty or deleted.


    // ##### METHODS #####


    // ##### Constant Methods ######
    /**
      * @brief Toggles the building on/off using a boolean parameter.
      * @param running  The new state for the building to be in. true enables acting, false disables acting.
      */
    protected void TogglePower(in bool running)
    {
        isRunning = running;
        return;
    }

    /**
      * @brief Sends a resource.
      * @param resourceID The integer value corresponding to a resource's ID.
      * @returns A boolean of whether or not the receive action succeeded.
      */
    protected bool Send(in int resourceID)
    {
        bool validReceiver = receiver != null && receiver.IsAlive && receiver.Target != null && receiver.GetType() == typeof(AbstractBuilding);
        bool canSend = outputResource != -1 && 0 < inventory[resourceID];
        AbstractBuilding receiverProxy = null;
        if(canSend && validReceiver)
        {
            receiverProxy = (AbstractBuilding)receiver.Target;
            if (receiverProxy.Receive(resourceID, this))
            {
                inventory[resourceID]--;
                inventoryTotal--;
                return true;
            }
        }
        return false;
    }

    /**
      * @brief static default helper for Receive(); most buildings will proxy to this function via Receive().
      * @param resourceID The integer value corresponding to a resource's ID.
      * @param sender       The object sending the resource. Defaults to 'this' in Send().
      * @returns A boolean of whether or not the receive action succeeded.
      */
    protected bool Store(in int resourceID, in AbstractBuilding inputSender)
    {
        bool canReceive = acceptedResources[resourceID] && inventory[resourceID] < maxStackSize && inventoryTotal < maxInventorySize;
        bool validSender = sender != null && sender.IsAlive && sender.Target != null && sender.GetType() == typeof(AbstractBuilding);
        bool validInputSender = inputSender != null;
        if(canReceive && validSender && validInputSender && inputSender == (AbstractBuilding)sender.Target)
        {
            inventory[resourceID]++;
            inventoryTotal++;
            return true;
        }
        return false;
    }

    // ##### Abstract Methods #####
    /**
      * @brief Receives a resource. Typically invoked via another Building's Send() call. Abstracted to accomodate void terminals.
      * @param resourceID   The integer value corresponding to a resource's ID.
      * @param sender       The building sending the resource. Defaults to the 'this' keyword in Send().
      * @returns A boolean of whether or not the receive action succeeded.
      */
    abstract protected bool Receive(in int resourceID, in AbstractBuilding inputSender);

    /**
      * @brief The function called during the onUpdate() override.
      * @returns A boolean of whether or not the send action succeeded.
      */
    abstract protected bool Act();
}
