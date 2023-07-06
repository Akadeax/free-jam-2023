using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerState
{
    Movement, Cast, Steal, Dead
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

    public void SetDead()
    {
        if (dying == false)
        {
            state = PlayerState.Dead;
            dying = true;
            StartCoroutine(_SetDead());
        }
    }

    public PixelAnimationClip idle;
    public PixelAnimationClip death;
    public SpriteRenderer fullBlack;

    bool dying = false;

    IEnumerator _SetDead()
    {
        if (MenuHandler.instance) Destroy(MenuHandler.instance.gameObject);

        GetComponent<Rigidbody2D>().isKinematic = true;
        GetComponent<SpriteRenderer>().sortingOrder = 1000;
        GetComponent<PixelAnimator>().SwitchState("Caught");
        fullBlack.enabled = true;

        GetComponent<PlayerSteal>().enabled = false;
        GetComponent<PlayerCast>().enabled = false;
        GetComponent<PlayerMovement>().enabled = false;

        yield return new WaitForSeconds(1);

        GetComponent<PixelAnimator>().SetCurrentAnimation(death, true);

        yield return new WaitForSeconds(1.5f);

        PlayerPrefs.SetInt("highscore", GameUIHandler.Instance.CurrentScore);
        SceneManager.LoadScene(0);
    }
}
