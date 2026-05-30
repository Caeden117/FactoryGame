using UnityEngine;          // For functionality. 



// Brief description.
public class VoidTerminal : AbstractBuilding
{

    // ##### MEMBER VARIABLE OVERRIDES #####
    [SerializeField] protected ItemLibrarySO il;
    protected int currentID = -1;


    // ##### METHODS #####

    // Method Overrides
    /**
      * @brief Receives a resource. Typically invoked via another Building's Send() call. Abstracted to accomodate void terminals.
      * @param resourceID   The integer value corresponding to a resource's ID.
      * @param sender       The building sending the resource. Defaults to the 'this' keyword in Send().
      * @returns A boolean of whether or not the receive action succeeded.
      */
    override internal bool Receive(in int resourceID, AbstractBuilding sender)
    {
        if (currentID == -1)
        {
            currentID = resourceID;
            return true;
        }
        return false;
    }

    /**
      * @brief The function called during the onUpdate() override.
      * @returns A boolean of whether or not the send action succeeded.
      */
    override internal void Act()
    {
        IsRunning = MinResourceID <= currentID && currentID <= MaxResourceID;
        if (IsRunning && il != null)
        {
            // Update Money State with sold item
            MoneyState.Add(il.Items[currentID].MoneyValue);
            currentID = -1;
        }
        return;
    }

    // ##### Unity Methods #####
    // @brief Runs on creation of a void terminal building. Used for assigning initial cooldown and attached buildings.
    void Start()
    {
        // Sets all resources to accepted.
        for (int i = 0; i < AcceptedResources.Length; i++)
        {
            AcceptedResources[i] = true;
        }
        // No receivers for void terminals as they have no output slot.
        // Attempt to attach to Sender building.
        if (Physics.Raycast(transform.position, -transform.right, out RaycastHit potentialSender, ConnectionRange))
        {
            if (potentialSender.transform.gameObject.TryGetComponent(out AbstractBuilding toBeSender))
            {
                Senders.Add(toBeSender);
                toBeSender.Receivers.Add(this);
            }
        }
    }
}
