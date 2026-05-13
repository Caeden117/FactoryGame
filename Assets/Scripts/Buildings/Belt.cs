using UnityEngine;          // For functionality. 
using System;               // For WeakReference.



// Brief description.
public class Belt : AbstractBuilding
{


    // ##### VIRTUAL MEMBER VARIABLE OVERRIDES #####


    // ##### METHODS #####


    // ##### Method Overrides #####
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
        return;
    }

    // ##### Unity Methods #####

    void OnCreate()
    {
        
    }

    void OnDestroy()
    {
        
    }

}
