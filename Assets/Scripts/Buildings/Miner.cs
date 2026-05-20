using UnityEngine;          // For functionality.



// Brief description.
public class Miner : AbstractBuilding
{

    // ##### MEMBER VARIABLE OVERRIDES #####
    protected int OutputResource { get; set; } = 0;


    // ##### METHODS #####

    // Building-unique Methods
    /**
      * @brief Creates an OutputResource from scratch.
      * @returns A boolean of whether or not the mine action succeeded.
      */
    protected bool Mine()
    {
        var canMine = OutputResource != -1 && Outventory[OutputResource] < MaxStackSize;
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
    internal override bool Receive(in int resourceID, AbstractBuilding sender)
    {
        return false;
    }

    /**
      * @brief The function called during the onUpdate() override.
      * @returns A boolean of whether or not the send action succeeded.
      */
    internal override void Act()
    {
        ActTimer -= Time.deltaTime;
        if (ActTimer <= 0)
        {
            var canMine = Mine();
            var canSend = Receivers.Count > 0 && Send(OutputResource, Receivers[0]);
            TogglePower(canMine || canSend);
            ResetProgress();
        }
    }

    // Unity Methods
    // @brief Runs on creation of a miner building. Used for assigning initial cooldown and attached buildings.
    private void Start()
    {
        OnCreate();
    }

    private void OnCreate()
    {
        Cooldown = 2.0f;
        Progress = 0.0f;
        IsRunning = true;
        ActTimer = Cooldown;
        // Attempt to attach to Receiver building.
        if (Physics.Raycast(transform.position, transform.right, out var potentialReceiver, ConnectionRange))
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
        
    }

}
