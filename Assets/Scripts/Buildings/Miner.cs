using UnityEngine;          // For functionality.



// Brief description.
public class Miner : AbstractBuilding
{

    // ##### MEMBER VARIABLE OVERRIDES #####
    protected ItemSO miningResource = null;
    protected int outputID = -1;
    [SerializeField] protected GameObject RecipeScreenOriginal;
    private GameObject recipeScreen;


    // ##### METHODS #####

    // Building-unique Methods
    /**
      * @brief Creates an OutputResource from scratch.
      * @returns A boolean of whether or not the mine action succeeded.
      */
    protected bool Mine()
    {
        miningResource = tm.GetResourceAt(transform.position);
        if(miningResource != null && recipeScreen.TryGetComponent<SpriteRenderer>(out SpriteRenderer recipeSr))
            recipeSr.sprite = miningResource.Icon;
        if (miningResource != null && tm.DrillResourceAt(transform.position))
        {
            Outventory[miningResource.Id]++;
            return true;
        }
        return false;
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
            var canSend = Receivers.Count > 0 && Send(outputID, Receivers[0]);
            TogglePower(canMine || canSend);
            ResetProgress();
        }
    }

    // Unity Methods
    // @brief Runs on creation of a miner building. Used for assigning initial cooldown and attached buildings.
    private void Start()
    {
        tm = (TilemapChunk)FindAnyObjectByType(typeof(TilemapChunk));
        outputID = tm.GetResourceAt(transform.position).Id;
        Cooldown = 1.0f;
        Progress = 0.0f;
        IsRunning = true;
        ActTimer = Cooldown;
        recipeScreen = Instantiate(RecipeScreenOriginal, transform.position + (transform.right * -0.213f), transform.rotation * Quaternion.Euler(0f, 0f, 90f), transform);
        recipeScreen.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
        if(recipeScreen.TryGetComponent<SpriteRenderer>(out SpriteRenderer recipeSr))
            recipeSr.sortingOrder = 21;
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

}
