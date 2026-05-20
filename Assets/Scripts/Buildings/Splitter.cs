using UnityEngine;          // For functionality.



// Brief description.
public class Splitter : AbstractBuilding
{

    // ##### MEMBER VARIABLE OVERRIDES #####
    protected int currentID = -1;
    protected int fairnessTracker = 0;  // Used for distributing evenly when multiple receivers maintain open states.

    // ##### METHODS #####

    // Method Overrides
    /**
      * @brief The function called during the onUpdate() override.
      * @returns A boolean of whether or not the send action succeeded.
      */
    override internal void Act()
    {
        // Two Loops used to mimic a single loop starting partway thru and wrapping around (i.e. [0, 4] starting at 2, ending at 1).
        // Loop 1: Start at fairness tracker and try to send.
        for(int i = fairnessTracker; i < Receivers.Count; i++)
        {
            if(Receivers[i] != null && Send(currentID, this))
            {
                fairnessTracker = (i + 1) % Receivers.Count;    // Modulus to clamp and wrap values within range.
                return;
            }
        }
        // Loop 2: Start at 0 and go to fairness tracker and try to send.
        for(int i = 0; i < fairnessTracker; i++)
        {
            if(Receivers[i] != null && Send(currentID, this))
            {
                fairnessTracker = (i + 1) % Receivers.Count;    // Modulus to clamp and wrap values within range.
                return;
            }
        }
        // All sends failed; reset fairness tracker.
        fairnessTracker = 0;
        return;
    }

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

    // Unity Methods
    // @brief Runs on creation of a splitter building. Used for assigning initial cooldown and attached buildings.
    private void Start()
    {
        ActTimer = Cooldown;
        // Sets all resources to accepted.
        for(int i = 0; i < AcceptedResources.Length; i++)
        {
            AcceptedResources[i] = true;
        }
        // Attempt to attach to Receiver building left of merger.
        if (Physics.Raycast(transform.position, -transform.right, out RaycastHit potentialLeftReceiver, ConnectionRange))
        {
            if (potentialLeftReceiver.transform.gameObject.TryGetComponent(out AbstractBuilding toBeLeftReceiver))
            {
                Receivers.Add(toBeLeftReceiver);
                toBeLeftReceiver.Senders.Add(this);
            }
        }
        // Attempt to attach to Receiver building in front of merger.
        if (Physics.Raycast(transform.position, transform.right, out RaycastHit potentialReceiver, ConnectionRange))
        {
            if (potentialReceiver.transform.gameObject.TryGetComponent(out AbstractBuilding toBeReceiver))
            {
                Receivers.Add(toBeReceiver);
                toBeReceiver.Senders.Add(this);
            }
        }
        // Attempt to attach to Receiver building right of merger.
        if (Physics.Raycast(transform.position, transform.right, out RaycastHit potentialRightReceiver, ConnectionRange))
        {
            if (potentialRightReceiver.transform.gameObject.TryGetComponent(out AbstractBuilding toBeRightReceiver))
            {
                Receivers.Add(toBeRightReceiver);
                toBeRightReceiver.Senders.Add(this);
            }
        }
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

    // Runs on deletion of a splitter building. Used for manual garbage collection.
    void OnDestroy()
    {
        
    }

}
