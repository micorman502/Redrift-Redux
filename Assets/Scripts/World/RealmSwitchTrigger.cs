using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealmSwitchTrigger : MonoBehaviour // give the object this is attached to the layer Player Trigger.
{
    static RealmSwitchTrigger currentSwitchTrigger; // effectively a "slot" for a RealmSwitchTrigger so that only one can be active at a time.

    [SerializeField] string realmName;
    [SerializeField] bool teleportPlayer = false;

    void OnTriggerEnter (Collider other)
    {
        SwitchOn(other);
    }

    void OnTriggerStay (Collider other)
    {
        SwitchOn(other);
    }

    void OnTriggerExit (Collider other)
    {
        SwitchOff(other, false);
    }

    void OnDestroy ()
    {
        SwitchOff(null, true);
    }

    void SwitchOn (Collider col)
    {
        if (col.gameObject != Player.GetPlayerObject())
            return;
        if (currentSwitchTrigger)
            return;

        currentSwitchTrigger = this;
        RealmTeleportManager.Instance.TeleportToRealm(realmName, teleportPlayer);
    }

    void SwitchOff (Collider col, bool overridePlayerCheck)
    {
        if (!overridePlayerCheck && col.gameObject != Player.GetPlayerObject())
            return;
        if (currentSwitchTrigger != this)
            return;

        currentSwitchTrigger = null;
    }
}
