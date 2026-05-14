using UnityEngine;          // For functionality.



// Brief description.
public class Splitter : AbstractBuilding
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
    // @brief Runs on creation of a splitter building. Used for assigning initial cooldown and attached buildings.
    void OnCreate()
    {
        ActTimer = Cooldown;
        // Attempt to attach to Receiver building left of merger.
        if (Physics.Raycast(transform.position, -transform.right, out RaycastHit potentialLeftReceiver, 1.0f))
        {
            if (potentialLeftReceiver.transform.gameObject.TryGetComponent(out AbstractBuilding toBeLeftReceiver))
            {
                Receivers.Add(toBeLeftReceiver);
                toBeLeftReceiver.Senders.Add(this);
            }
        }
        // Attempt to attach to Receiver building in front of merger.
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit potentialReceiver, 1.0f))
        {
            if (potentialReceiver.transform.gameObject.TryGetComponent(out AbstractBuilding toBeReceiver))
            {
                Receivers.Add(toBeReceiver);
                toBeReceiver.Senders.Add(this);
            }
        }
        // Attempt to attach to Receiver building right of merger.
        if (Physics.Raycast(transform.position, transform.right, out RaycastHit potentialRightReceiver, 1.0f))
        {
            if (potentialRightReceiver.transform.gameObject.TryGetComponent(out AbstractBuilding toBeRightReceiver))
            {
                Receivers.Add(toBeRightReceiver);
                toBeRightReceiver.Senders.Add(this);
            }
        }
        // Attempt to attach to Sender building.
        if (Physics.Raycast(transform.position, -transform.forward, out RaycastHit potentialSender, 1.0f))
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
