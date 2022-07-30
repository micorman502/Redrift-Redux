using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemPickup {
    WorldItem[] GetItems();
    void Pickup ();
}
