using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class RecipeManager : MonoBehaviour {

	public enum Categories { All, Construction, Resources, Tools, Automation, Decoration };

	[SerializeField] Sprite[] categoryIcons;
	[SerializeField] GameObject categoriesContainer;
	[SerializeField] GameObject categoryButtonPrefab;

	public Transform craftingRecipeContainer;
	public RectTransform craftingScrollRectTransform;
	ScrollRect craftingScrollRect;

	public GameObject craftingRecipePrefab;

	Inventory inventory;

	void Awake() {
		craftingScrollRect = craftingScrollRectTransform.GetComponent<ScrollRect>();
	}

	void Start() {
		inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>().inventory;
		InitializeCategories();
		InitializeRecipes(Categories.All);
	}

    void OnEnable()
    {
		InventoryEvents.ConstructItem += ConstructRecipe;
    }

    void OnDisable()
    {
		InventoryEvents.ConstructItem -= ConstructRecipe;
	}

    public void InitializeRecipes(Categories c) {
		ClearRecipes();
		int i = 0;

		foreach (Recipe recipe in RecipeDatabase.GetAllRecipes()) {
			if(recipe.categories.Contains(c) || c == 0) {
				GameObject recipeObj = Instantiate(craftingRecipePrefab, craftingRecipeContainer);
				recipeObj.GetComponent<RecipeListItem>().Setup(recipe, inventory);

				RectTransform rectTransform = recipeObj.GetComponent<RectTransform>();
				Vector3 tempSize = rectTransform.sizeDelta;
				tempSize.y = Mathf.Ceil(recipe.inputs.Length / 3f) * 100f;
				rectTransform.sizeDelta = tempSize;
				craftingScrollRect.verticalScrollbar.value = 1f;
			}
			i++;
		}
	}

	public void ConstructRecipe(Recipe recipe)
	{

		int[] inputAmounts = new int[recipe.inputs.Length];

		if (!recipe.IsCraftable(inventory) || inventory.SpaceLeftForItem(recipe.output) < recipe.output.amount)
		{
			return;
		}

		int o = 0; //COME BACK
		foreach (InventorySlot invItem in inventory.Slots)
		{
			if (invItem.Item)
			{ // If the inventory slot has an item in it
				int i = 0;
				foreach (WorldItem item in recipe.inputs)
				{ // Loop through all the ingredients in the recipe to see if the slot's item is the same as one of them
					if (invItem.Item == item.item)
					{ // Is the item the same?
						int amountToDecrease = Mathf.Max(0, item.amount - inputAmounts[i]);
						inputAmounts[i] += invItem.Count; // Add the amount of that item to the inputAmounts
						inventory.RemoveItem(new WorldItem(item.item, amountToDecrease));
						break;
					}
					i++;
				}
			}
			o++;
		}

		inventory.AddItem(recipe.output);

		int r = 0;
		foreach (WorldItem replacementItem in recipe.replacedItems)
		{
			inventory.AddItem(replacementItem);
			r++;
		}
	}

	void ClearRecipes() {
		foreach(Transform recipeTrans in craftingRecipeContainer) {
			if(recipeTrans.GetComponent<RecipeListItem>()) {
				Destroy(recipeTrans.gameObject);
			}
		}
	}

	void InitializeCategories() {
		int numCategories = System.Enum.GetNames(typeof(Categories)).Length;
		string[] categoryNames = System.Enum.GetNames(typeof(Categories));
		for(int i = 0; i <= numCategories - 1; i++) {
			int recipeToInitialize = i;
			GameObject obj = Instantiate(categoryButtonPrefab, categoriesContainer.transform);
			obj.GetComponent<Button>().onClick.AddListener(delegate { InitializeRecipes((Categories)recipeToInitialize); });
			obj.GetComponentInChildren<Text>().text = categoryNames[i];
			obj.GetComponentsInChildren<Image>()[1].sprite = categoryIcons[i];
		}
	}
}
