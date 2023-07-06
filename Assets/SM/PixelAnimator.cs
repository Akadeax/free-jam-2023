using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(SpriteRenderer))]
public class PixelAnimator : MonoBehaviour
{
    [SerializeField] PixelAnimatorTemplate template;
    Dictionary<string, PixelAnimatorState> stateList = new();
    SpriteRenderer rend;

    PixelAnimatorState currentState;
    float timeSinceStateEnter = 0f;
    [HideInInspector] public float animationSpeed = 1f;

    PixelAnimationClip currentAnimation;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();

        if (template == null)
        {
            Debug.LogError("PixelAnimator does not have template.");
            return;
        }

        if (template.stateData.Count != 0)
        {
            // Copy all states
            foreach (PixelAnimatorStateData data in template.stateData)
            {
                stateList.Add(data.stateName, data.ConstructState(this));
            }

            currentState = stateList[template.stateData[0].stateName];
            currentState.OnEnter(this);
        }
    }

    private void Update()
    {
        if (currentState == null) return;
        
        currentState.OnUpdate(this);

        if (currentAnimation == null || currentAnimation.IsEmpty) return;

        timeSinceStateEnter += Time.deltaTime * animationSpeed;
        if(currentAnimation.isLooping && timeSinceStateEnter > currentAnimation.duration)
        {
            timeSinceStateEnter = 0f;
        }

        float singleSpriteDuration = currentAnimation.duration / currentAnimation.sprites.Count;
        int animationIndex = (int)Mathf.Clamp(timeSinceStateEnter / singleSpriteDuration, 0, currentAnimation.sprites.Count - 1);
        rend.sprite = currentAnimation.sprites[animationIndex];
    }

    public void SwitchState(string newState)
    {
        if (!stateList.ContainsKey(newState))
        {
            Debug.LogWarning($"Could not find state '{newState}'");
            return;
        }

        // Stop coroutines that were started in state
        StopAllCoroutines();

        currentState.OnExit(this);
        currentState = stateList[newState];
        timeSinceStateEnter = 0f;
        currentState.OnEnter(this);
    }

    public PixelAnimatorState GetState(string name)
    {
        return stateList[name];
    }

    public void SetCurrentAnimation(PixelAnimationClip4D clip, Vector2 dir)
    {
        if (clip == null) Debug.LogWarning($"Trying to set animation clip on {gameObject.name} to null");
        SetCurrentAnimation(clip.GetAppropriate(dir));
    }

    public void SetCurrentAnimation(PixelAnimationClip clip, bool resetStateTime = false)
    {
        if (resetStateTime) timeSinceStateEnter = 0f;

        if (clip == null) Debug.LogWarning($"Trying to set animation clip on {gameObject.name} to null");
        currentAnimation = clip;
    }
}
