using UnityEngine;          // For functionality.



// Brief description.
public class Miner : AbstractBuilding
{

    // ##### MEMBER VARIABLE OVERRIDES #####
    protected int OutputResource { get; set; } = 0;
    protected override float Cooldown { get; set; } = 2.0f;
    protected override float Progress { get; set; } = 0.0f;
    protected override bool IsRunning { get; set; } = true;


    // ##### METHODS #####

    // Building-unique Methods
    /**
      * @brief Creates an OutputResource from scratch.
      * @returns A boolean of whether or not the mine action succeeded.
      */
    protected bool Mine()
    {
        bool canMine = OutputResource != -1 && Inventory[OutputResource] < MaxStackSize;
        if (canMine)
        {
            Outventory[OutputResource]++;
        }
        return canMine;
    }

    // Method Overrides
    /**
      * @brief Receives a resource. Typically invoked via another Building's Send() call. Abstracted to accomodate void terminals.
      * @param resourceID   The integer value corresponding to a resource's ID.
      * @param sender       The building sending the resource. Defaults to the 'this' keyword in Send().
      * @returns A boolean of whether or not the receive action succeeded.
      */
    override protected bool Receive(in int resourceID, in AbstractBuilding sender)
    {
        return false;
    }

    /**
      * @brief The function called during the onUpdate() override.
      * @returns A boolean of whether or not the send action succeeded.
      */
    override public void Act()
    {
        ActTimer -= Time.deltaTime;
        if (ActTimer <= 0)
        {
            bool canMine = Mine();
            bool canSend = Send(OutputResource, Receivers[0]);
            TogglePower(canMine || canSend);
            ActTimer = Cooldown;
        }
    }

    // Unity Methods
    // @brief Runs on creation of a miner building. Used for assigning initial cooldown and attached buildings.
    void OnCreate()
    {
        ActTimer = Cooldown;
        // Attempt to attach to Receiver building.
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit potentialReceiver, 1.0f))
        {
            if (potentialReceiver.transform.gameObject.TryGetComponent(out AbstractBuilding toBeReceiver))
            {
                Receivers.Add(toBeReceiver);
                toBeReceiver.Senders.Add(this);
            }
        }
        // No sender for Miners as they have no input slot.
    }

    // Runs on deletion of a miner building. Used for manual garbage collection.
    void OnDestroy()
    {
        foreach(AbstractBuilding receiver in Receivers)
        {
            receiver = null;
        }
    }

}
