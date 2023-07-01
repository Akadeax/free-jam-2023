using UnityEngine;

[System.Serializable]
public class ActionTimer
{
    public ActionTimer() { }

    public ActionTimer(float duration)
    {
        this.actionDuration = duration;
        this.actionCooldown = duration;
    }

    public ActionTimer(float duration, float cooldown)
    {
        this.actionDuration = duration;
        this.actionCooldown = cooldown;
    }

    public bool ActionDone => Elapsed > actionDuration;
    public bool ActionDoneThisFrame => ActionDone && Elapsed < (actionDuration - Time.deltaTime);


    public bool Ready => Elapsed > actionCooldown;
    public float ActionCompletion => Mathf.Clamp01(Elapsed / actionDuration);

    private float actionStartTimestamp;

    public float actionDuration;
    public float actionCooldown;
    public float Elapsed => Time.fixedTime - actionStartTimestamp;

    public void SetToDone()
    {
        actionStartTimestamp = Time.fixedTime - actionDuration;
    }

    public void SetToReady()
    {
        actionStartTimestamp = Time.fixedTime - actionCooldown;
    }

    public void Start()
    {
        actionStartTimestamp = Time.fixedTime;
    }
}
