using UnityEngine;          // For functionality.



// Brief description.
public class Merger : AbstractBuilding
{

    // ##### MEMBER VARIABLE OVERRIDES #####
    protected int currentID = -1;

    // ##### METHODS #####

    // Method Overrides
    /**
      * @brief Sends a resource.
      * @param resourceID The integer value corresponding to a resource's ID.
      * @returns A boolean of whether or not the receive action succeeded.
      */
    override protected bool Send(in int resourceID, AbstractBuilding inputReceiver)
    {
        bool validReceiver = inputReceiver != null && Receivers.Contains(inputReceiver);
        bool canSend = inputReceiver.AcceptedResources[resourceID];
        if(canSend && validReceiver)
        {
            if (inputReceiver.Receive(resourceID, this))
            {
                currentID = -1;
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
    override internal bool Receive(in int resourceID, AbstractBuilding inputSender)
    {
        bool canReceive =  currentID == -1 && Outventory[resourceID] < MaxStackSize;
        bool validSender = inputSender != null && Senders.Contains(inputSender);
        if(canReceive && validSender)
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
        ActTimer -= Time.deltaTime;
        if (ActTimer <= 0)
        {
            TogglePower(Send(currentID, Receivers[0]));
            ResetProgress();
        }
    }

    // Unity Methods
    // @brief Runs on creation of a merger building. Used for assigning initial cooldown and attached buildings.
    private void Start()
    {
        ActTimer = Cooldown;
        // Sets all resources to accepted.
        for(int i = 0; i < AcceptedResources.Length; i++)
        {
            AcceptedResources[i] = true;
        }
        // Attempt to attach to Receiver building.
        if (Physics.Raycast(transform.position, transform.right, out RaycastHit potentialReceiver, ConnectionRange))
        {
            if (potentialReceiver.transform.gameObject.TryGetComponent(out AbstractBuilding toBeReceiver))
            {
                Receivers.Add(toBeReceiver);
                toBeReceiver.Senders.Add(this);
            }
        }
        // Attempt to attach to Sender building left of merger.
        if (Physics.Raycast(transform.position, -transform.right, out RaycastHit potentialLeftSender, ConnectionRange))
        {
            if (potentialLeftSender.transform.gameObject.TryGetComponent(out AbstractBuilding toBeLeftSender))
            {
                Senders.Add(toBeLeftSender);
                toBeLeftSender.Receivers.Add(this);
            }
        }
        // Attempt to attach to Sender building in behind merger.
        if (Physics.Raycast(transform.position, -transform.right, out RaycastHit potentialSender, ConnectionRange))
        {
            if (potentialSender.transform.gameObject.TryGetComponent(out AbstractBuilding toBeSender))
            {
                Senders.Add(toBeSender);
                toBeSender.Receivers.Add(this);
            }
        }
        // Attempt to attach to Sender building right of merger.
        if (Physics.Raycast(transform.position, transform.right, out RaycastHit potentialRightSender, ConnectionRange))
        {
            if (potentialRightSender.transform.gameObject.TryGetComponent(out AbstractBuilding toBeRightSender))
            {
                Senders.Add(toBeRightSender);
                toBeRightSender.Receivers.Add(this);
            }
        }
    }

    // Runs on deletion of a miner merger. Used for manual garbage collection.
    void OnDestroy()
    {
        
    }

}
