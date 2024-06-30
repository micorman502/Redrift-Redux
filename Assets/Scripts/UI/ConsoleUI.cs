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
        if (commandStrings[0] == "spawn")
        {
            ParseSpawnCommand(commandStrings);
        }
        if (commandStrings[0] == "applyEffect")
        {
            ParseStatusEffectCommand(commandStrings);
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
        outputText.text += "spawn [name / ID] <position x y z>: Spawns any object from the Object Register, either in front of the player, or at a specified point.\n";
        outputText.text += "applyEffect [name / ID] [duration] <stackSize>: Applies a given status effect to the player.";
    }

    void ParseStatusEffectCommand (string[] commandStrings)
    {
        if (commandStrings.Length != 3 && commandStrings.Length != 4)
        {
            outputText.text = "Incorrect amount of arguments for applyEffect command: " + commandStrings.Length;
            return;
        }

        StatusEffect effect = ParseStatusEffect(commandStrings[1]);
        float duration = float.Parse(commandStrings[2]);
        int stackSize = 1;

        if (commandStrings.Length > 3)
        {
            stackSize = int.Parse(commandStrings[3]);
        }

        Player.GetPlayerObject().GetComponentInChildren<StatusEffectApplier>().ApplyStatusEffect(effect, duration, stackSize);

        outputText.text = $"Successfully applied status effect {effect.accessor}";
    }

    void ParseSpawnCommand (string[] commandStrings)
    {
        if (commandStrings.Length != 2 && commandStrings.Length != 5)
        {
            outputText.text = "Incorrect amount of arguments for spawn command: " + commandStrings.Length;
            return;
        }

        GameObject spawningObject = ParseSpawnObject(commandStrings[1]);
        Vector3 position = Vector3.zero;

        if (commandStrings.Length == 2)
        {
            Transform playerTransform = Player.GetPlayerObject().transform;

            if (Physics.Raycast(playerTransform.position, playerTransform.forward, out RaycastHit hit, 5f))
            {
                position = hit.point;
            }
            else
            {
                position = playerTransform.position + playerTransform.forward * 5f;
            }
        }

        if (commandStrings.Length == 5)
        {
            position = ParseVector3(commandStrings[2], commandStrings[3], commandStrings[4]);
        }

        Instantiate(spawningObject, position, Quaternion.identity);

        outputText.text = $"Successfully spawned object {spawningObject.name}";
    }

    void ParseGiveCommand (string[] commandStrings, bool parseExternalName = false)
    {
        if (commandStrings.Length < 2 || commandStrings.Length > 3)
        {
            outputText.text = "Incorrect amount of arguments for give command: " + commandStrings.Length;
            return;
        }

        Inventory inventory = Player.GetPlayerObject().GetComponent<PlayerInventory>().inventory;

        ItemInfo item = ParseItem(commandStrings[1], parseExternalName);
        int amount = commandStrings.Length > 2 ? int.Parse(commandStrings[2]) : 1;

        inventory.AddItem(item, amount);

        outputText.text = $"Added {amount} of Item {item.name}";
    }

    Vector3 ParseVector3 (string x, string y, string z)
    {
        return new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
    }

    StatusEffect ParseStatusEffect (string rawStatusEffectText)
    {
        if (int.TryParse(rawStatusEffectText, out int id))
        {
            return StatusEffectDatabase.GetStatusEffect(id);
        }

        return StatusEffectDatabase.GetStatusEffect(rawStatusEffectText);
    }

    GameObject ParseSpawnObject (string rawSpawnText)
    {
        if (int.TryParse(rawSpawnText, out int id))
        {
            return ObjectDatabase.GetObject(id);
        }

        return ObjectDatabase.GetObject(rawSpawnText);
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

        string itemText = ParseConjoinedString(rawItemText);

        return ItemDatabase.GetItemByExternalName(itemText);
    }

    string ParseConjoinedString (string input)
    {
        return input.Replace('_', ' ');
    }

#endif
}
