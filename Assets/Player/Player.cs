using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    Movement, Cast, Steal
}

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerInput))]
public class Player : MonoBehaviour
{
    // Events
    public delegate void PlayerStateChangeEvent(PlayerState newState);
    public event PlayerStateChangeEvent OnPlayerStateChange;

    [HideInInspector] public PlayerInput input;

    [SerializeField] PlayerState state;
    public PlayerState State
    {
        get => state;
        set
        {
            if (state == value) return;

            state = value;
            OnPlayerStateChange?.Invoke(value);
        }
    }

    PlayerMovement movement;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        movement = GetComponent<PlayerMovement>();
    }

    private void FixedUpdate()
    {
        switch(state)
        {
            case PlayerState.Movement:
                movement.UpdateMovement();
                break;
        }
    }

    public void EnterMovement()
    {
        movement.EnterMovement();
    }

    public void Knockback(Vector2 direction, float force, float time)
    {
        movement.StartCoroutine(movement.Knockback(direction, force, time));
    }
}
