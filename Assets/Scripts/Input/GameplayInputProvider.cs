using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayInputProvider : MonoBehaviour
{
    public static GameplayInputProvider Instance;
    List<IGameplayInputHandler> inputHandlers = new List<IGameplayInputHandler>();
    bool jump;

    private void Awake ()
    {
        if (Instance)
        {
            Debug.Log($"An instance of {nameof(GameplayInputProvider)} already exists. Destroying this {nameof(GameplayInputProvider)}.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Update ()
    {
        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }
    }

    private void FixedUpdate ()
    {
        if (inputHandlers.Count <= 0)
            return;

        Vector3 movementAxes = new Vector3(Input.GetAxisRaw("Sideways"), Input.GetAxisRaw("Vertical"), Input.GetAxisRaw("Forward"));
        bool accelerate = Input.GetButton("Sprint");

        inputHandlers[inputHandlers.Count - 1].TakeMovementInput(movementAxes, accelerate, jump);

        jump = false;
    }

    public void AddOverrideHandler (IGameplayInputHandler handler, bool addAsFirst = false)
    {
        if (addAsFirst)
        {
            inputHandlers.Insert(0, handler);
            return;
        }

        inputHandlers.Add(handler);
    }

    public void RemoveOverrideHandler (IGameplayInputHandler handler)
    {
        inputHandlers.Remove(handler);
    }
}
