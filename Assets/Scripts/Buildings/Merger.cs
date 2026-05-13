using UnityEngine;          // For functionality. 
using System;               // For WeakReference.



// Brief description.
public class Merger : AbstractBuilding
{


    // ##### MEMBER VARIABLE OVERRIDES #####
    public AbstractBuilding LeftSender { get; set; } = null;  // The building sending outputs to this building from this building's left. null when empty or deleted.
    public AbstractBuilding RightSender { get; set; } = null; // The building receiving outputs from this building. null when empty or deleted.

    // ##### METHODS #####


    // ##### Method Overrides #####
    

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
