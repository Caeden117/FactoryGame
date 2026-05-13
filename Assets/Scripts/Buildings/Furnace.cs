using UnityEngine;          // For functionality. 
using System;               // For WeakReference.



// Brief description.
public class Furnace : AbstractBuilding
{


    // ##### VIRTUAL MEMBER VARIABLE OVERRIDES #####


    // ##### METHODS #####


    // ##### Method Overrides #####
    

    /**
      * @brief The function called during the onUpdate() override.
      * @returns A boolean of whether or not the send action succeeded.
      */
    override protected bool Act()
    {
        return false;
    }

    // ##### Unity Methods #####

    void OnCreate()
    {
        
    }

    void OnDestroy()
    {
        
    }

}
