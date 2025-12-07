using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public PlayerInputSet input { get; private set; }
    private StateMachine stateMachine;

    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_AttackState attackState { get; private set; }

    [Header("Movement details")]
    public float moveSpeed;

    [Range(0,1)]
    public int facingDirX {get; private set;} = 1;
    public int facingDirY {get; private set;} = -1;    

    public Vector2 moveInput { get; private set; }

    [Header("Collision detection")]
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private LayerMask whatIsWall;


    private bool facingRight = true;
    private bool facingDown = true;

    public bool wallDetectedX {get; private set;}
    public bool wallDetectedY {get; private set;}

    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();

        stateMachine = new StateMachine();
        input = new PlayerInputSet();

        idleState = new Player_IdleState(this, stateMachine, "idle");
        moveState = new Player_MoveState(this, stateMachine, "move");
        attackState = new Player_AttackState(this, stateMachine, "attack");
    }

    private void OnEnable()
    {
        input.Enable();

        //input.Player.Movement.started       //押す
        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();                 //長押し
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;          //キャンセル

        //input.Player.Attack
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void Start()
    {
        stateMachine.Initialize(idleState);
    }

    private void Update()
    {
        HandleCllisionDetection();
        stateMachine.UpdateActiveState();
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
