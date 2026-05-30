using UnityEngine;          // For functionality.



// Brief description.
public class Crafter : AbstractBuilding
{

    // ##### MEMBER VARIABLE OVERRIDES #####
    [SerializeField] private RecipeListSO recipeList;
    private RecipeSO recipe;
    [SerializeField] protected GameObject RecipeScreenOriginal;
    private GameObject recipeScreen;


    // ##### METHODS #####

    // Building-unique Methods
    /**
      * @brief Checks that a recipe is provided and that it can afford to function and provide full output.
      * @returns A boolean of whether or not the recipe could be fulfilled effectively.
      */
    protected bool RecipeCheck()
    {
        // Edge Case Handling: No recipe set.
        if (recipe == null)
        {
            return false;
        }
        // Checking that sufficient ingredients are stored.
        foreach (RecipeSO.Ingredient ingredient in recipe.Ingredients)
        {
            if (Inventory[ingredient.Item.Id] < ingredient.Amount)
            {
                return false;
            }
        }
        // Checking that there is room for the output.
        foreach (RecipeSO.Output output in recipe.Outputs)
        {
            if (MaxStackSize < Outventory[output.Item.Id] + output.Amount)
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
        if (RecipeCheck())
        {
            // Decreasing ingredient stores according to recipe.
            foreach (RecipeSO.Ingredient ingredient in recipe.Ingredients)
            {
                Inventory[ingredient.Item.Id] -= ingredient.Amount;
            }
            // Increasing output stores according to recipe.
            foreach (RecipeSO.Output output in recipe.Outputs)
            {
                Outventory[output.Item.Id] += output.Amount;
            }
            return true;
        }
        return false;
    }

    // Method Overrides
    /**
      * @brief The function called during the onUpdate() override.
      * @returns A boolean of whether or not the send action succeeded.
      */
    override internal void Act()
    {
        ActTimer -= Time.deltaTime;
        if (ActTimer <= 0 && recipe != null)
        {
            bool canCraft = Craft();
            bool canSend = false;
            foreach (RecipeSO.Output output in recipe.Outputs)
            {
                if(0 < Outventory[output.Item.Id])
                {
                    if(Receivers.Count > 0 && Send(output.Item.Id, Receivers[0]))
                    {
                        canSend = true;
                        break;
                    }
                }
            }
            TogglePower(canCraft || canSend);
            ResetProgress();
        }
        return;
    }

    // Unity Methods
    // @brief Runs on creation of a crafter building. Used for assigning initial cooldown and attached buildings.
    private void Start()
    {
        IsRunning = false;
        recipeScreen = Instantiate(RecipeScreenOriginal, transform.position, transform.rotation);

        // Attempt to attach to Receiver building.
        if (Physics.Raycast(transform.position, transform.right, out RaycastHit potentialReceiver, ConnectionRange))
        {
            if (potentialReceiver.transform.gameObject.TryGetComponent(out AbstractBuilding toBeReceiver))
            {
                Receivers.Add(toBeReceiver);
                toBeReceiver.Senders.Add(this);
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

    private void OnMouseUp() 
    {
        RecipeAssignmentUI.Open(nameof(Crafter), recipeList, (selectedRecipe) =>
        {
            // Update recipe properties to match assigned recipe.
            recipe = selectedRecipe;
            Cooldown = recipe.CraftingTime;
            Progress = 0.0f;
            IsRunning = false;
            ActTimer = Cooldown;
            if(recipeScreen.TryGetComponent<SpriteRenderer>(out SpriteRenderer recipeSr))
            recipeSr.sprite = recipe.Outputs[0].Item.Icon;


            // Manually reset and re-assign accepted resources (the recipe can be assigned at any time)
            for (var i = 0; i < AcceptedResources.Length; i++)
            {
                AcceptedResources[i] = false;
            }
            foreach (RecipeSO.Ingredient ingredient in recipe.Ingredients)
            { 
                AcceptedResources[ingredient.Item.Id] = true;
            }
        });
    }
}
