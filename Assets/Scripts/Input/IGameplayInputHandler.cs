using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGameplayInputHandler
{
    public void TakeMovementInput (Vector3 rawMovementAxes, bool accelerate, bool jump);
}
