using UnityEngine;          // For functionality.



// Brief description.
public class Furnace : AbstractBuilding
{

    // ##### MEMBER VARIABLE OVERRIDES #####
    protected RecipeSO Recipe = null;


    // ##### METHODS #####

    // Building-unique Methods
    /**
      * @brief Checks that a recipe is provided and that it can afford to function and provide full output.
      * @returns A boolean of whether or not the recipe could be fulfilled effectively.
      */
    protected bool RecipeCheck()
    {
        // Edge Case Handling: No recipe set.
        if(Recipe == null)
        {
            return false;
        }
        // Checking that sufficient ingredients are stored.
        foreach (var ingredient in Recipe.Ingredients)
        {
            if(Inventory[ingredient.Item.Id] < ingredient.Amount)
            {
                return false;
            }
        }
        // Checking that there is room for the output.
        foreach(var output in Recipe.Outputs)
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
    protected bool Smelt()
    {
        if(RecipeCheck())
        // Decreasing ingredient stores according to recipe.
        foreach (var ingredient in Recipe.Ingredients)
        {
            Inventory[ingredient.Item.Id] -= ingredient.Amount;
        }
        // Increasing output stores according to recipe.
        foreach(var output in Recipe.Outputs)
        {
            Outventory[output.Item.Id] += output.Amount;
        }
        return true;
    }

    // Method Overrides
    /**
      * @brief The function called during the onUpdate() override.
      */
    internal override void Act()
    {
        ActTimer -= Time.deltaTime;
        if(ActTimer <= 0)
        {
            TogglePower(Smelt());
            ResetProgress();
        }
        return;
    }

    // Unity Methods 
    // @brief Runs on creation of a furnace building. Used for assigning initial cooldown and attached buildings.
    private void Start()
    {
        OnCreate();
    }

    private void OnCreate()
    {
        Cooldown = 2.0f;
        Progress = 0.0f;
        IsRunning = false;
        ActTimer = Cooldown;
        // Attempt to attach to Receiver building.
        if (Physics.Raycast(transform.position, transform.right, out var potentialReceiver, ConnectionRange))
        {
            if (potentialReceiver.transform.gameObject.TryGetComponent(out AbstractBuilding toBeReceiver))
            {
                Receivers.Add(toBeReceiver);
                toBeReceiver.Senders.Add(this);
            }
        }
        // Attempt to attach to Sender building.
        if (Physics.Raycast(transform.position, -transform.right, out var potentialSender, ConnectionRange))
        {
            if (potentialSender.transform.gameObject.TryGetComponent(out AbstractBuilding toBeSender))
            {
                Senders.Add(toBeSender);
                toBeSender.Receivers.Add(this);
            }
        }
    }

}
