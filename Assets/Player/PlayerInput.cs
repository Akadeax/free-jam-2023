using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    Vector2 normedMoveInput;
    public Vector2 NormedMoveInput => normedMoveInput;

    public delegate void InputAttackEvent();
    public event InputAttackEvent OnInputAttack;

    public delegate void InputCastEvent();
    public event InputCastEvent OnInputCast;

    public delegate void InputStealEvent();
    public event InputCastEvent OnInputSteal;

    private void Update()
    {
        normedMoveInput = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        ).normalized;

        if (Input.GetMouseButtonDown(0)) OnInputAttack?.Invoke();


        if (Input.GetKeyDown(KeyCode.Q)) OnInputCast?.Invoke();
        if (Input.GetKeyDown(KeyCode.E)) OnInputSteal?.Invoke();
    }
}
