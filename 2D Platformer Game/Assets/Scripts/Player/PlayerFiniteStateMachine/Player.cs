using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] PlayerData _playerData;

    [SerializeField] Transform _groundCheck;
    [SerializeField] Transform _wallCheck;

    public PlayerStateMachine stateMachine { get; private set; }
    public PlayerIdleState idleState { get; private set; }
    public PlayerMoveState moveState { get; private set; }
    public PlayerJumpState jumpState { get; private set; }
    public PlayerInAirState inAirState { get; private set; }
    public PlayerLandState landState { get; private set; }
    public PlayerWallSlideState wallSlideState { get; private set; }
    public PlayerWallGrabState wallGrabState { get; private set; }
    public PlayerWallClimbState wallClimbState { get; private set; }

    public Animator anim { get; private set; }
    public Rigidbody2D rb { get; private set; }
    public PlayerInputHandler inputHandler { get; private set; }

    public Vector2 currentVelocity { get; private set; }
    public int facingDirection { get; private set; }
    public float initialGravity { get; private set; }

    Vector2 _workSpace;

    void Awake()
    {
        anim = GetComponent<Animator>();
        inputHandler = GetComponent<PlayerInputHandler>();
        rb = GetComponent<Rigidbody2D>();

        stateMachine = new PlayerStateMachine();

        idleState = new PlayerIdleState(this, stateMachine, _playerData, "idle");
        moveState = new PlayerMoveState(this, stateMachine, _playerData, "move");
        jumpState = new PlayerJumpState(this, stateMachine, _playerData, "inAir");
        inAirState = new PlayerInAirState(this, stateMachine, _playerData, "inAir");
        landState = new PlayerLandState(this, stateMachine, _playerData, "land");
        wallSlideState = new PlayerWallSlideState(this, stateMachine, _playerData, "wallSlide");
        wallGrabState = new PlayerWallGrabState(this, stateMachine, _playerData, "wallGrab");
        wallClimbState = new PlayerWallClimbState(this, stateMachine, _playerData, "wallClimb");
    }

    void OnEnable()
    {
        stateMachine.Initialize(idleState);
        facingDirection = 1;
        initialGravity = rb.gravityScale;
    }

    void Update()
    {
        currentVelocity = rb.velocity;
        stateMachine.currentState.LogicUpdate();
    }

    void FixedUpdate()
    {
        stateMachine.currentState.PhysicsUpdate();
    }

    public void SetVelocityX(float velocity)
    {
        _workSpace.Set(velocity, currentVelocity.y);
        rb.velocity = _workSpace;
        currentVelocity = _workSpace;
    }

    public void SetVelocityY(float velocity)
    {
        _workSpace.Set(currentVelocity.x, velocity);
        rb.velocity = _workSpace;
        currentVelocity = _workSpace;
    }

    public void CheckIfShouldFlip(int xInput)
    {
        if (xInput != 0 && xInput != facingDirection)
        {
            Flip();
        }
    }

    public bool CheckIfGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, _playerData.groundCheckRadius, _playerData.ground);
    }

    public bool CheckIfTouchingWall()
    {
        return Physics2D.Raycast(_wallCheck.position, Vector2.right * facingDirection, _playerData.wallCheckDistance, _playerData.ground);
    }

    void Flip()
    {
        facingDirection *= -1;
        transform.Rotate(0f, 180f, 0f);
    }

    void AnimationTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }

    void AnimationFinishTrigger()
    {
        stateMachine.currentState.AnimationFinishTrigger();
    }
}
