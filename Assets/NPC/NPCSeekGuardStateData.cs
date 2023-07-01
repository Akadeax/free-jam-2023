using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "SM/NPC/SeekGuard")]
public class NPCSeekGuardStateData : PixelAnimatorStateData
{
    public float guardTargetDistanceLeniency = 1f;

    [Header("Animations")]
    public PixelAnimationClip4D idleAnim;
    public PixelAnimationClip4D walkingAnim;

    public override PixelAnimatorState ConstructState(PixelAnimator animator)
    {
        return new NPCSeekGuardState(stateName, this, animator);
    }
}

public class NPCSeekGuardState : PixelAnimatorState
{
    new NPCSeekGuardStateData data;
    PixelAnimator animator;
    Rigidbody2D rb;
    NPCData npc;

    NPCData targetGuard = null;
    bool reachedTarget = false;

    Vector2 seekStartPosition;

    public NPCSeekGuardState(string name, PixelAnimatorStateData data, PixelAnimator animator) : base(name, data)
    {
        this.data = data as NPCSeekGuardStateData;
        this.animator = animator;

        rb = animator.GetComponent<Rigidbody2D>();
        npc = animator.GetComponent<NPCData>();
    }

    public override void OnEnter(PixelAnimator animator)
    {
        seekStartPosition = rb.transform.position;

        NPCData[] npcList = Object.FindObjectsOfType<NPCData>();

        // find closest guard
        float closestGuardDistance = Mathf.Infinity;
        NPCData closestGuard = null;
        for (int i = 0; i < npcList.Length; i++)
        {
            if (npcList[i].Type != NPCData.NPCType.Guard) continue;

            float currentDistance = Vector2.Distance(rb.transform.position, npcList[i].transform.position);
            if (currentDistance < closestGuardDistance)
            {
                closestGuard = npcList[i];
                closestGuardDistance = currentDistance;
            }
        }

        reachedTarget = false;
        targetGuard = closestGuard;

        npc.CreateNewPath(targetGuard.transform.position);
        npc.FollowCurrentPath = true;

        npc.SetHeadDisplay(NPCData.HeadDisplay.alert);
    }

    public override void OnUpdate(PixelAnimator animator)
    {
        if (reachedTarget) return;

        float distanceToGuard = Vector2.Distance(rb.transform.position, targetGuard.transform.position);
        float distanceToPathEnd = Vector2.Distance(rb.transform.position, npc.CurrentPathTarget);

        if (targetGuard.IsBusy)
        {
            animator.SwitchState("Patrol");
            return;
        }

        if (distanceToGuard < data.guardTargetDistanceLeniency)
        {
            reachedTarget = true;
            animator.StartCoroutine(ReachedTargetGuard());
        }
        else if (distanceToPathEnd < data.guardTargetDistanceLeniency)
        {
            npc.CreateNewPath(targetGuard.transform.position);
        }
        else
        {
            animator.SetCurrentAnimation(data.walkingAnim, rb.velocity);
        }
    }

    IEnumerator ReachedTargetGuard()
    {
        targetGuard.IsBusy = true;

        rb.velocity = Vector2.zero;
        npc.FollowCurrentPath = false;

        animator.SetCurrentAnimation(data.idleAnim, npc.FacingDir);
        // make guard face approaching NPC
        Vector2 dirToGuard = (targetGuard.transform.position - rb.transform.position).normalized;
        targetGuard.SetFacingDir(-dirToGuard);
        targetGuard.FollowCurrentPath = false;

        npc.SetHeadDisplay(NPCData.HeadDisplay.speak);

        yield return new WaitForSeconds(2f);

        npc.SetHeadDisplay(NPCData.HeadDisplay.none);

        // path both NPC and guard to location
        targetGuard.CreateNewPath(seekStartPosition);
        npc.CreateNewPath(seekStartPosition);

        targetGuard.GetComponent<PixelAnimator>().SwitchState("SeekPlayer");
        animator.SwitchState("SeekPlayer");
    }
}
