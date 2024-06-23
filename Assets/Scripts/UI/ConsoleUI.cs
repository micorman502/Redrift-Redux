using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleUI : MonoBehaviour
{
#if UNITY_EDITOR
    [SerializeField] TMP_InputField inputField;
    [SerializeField] TMP_Text outputText;

    public void OnTextEntered (string text)
    {
        ParseText(text);

        inputField.SetTextWithoutNotify("");
    }

    void ParseText (string text)
    {
        string[] commandStrings = text.Split(' ');

        if (commandStrings[0] == "give")
        {
            ParseGiveCommand(commandStrings);
        }
        if (commandStrings[0] == "giveEx")
        {
            ParseGiveCommand(commandStrings, true);
        }

        if (commandStrings[0] == "help")
        {
            ParseHelpCommand(commandStrings);
        }
    }

    void ParseHelpCommand (string[] commandStrings)
    {
        outputText.text = "Syntax: [] is a required parameter, <> is an optional parameter.\n";
        outputText.text += "give [name / ID] <amount>: Puts the requested item and amount into your inventory, using the item's internal name.\n";
        outputText.text += "giveEx [itemName / ID] <amount>: Puts the requested item and amount into your inventory, using the item's in-game name.\n";
    }

    void ParseGiveCommand (string[] commandStrings, bool parseExternalName = false)
    {
        if (commandStrings.Length < 2 || commandStrings.Length > 3)
        {
            Debug.LogWarning("Incorrect amount of arguments for give command: " + commandStrings.Length);
            return;
        }

        Inventory inventory = Player.GetPlayerObject().GetComponent<PlayerInventory>().inventory;

        ItemInfo item = ParseItem(commandStrings[1], parseExternalName);
        int amount = commandStrings.Length > 2 ? int.Parse(commandStrings[2]) : 1;

        inventory.AddItem(item, amount);

        outputText.text = $"Added {amount} of Item {item.name}";
    }

    ItemInfo ParseItem (string rawItemText, bool parseExternalName = false)
    {
        if (int.TryParse(rawItemText, out int id))
        {
            return ItemDatabase.GetItem(id);
        }

        if (!parseExternalName)
        {
            return ItemDatabase.GetItemByInternalName(rawItemText);
        }

        string itemText = rawItemText.Replace('_', ' ');

        return ItemDatabase.GetItemByExternalName(itemText);
    }

#endif
}
