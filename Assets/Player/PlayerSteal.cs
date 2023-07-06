using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSteal : MonoBehaviour
{
    // Events
    public delegate void PlayerMinigameFinishEvent(bool success);
    public event PlayerMinigameFinishEvent OnPlayerMinigameFinish;

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
    [SerializeField] float stealRange = 1.5f;

    [SerializeField] ActionTimer minigameTimer = new(1.5f, 5f);
    [SerializeField] float pinRotationDegPerSecond = 400f;
    [SerializeField] float minigameEndTime = 1f;

    [SerializeField] int minigameLeniency = 3;
    public Vector2Int angleSizeRange = new(90, 130);

    // Variables
    float pinRotationDeg = 0f;
    bool hasCompletedMinigame = false;
    float currentMinigameEndTime = 0f;

    int minigameAngleStart = 0;
    int minigameAngleSize = 0;

    NPCData stealingNPC = null;

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
        else if (minigameTimer.Ready && stealingNPC != null)
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
        minigameAngleSize = Random.Range(angleSizeRange.x, angleSizeRange.y);

        radialFillMaterial.SetFloat("_Angle", minigameAngleStart);
        radialFillMaterial.SetFloat("_Arc1", 360 - minigameAngleSize);

        stealingNPC.FollowCurrentPath = false;
    }

    void OnMinigameInput()
    {
        hasCompletedMinigame = true;

        int currentRotation = (int)pinRotationDeg % 360;
        //Debug.Log($"target has to be [{minigameAngleStart - minigameLeniency}; {(minigameAngleStart + minigameAngleSize + minigameLeniency)}], and is: {currentRotation}");
        bool success = currentRotation > (minigameAngleStart - minigameLeniency) && currentRotation < (minigameAngleStart + minigameAngleSize + minigameLeniency);

        stealingNPC.FollowCurrentPath = true;

        OnPlayerMinigameFinish?.Invoke(success);

        if (success)
        {
            GameUIHandler.Instance.IncreaseScore();
            stealingNPC.SetStealDisplay(NPCData.StealDisplay.green);
        }
        else
        {
            stealingNPC.SetStealDisplay(NPCData.StealDisplay.red);
            stealingNPC.SuspiciousBarAmount = 1f;
        }
    }


    private void Update()
    {
        if (player.State != PlayerState.Steal)
        {
            NPCData[] npcList = FindObjectsOfType<NPCData>();

            float closestDistance = Mathf.Infinity;
            NPCData closestNPC = null;

            foreach (NPCData npc in npcList)
            {
                npc.SetStealDisplay(NPCData.StealDisplay.none);

                float distance = Vector2.Distance(rb.transform.position, npc.transform.position);
                if (distance < stealRange && distance < closestDistance && npc.SuspiciousBarAmount < 0.75f && !npc.IsBusy && npc.Type != NPCData.NPCType.Guard)
                {
                    closestNPC = npc;
                    closestDistance = distance;
                }
            }

            if (closestNPC)
            {
                closestNPC.SetStealDisplay(NPCData.StealDisplay.normal);
            }

            stealingNPC = closestNPC;

            return;
        }

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
