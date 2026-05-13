using UnityEngine;          // For functionality. 
using System;               // For WeakReference.



// Brief description.
public class Furnace : AbstractBuilding
{


    // ##### MEMBER VARIABLE OVERRIDES #####
    protected override int OutputResource { get; set; } = -1;
    protected override int InventoryTotal { get; set; } = 200;
    protected override int MaxInventorySize { get; set; } = 200;
    protected override float Cooldown { get; set; } = 2.0f;
    protected override float Progress { get; set; } = 0.0f;
    protected override bool IsRunning { get; set; } = false;
    protected float ActTimer;
    protected RecipeSO recipe = null;

    // ##### METHODS #####


    // ##### Method Overrides #####
    

    /**
      * @brief The function called during the onUpdate() override.
      * @returns A boolean of whether or not the send action succeeded.
      */
    override public void Act()
    {
        if(recipe != null)
        {
            
        }
        return;
    }

    // ##### Unity Methods #####

    void OnCreate()
    {
        ActTimer = Cooldown;
        // Attempt to attach to Receiver building.
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit potentialReceiver, 1.0f)) 
        {
           if(potentialReceiver.transform.gameObject.TryGetComponent(out AbstractBuilding toBeReceiver))
            {
                Receiver = toBeReceiver;
                Receiver.Sender = this;
            }
        }
        // Attempt to attach to Sender building.
        if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit potentialSender, 1.0f)) 
        {
           if(potentialSender.transform.gameObject.TryGetComponent(out AbstractBuilding toBeSender))
            {
                Sender = toBeSender;
                Sender.Receiver = this;
            }
        }
    }

    void OnDestroy()
    {
        Sender = null;
        Receiver = null;
    }

}
