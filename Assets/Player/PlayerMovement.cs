using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Events
    public delegate void PlayerMoveTickEvent(Vector2 normedInput);
    public event PlayerMoveTickEvent OnPlayerMoveTick;

    // Acquired Refs
    Player player;
    Rigidbody2D rb;
    PixelAnimator anim;

    private void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<PixelAnimator>();
    }

    [Header("Fields")]
    [SerializeField] float moveSpeed = 5f;

    public void EnterMovement()
    {
        player.State = PlayerState.Movement;
        anim.SwitchState("Movement");
    }

    public void UpdateMovement()
    {
        Vector2 oldVelocity = rb.velocity;

        Vector2 normedInput = player.input.NormedMoveInput;

        Vector2 force = normedInput * moveSpeed - oldVelocity;
        rb.AddForce(force, ForceMode2D.Impulse);

        OnPlayerMoveTick?.Invoke(normedInput);
    }

    public IEnumerator Knockback(Vector2 direction, float force, float time)
    {
        float currentTimer = time;
        while (currentTimer > 0)
        {
            currentTimer -= Time.deltaTime;
            Vector2 velocity = Vector2.Lerp(direction * force, Vector2.zero, 1 - currentTimer / time);
            rb.velocity = velocity;
            yield return null;
        }
    }

}
