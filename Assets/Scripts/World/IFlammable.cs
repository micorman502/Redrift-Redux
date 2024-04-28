using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFlammable
{
    public void Ignite (float ignitionStrength);
    public void Extinguish (float extinguishStrength);
}
