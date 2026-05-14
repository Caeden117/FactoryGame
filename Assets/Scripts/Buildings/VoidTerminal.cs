using UnityEngine;          // For functionality. 



// Brief description.
public class VoidTerminal : AbstractBuilding
{

    // ##### MEMBER VARIABLE OVERRIDES #####
    protected int Capacity { get; set; } = 0;


    // ##### METHODS #####

    // Method Overrides
    /**
      * @brief Receives a resource. Typically invoked via another Building's Send() call. Abstracted to accomodate void terminals.
      * @param resourceID   The integer value corresponding to a resource's ID.
      * @param sender       The building sending the resource. Defaults to the 'this' keyword in Send().
      * @returns A boolean of whether or not the receive action succeeded.
      */
    override protected bool Receive(in int resourceID, in AbstractBuilding sender)
    {
        Capacity++;
        return true;
    }

    /**
      * @brief The function called during the onUpdate() override.
      * @returns A boolean of whether or not the send action succeeded.
      */
    override public void Act()
    {
        IsRunning = 0 < Capacity;
        if(IsRunning)
        {
            Capacity--;
            // TO-DO: [Future] Implement money counter based on resourceID here.
        }
        return;
    }

    // ##### Unity Methods #####
    // @brief Runs on creation of a void terminal building. Used for assigning initial cooldown and attached buildings.
    void OnCreate()
    {
        // No receivers for void terminals as they have no output slot.
        // Attempt to attach to Sender building.
        if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit potentialSender, 1.0f)) 
        {
           if(potentialSender.transform.gameObject.TryGetComponent(out AbstractBuilding toBeSender))
            {
                Senders.Add(toBeSender);
                toBeSender.Receivers.Add(this);
            }
        }
    }

    // Runs on deletion of a void terminal building. Used for manual garbage collection.
    void OnDestroy()
    {
        foreach(AbstractBuilding sender in Senders)
        {
            sender = null;
        }
    }

}
