using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCast : MonoBehaviour
{
    // Events
    public delegate void PlayerCastEvent();
    public event PlayerCastEvent OnPlayerCast;

    // Acquired Refs
    Player player;
    Rigidbody2D rb;
    PixelAnimator anim;

    [Header("Fields")]
    [SerializeField] ActionTimer castTimer = new(0.5f, 1f);


    private void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<PixelAnimator>();
    }

    private void OnEnable()
    {
        player.input.OnInputCast += Input_OnInputCast;
    }
    private void OnDisable()
    {
        player.input.OnInputCast -= Input_OnInputCast;
    }

    private void Input_OnInputCast()
    {
        if (player.State != PlayerState.Movement) return;
        if (!castTimer.Ready) return;

        player.State = PlayerState.Cast;
        anim.SwitchState("Cast");

        OnPlayerCast?.Invoke();

        rb.velocity = Vector2.zero;
        castTimer.Start();
    }

    private void Update()
    {
        if (player.State == PlayerState.Cast && castTimer.ActionDone)
        {
            player.EnterMovement();
        }
    }
}
