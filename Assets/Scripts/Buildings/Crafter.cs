using UnityEngine;          // For functionality.



// Brief description.
public class Crafter : AbstractBuilding
{

    // ##### MEMBER VARIABLE OVERRIDES #####
    protected override float Cooldown { get; set; } = 2.0f;
    protected override float Progress { get; set; } = 0.0f;
    protected override bool IsRunning { get; set; } = false;
    protected RecipeSO recipe = null;


    // ##### METHODS #####

    // Building-unique Methods
    /**
      * @brief Checks that a recipe is provided and that it can afford to function and provide full output.
      * @returns A boolean of whether or not the recipe could be fulfilled effectively.
      */
    protected bool RecipeCheck()
    {
        // Edge Case Handling: No recipe set.
        if(recipe == null)
        {
            return false;
        }
        // Checking that sufficient ingredients are stored.
        foreach (RecipeSO.Ingredient ingredient in recipe.Ingredients)
        {
            if(Inventory[ingredient.Item.Id] < ingredient.Amount)
            {
                return false;
            }
        }
        // Checking that there is room for the output.
        foreach(RecipeSO.Output output in recipe.Outputs)
        {
            if(MaxStackSize < Outventory[output.Item.Id] + output.Amount)
            {
                return false;
            }
        }
        return true;
    }

    /**
      * @brief Converts ingredients into outputs according to recipe.
      * @returns A boolean of whether or not the smelt action succeeded.
      */
    protected bool Craft()
    {
        if(RecipeCheck())
        // Decreasing ingredient stores according to recipe.
        foreach (RecipeSO.Ingredient ingredient in recipe.Ingredients)
        {
            Inventory[ingredient.Item.Id] -= ingredient.Amount;
        }
        // Increasing output stores according to recipe.
        foreach(RecipeSO.Output output in recipe.Outputs)
        {
            Outventory[output.Item.Id] += output.Amount;
        }
        return true;
    }

    // Method Overrides
    /**
      * @brief The function called during the onUpdate() override.
      * @returns A boolean of whether or not the send action succeeded.
      */
    override internal void Act()
    {
        ActTimer -= Time.deltaTime;
        if(ActTimer <= 0)
        {
            TogglePower(Craft());
            ResetProgress();
        }
        return;
    }

    // Unity Methods
    // @brief Runs on creation of a crafter building. Used for assigning initial cooldown and attached buildings.
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

    // Runs on deletion of a crafter building. Used for manual garbage collection.
    void OnDestroy()
    {
        
    }

}
