using Unity.VisualScripting;
using UnityEngine;

public class Entity : MonoBehaviour
{
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    protected StateMachine stateMachine;


    [Header("Movement details")]

    [Range(0, 1)]
    public int facingDirX { get; private set; } = 1;
    public int facingDirY { get; private set; } = -1;


    [Header("Collision detection")]
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsWall;


    private bool facingRight = true;
    private bool facingDown = true;

    public bool wallDetectedX { get; private set; }
    public bool wallDetectedY { get; private set; }

    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        stateMachine = new StateMachine();
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        HandleCllisionDetection();
        stateMachine.UpdateActiveState();
    }

    private void FixedUpdate()
    {
        stateMachine.FixedUpdateActiveState();
    }

    public void CallAnimationTrigger()
    {
        stateMachine.currentState.CallAnimationTrigger();
    }

    private void HandleFlipX(float xVelocity)
    {
        if (xVelocity > 0 && !facingRight)
            FlipX();
        else if (xVelocity < 0 && facingRight)
            FlipX();
    }

    private void HandleFlipY(float yVelocity)
    {
        if (yVelocity < 0 && !facingDown)
            FlipY();
        else if (yVelocity > 0 && facingDown)
            FlipY();
    }

    public void SetVelocity(float xVelocity, float yVelocity)
    {
        rb.linearVelocity = new Vector2(xVelocity, yVelocity);
        HandleFlipX(xVelocity);
        HandleFlipY(yVelocity);
    }

    public void FlipX()
    {
        transform.Rotate(0f, 180f, 0f);
        facingRight = !facingRight;
        facingDirX *= -1;
    }

    public void FlipY()
    {
        //transform.Rotate(180f, 0f, 0f);
        facingDown = !facingDown;
        facingDirY *= -1;
    }

    private void HandleCllisionDetection()
    {
        //壁に当たる判定
        wallDetectedX = Physics2D.Raycast(transform.position, Vector2.right * facingDirX, wallCheckDistance, whatIsWall);
        wallDetectedY = Physics2D.Raycast(transform.position, Vector2.up * facingDirY, wallCheckDistance, whatIsWall);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(wallCheckDistance * facingDirX, 0));
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(0, wallCheckDistance * facingDirY));
    }
}
