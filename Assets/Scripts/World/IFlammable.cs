using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFlammable
{
    public void Ignite (int ignitionStrength);
    public void Extinguish (int extinguishStrength);
}
