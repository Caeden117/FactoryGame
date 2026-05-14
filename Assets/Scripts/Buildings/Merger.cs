using UnityEngine;          // For functionality.



// Brief description.
public class Merger : AbstractBuilding
{

    // ##### MEMBER VARIABLE OVERRIDES #####


    // ##### METHODS #####

    // Method Overrides
    /**
      * @brief The function called during the onUpdate() override.
      * @returns A boolean of whether or not the send action succeeded.
      */
    override public void Act()
    {
        return;
    }

    // Unity Methods
    // @brief Runs on creation of a merger building. Used for assigning initial cooldown and attached buildings.
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
        // Attempt to attach to Sender building left of merger.
        if (Physics.Raycast(transform.position, -transform.right, out RaycastHit potentialLeftSender, 1.0f))
        {
            if (potentialLeftSender.transform.gameObject.TryGetComponent(out AbstractBuilding toBeLeftSender))
            {
                Senders.Add(toBeLeftSender);
                toBeLeftSender.Receivers.Add(this);
            }
        }
        // Attempt to attach to Sender building in behind merger.
        if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit potentialSender, 1.0f))
        {
            if (potentialSender.transform.gameObject.TryGetComponent(out AbstractBuilding toBeSender))
            {
                Senders.Add(toBeSender);
                toBeSender.Receivers.Add(this);
            }
        }
        // Attempt to attach to Sender building right of merger.
        if (Physics.Raycast(transform.position, transform.right, out RaycastHit potentialRightSender, 1.0f))
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
        foreach(AbstractBuilding receiver in Receivers)
        {
            receiver = null;
        }
        foreach(AbstractBuilding sender in Senders)
        {
            sender = null;
        }
    }

}
