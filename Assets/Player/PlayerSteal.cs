using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSteal : MonoBehaviour
{
    [Header("Assigned Refs")]
    [SerializeField] Material radialFillMaterial;
    [SerializeField] Transform minigameBase;
    [SerializeField] Transform minigamePin;

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
    [SerializeField] ActionTimer minigameTimer = new(1.5f, 5f);
    [SerializeField] float pinRotationDegPerSecond = 400f;
    [SerializeField] float minigameEndTime = 1f;

    [SerializeField] Vector2Int easyAngleSizeRange = new(90, 130);
    [SerializeField] Vector2Int mediumAngleSizeRange = new(40, 90);
    [SerializeField] Vector2Int hardAngleSizeRange = new(25, 40);

    // Variables
    float pinRotationDeg = 0f;
    bool hasCompletedMinigame = false;
    float currentMinigameEndTime = 0f;

    int minigameAngleStart = 0;
    int minigameAngleSize = 0;

    private void OnEnable()
    {
        player.input.OnInputSteal += Input_OnInputSteal;
    }

    private void OnDisable()
    {
        player.input.OnInputSteal -= Input_OnInputSteal;
    }


    private void Input_OnInputSteal()
    {
        if (!minigameTimer.ActionDone && player.State == PlayerState.Steal)
        {
            OnMinigameInput();
        }
        else if (minigameTimer.Ready)
        {
            player.State = PlayerState.Steal;
            anim.SwitchState("Steal");

            StartMinigame();
        }
    }

    void StartMinigame()
    {
        minigameTimer.Start();
        minigameBase.gameObject.SetActive(true);

        rb.velocity = Vector2.zero;

        minigameAngleStart = Random.Range(0, 360);
        minigameAngleSize = Random.Range(mediumAngleSizeRange.x, mediumAngleSizeRange.y);

        radialFillMaterial.SetFloat("_Angle", minigameAngleStart);
        radialFillMaterial.SetFloat("_Arc1", 360 - minigameAngleSize);
    }

    void OnMinigameInput()
    {
        hasCompletedMinigame = true;

        int currentRotation = (int)pinRotationDeg % 360;
        bool success = currentRotation > minigameAngleStart && currentRotation < minigameAngleStart + minigameAngleSize;
        // TODO: What does success mean?
    }


    private void Update()
    {
        if (player.State != PlayerState.Steal) return;

        if (!hasCompletedMinigame)
        {
            pinRotationDeg += pinRotationDegPerSecond * Time.deltaTime;
            minigamePin.rotation = Quaternion.Euler(0, 0, pinRotationDeg);
        }

        if (hasCompletedMinigame)
        {
            if (currentMinigameEndTime < minigameEndTime)
            {
                currentMinigameEndTime += Time.deltaTime;
            }
            else
            {
                TerminateMinigame();
            }
        }

        if (minigameTimer.ActionDone && !hasCompletedMinigame)
        {
            TerminateMinigame();
        }
    }


    void TerminateMinigame()
    {
        hasCompletedMinigame = false;
        currentMinigameEndTime = 0f;
        pinRotationDeg = 0;

        minigameBase.gameObject.SetActive(false);

        player.EnterMovement();
    }
}
