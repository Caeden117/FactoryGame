using UnityEngine;                  // For functionality.
using System.Collections.Generic;   // For List<T>.



// Our interface for enforcing building data uniformity. All buildings will utilize these variables and methods.
public abstract class AbstractBuilding : MonoBehaviour
{


    // ##### DATA #####


    // ###### Constants ######
    protected const int MinResourceID = 0;                                                          // Minimum resource ID. Likely never to be changed.
    protected const int MaxResourceID = 15;                                                         // Maximum resource ID (inclusive). Only changed in case maximum number of resources increases.
    protected const int ProcessorStackSize = 100;                                                   // Maximum stack size for all non-belt buildings.
    protected const int MaxIOcount = 4;                                                             // Maximum number of input slots and maximum number of output slots.
    // ##### Non-virtual Member Variables #####
    [SerializeField] protected float ConnectionRange = 0.51f;                                       // Raycast distance for automatic building linkage.
    protected TilemapChunk tm;
    internal bool[] AcceptedResources { get; set; } = new bool[MaxResourceID - MinResourceID + 1];  // An integer list of accepted resources. Primarily used for Receive() and primarily set by the Recipe struct.
    protected int[] Inventory { get; set; } = new int[MaxResourceID - MinResourceID + 1];           // Currently-stored, received items accessed via resourceID.
    protected int[] Outventory { get; set; } = new int[MaxResourceID - MinResourceID + 1];          // Currently-stored items to be delivered, accessed via resourceID.
    internal List<AbstractBuilding> Senders { get; set; } = new List<AbstractBuilding>();           // The building sending outputs to this building. null when empty or deleted.
    internal List<AbstractBuilding> Receivers { get; set; } = new List<AbstractBuilding>();         // The building receiving outputs from this building. null when empty or deleted.
    // ###### Virtual Variables ######
    protected virtual bool IsRunning { get; set; } = false;                                         // Is the building operating (has input need met and output capacity available).
    protected virtual int MaxStackSize { get; set; } = ProcessorStackSize;                          // Maximum inventory size for any one resource (in and out).
    protected virtual float Cooldown { get; set; } = 1.0f;                                          // Cooldown in seconds between operations. Values below 1/60 (~0.016667) will be treated as 1/60.
    protected virtual float Progress { get; set; } = 0.0f;                                          // [0.0f, 1.0f] as a normalized scalar of progress.
    protected virtual float ActTimer { get; set; } = 1.0f;                                          // Cooldown tracker in seconds. Typically set equal to Cooldown in OnCreate() method.



    // ##### METHODS #####


    // ##### Constant Methods ######
    /**
      * @brief Toggles the building on/off using a boolean parameter.
      * @param running  The new state for the building to be in. true enables acting, false disables acting.
      */
    protected void TogglePower(in bool running)
    {
        IsRunning = running;
        if (!IsRunning)
        {
            ResetProgress();
        }
        return;
    }

    /**
      * @brief Resets the action timer and progress accordingly.
      */
    protected void ResetProgress()
    {
        ActTimer = Cooldown;
        Progress = 0.0f;
        return;
    }

    // ##### Virtual Methods #####
    /**
      * @brief Sends a resource.
      * @param resourceID The integer value corresponding to a resource's ID.
      * @returns A boolean of whether or not the receive action succeeded.
      */
    protected virtual bool Send(in int resourceID, AbstractBuilding inputReceiver)
    {
        bool validReceiver = inputReceiver != null && Receivers.Contains(inputReceiver);
        bool canSend = 0 < Outventory[resourceID] && inputReceiver.AcceptedResources[resourceID];
        if (canSend && validReceiver)
        {
            if (inputReceiver.Receive(resourceID, this))
            {
                Outventory[resourceID]--;
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
    internal virtual bool Receive(in int resourceID, AbstractBuilding inputSender)
    {
        bool canReceive = AcceptedResources[resourceID] && Inventory[resourceID] < MaxStackSize;
        bool validSender = inputSender != null && Senders.Contains(inputSender);
        if (canReceive && validSender)
        {
            Inventory[resourceID]++;
            return true;
        }
        return false;
    }

    internal virtual void Update()
    {
        Act();
    }
    /*
    internal virtual void Awake()
    {
        tm = FindAnyObjectByType(TilemapChunk);
    }
    */

    // ##### Abstract Methods #####

    /**
      * @brief The function called during the onUpdate() override.
      * @returns A boolean of whether or not the send action succeeded.
      */
    abstract internal void Act();
}
