using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class PlayerEvents
{
    public static Action<bool> OnLockStateSet = delegate { };
    public static Action OnPlayerDeath = delegate { };
    public static Action RespawnPlayer = delegate { };
}
