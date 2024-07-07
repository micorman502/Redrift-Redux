using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectUI : MonoBehaviour
{
    [SerializeField] Transform statusEffectHolder;
    [SerializeField] GameObject statusEffectPrefab;
    List<StatusEffectListItem> statusEffectListItems = new List<StatusEffectListItem>();
    bool subscribed;
    StatusEffectApplier target;

    void Start ()
    {
        target = Player.GetPlayerObject().GetComponentInChildren<StatusEffectApplier>();

        Subscribe();
    }

    private void OnDestroy ()
    {
        Unsubscribe();
    }

    void Subscribe ()
    {
        if (subscribed)
            return;

        subscribed = true;

        target.OnStatusEffectAdded += OnStatusEffectAdded;
        target.OnStatusEffectRemoved += OnStatusEffectRemoved;
        target.OnStatusEffectUpdated += OnStatusEffectUpdated;

        for (int i = 0; i < target.statusEffects.Count; i++)
        {
            OnStatusEffectAdded(target.statusEffects[i].GetStatusEffect(), target.statusEffects[i].GetMaxDuration());
        }
    }

    void Unsubscribe ()
    {
        if (!subscribed)
            return;

        ClearStatusEffects();

        subscribed = false;

        target.OnStatusEffectAdded -= OnStatusEffectAdded;
        target.OnStatusEffectRemoved -= OnStatusEffectRemoved;
        target.OnStatusEffectUpdated -= OnStatusEffectUpdated;
    }

    void OnStatusEffectAdded (StatusEffect effect, float duration)
    {
        StatusEffectListItem listItem = Instantiate(statusEffectPrefab, statusEffectHolder).GetComponent<StatusEffectListItem>();

        listItem.Setup(effect, 1, duration);
        statusEffectListItems.Add(listItem);
    }

    void OnStatusEffectRemoved (StatusEffect effect)
    {
        for (int i = 0; i < statusEffectListItems.Count; i++)
        {
            StatusEffectListItem listItem = statusEffectListItems[i];
            if (effect == listItem.GetStatusEffect())
            {
                Destroy(listItem.gameObject);
                statusEffectListItems.RemoveAt(i);
                break;
            }
        }
    }

    void OnStatusEffectUpdated (StatusEffect effect, float duration, int stack)
    {
        for (int i = 0; i < statusEffectListItems.Count; i++)
        {
            StatusEffectListItem listItem = statusEffectListItems[i];
            if (effect == listItem.GetStatusEffect())
            {
                listItem.SetDuration(duration);
                listItem.SetStack(stack);
                break;
            }
        }
    }

    void ClearStatusEffects ()
    {
        for (int i = statusEffectListItems.Count - 1; i >= 0; i--)
        {
            StatusEffectListItem listItem = statusEffectListItems[i];

            Destroy(listItem.gameObject);
            statusEffectListItems.RemoveAt(i);
        }
    }
}
