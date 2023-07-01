using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class NPCData : MonoBehaviour
{

    [Header("Fields")]

    [SerializeField] List<Transform> patrolPoints;
    public List<Transform> PatrolPoints => patrolPoints;
    
    [SerializeField] float pathPointDistanceLeniency = 0.2f;
    public float TargetDistanceLeniency => pathPointDistanceLeniency;

    [SerializeField] float speed = 3f;

    public enum NPCType
    {
        Normal, Guard
    }
    [SerializeField] NPCType type;
    public NPCType Type => type;

    [SerializeField] SpriteRenderer suspiciousBar;

    [SerializeField] float suspiciousBarIncreaseFactor = 0.25f;

    [Header("Assigned Refs")]
    [SerializeField] SpriteRenderer headSymbolDisplay;

    public enum HeadDisplay
    {
        none, alert, speak, question, eye
    }

    [SerializeField] Sprite alertSymbol;
    [SerializeField] Sprite speakSymbol;
    [SerializeField] Sprite questionSymbol;
    [SerializeField] Sprite eyeSymbol;


    // Acquired Refs
    Rigidbody2D rb;

    Player player;
    public Player Player => player;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindObjectOfType<Player>();

    }

    // Variables
    bool followCurrentPath = true;
    public bool FollowCurrentPath
    {
        get => followCurrentPath;
        set
        {
            followCurrentPath = value;
            if (value == false)
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    Vector2[] currentPath;
    int currentPathPointIndex;

    public bool CurrentPathPointIsLast
    {
        get => currentPathPointIndex == currentPath.Length - 1;
    }
    public Vector2 CurrentPathTarget
    {
        get => currentPath[^1];
    }

    Vector2 facingDir;
    public Vector2 FacingDir => facingDir;

    float suspiciousBarAmount = 0f;
    public float SuspiciousBarAmount { get => suspiciousBarAmount; set => suspiciousBarAmount = value; }

    bool isBusy = false;
    public bool IsBusy {  get => isBusy; set => isBusy = value; }

    public void CreateNewPath(Vector2 target)
    {
        NavMeshPath path = new();
        NavMesh.CalculatePath(rb.transform.position, target, NavMesh.AllAreas, path);

        if (path.status != NavMeshPathStatus.PathComplete)
        {
            Debug.LogError("Failed to find path");
        }

        currentPath = path.corners.Select(pos => new Vector2(pos.x, pos.y)).ToArray();

        currentPathPointIndex = 0;
    }

    private void Update()
    {
        if (followCurrentPath && currentPath != null && currentPath.Length != 0)
        {
            FollowPath();
        }


        suspiciousBar.enabled = suspiciousBarAmount > 0f;
        suspiciousBar.color = Color.Lerp(Color.white, Color.red, suspiciousBarAmount);
    }

    public void ChangeSuspiciousBar(bool increase)
    {
        suspiciousBarAmount = Mathf.Clamp01(suspiciousBarAmount + Time.deltaTime * suspiciousBarIncreaseFactor * (increase ? 1 : -1));
    }

    void FollowPath()
    {
        if (rb.velocity != Vector2.zero) facingDir = rb.velocity.normalized;

        Vector2 currentPathPoint = currentPath[currentPathPointIndex];
        Vector2 dir = currentPathPoint - (Vector2)rb.transform.position;

        rb.velocity = dir.normalized * speed;

        float distanceToNextPathPoint = Vector2.Distance(rb.transform.position, currentPathPoint);
        if (distanceToNextPathPoint > pathPointDistanceLeniency) return;

        if (CurrentPathPointIsLast)
        {
            rb.velocity = Vector2.zero;
        }
        else
        {
            currentPathPointIndex++;
        }
    }


    public void SetHeadDisplay(HeadDisplay display)
    {
        switch (display)
        {
            case HeadDisplay.none:
                headSymbolDisplay.sprite = null;
                break;
            case HeadDisplay.alert:
                headSymbolDisplay.sprite = alertSymbol;
                break;
            case HeadDisplay.speak:
                headSymbolDisplay.sprite = speakSymbol;
                break;
            case HeadDisplay.question:
                headSymbolDisplay.sprite = questionSymbol;
                break;
            case HeadDisplay.eye:
                headSymbolDisplay.sprite = eyeSymbol;
                break;
        }
    }

    public void SetFacingDir(Vector2 dir)
    {
        facingDir = dir;
    }
}
