using System;
using UnityEngine;
using UnityEngine.UIElements;

public class RecipeAssignmentUI : MonoBehaviour
{
    // YES THIS IS A SMELLY PATTERN but this UI really is a singleton anyways
    private static RecipeAssignmentUI instance;

    [SerializeField] private UIDocument mainDocument;
    [SerializeField] private VisualTreeAsset recipeDisplayTemplate;
    [SerializeField] private VisualTreeAsset itemDisplayTemplate;

    [SerializeField] private RecipeListSO debugRecipeList;

    private VisualElement background;
    private ListView recipeListView;
    private Label assignmentLabel;

    private void Start()
    {
        instance = this;
        var root = mainDocument.rootVisualElement;
        recipeListView = root.Q<ListView>("RecipeList");
        assignmentLabel = root.Q<Label>("AssignLabel");
        background = root.Q<VisualElement>("Background");

        // Assign how the recipe list is created and displayed;
        recipeListView.makeItem = () => recipeDisplayTemplate.CloneTree();

        if (debugRecipeList != null)
        {
            OpenRecipeAssignUI("Debug", debugRecipeList, null);
        }
        else
        {
            background.visible = false;
        }
    }

    public static void Open(string machineName, RecipeListSO recipeList, Action<RecipeSO> onSelectedRecipe)
    {
        if (instance == null)
        {
            throw new InvalidOperationException("RecipeAssignmentUI instance does not exist. Please add one to the scene.");
        }

        Debug.Log($"Opening ui for {machineName} with list {recipeList}");
        instance.OpenRecipeAssignUI(machineName, recipeList, onSelectedRecipe);
    }

    public void OpenRecipeAssignUI(string machineName, RecipeListSO recipeList, Action<RecipeSO> onSelectedRecipe)
    {
        if (recipeList == null)
        {
            throw new InvalidOperationException("Recipe List is not assigned. Please ensure that a valid RecipeListSO instance is passed through.");
        }

        background.visible = true;
        assignmentLabel.text = $"Assigning recipe for {machineName}";
        recipeListView.itemsSource = recipeList.SupportedRecipes;
        
        // This binding method is really long because we have a lot of initialization work to do, so new method it is
        recipeListView.bindItem = (element, index) => CreateRecipeDisplay(recipeList, onSelectedRecipe, element, index);

        recipeListView.RefreshItems();
    }

    private void CreateRecipeDisplay(RecipeListSO recipeList, Action<RecipeSO> onSelectedRecipe, VisualElement element, int index)
    {
        var recipe = recipeList.SupportedRecipes[index];
        var inputListView = element.Q<ListView>("InputItems");
        var outputListView = element.Q<ListView>("OutputItems");
        var button = element.Q<Button>("SelectButton");
        var craftingTime = element.Q<Label>("CraftingTime");

        // Update crafting time (simplest to do)
        craftingTime.text = $"{recipe.CraftingTime:F2} sec.";

        // Clicking button = assigning recipe, notify to user.
        button.clicked += () =>
        {    
            var selectedRecipe = recipeList.SupportedRecipes[index];
            Debug.Log($"Selected {selectedRecipe.name}");
            onSelectedRecipe?.Invoke(selectedRecipe);
            background.visible = false;
        };

        // Need to populate the input/output lists with recipe ins/outs
        inputListView.makeItem = () => itemDisplayTemplate.CloneTree();
        outputListView.makeItem = () => itemDisplayTemplate.CloneTree();

        // These binding methods are effectively identical, just grab ingredient/output and populate the item display
        inputListView.bindItem = (inElement, inIdx) =>
        {
            var item = recipe.Ingredients[inIdx];
            CreateItemDisplay(item.Item, item.Amount, inElement);
        };
        outputListView.bindItem = (outElement, outIdx) =>
        {
            var item = recipe.Outputs[outIdx];
            CreateItemDisplay(item.Item, item.Amount, outElement);
        };

        // Assign data sources to our recipe ingredients/outputs and refresh the list for display.
        inputListView.itemsSource = recipe.Ingredients;
        outputListView.itemsSource = recipe.Outputs;
        inputListView.RefreshItems();
        outputListView.RefreshItems();
    }

    private void CreateItemDisplay(ItemSO item, int amount, VisualElement element)
    {
        var itemSprite = element.Q<Image>("ItemSprite");
        var itemName = element.Q<Label>("ItemName");
        var itemQuantity = element.Q<Label>("ItemQuantity");

        itemSprite.sprite = item.Icon;
        itemName.text = item.DisplayName;
        itemQuantity.text = amount.ToString();
    }
}
