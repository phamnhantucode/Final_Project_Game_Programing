using System.Collections;
using UnityEngine;

public class Enemy4 : Entity
{
    public E4_MoveState moveState { get; private set; }
    public E4_IdleState idleState { get; private set; }
    public E4_PlayerDetectedState playerDetectedState { get; private set; }
    public E4_LookForPlayerState lookForPlayerState { get; private set; }
    public E4_MeleeAttackState meleeAttackState { get; private set; }
    public E4_StunState stunState { get; private set; }
    public E4_DeadState deadState { get; private set; }
    public E4_DodgeState dodgeState { get; private set; }
    public E4_RangeAttackState rangeAttackState { get; private set; }

    public Vector3 initialPosition { get; private set; }
    public Quaternion initialRotation { get; private set; }

    [SerializeField] D_MoveState _moveStateData;
    [SerializeField] D_IdleState _idleStateData;
    [SerializeField] D_PlayerDetectedState _playerDetectedStateData;
    [SerializeField] D_LookForPlayerState _lookForPlayerStateData;
    [SerializeField] D_MeleeAttackState _meleeAttackStateData;
    [SerializeField] D_StunState _stunStateData;
    [SerializeField] D_DeadState _deadStateData;
    [SerializeField] D_DodgeState _dodgeStateData;
    [SerializeField] D_RangeAttackState _rangeAttackStateData;

    [SerializeField] Transform _meleeAttackPosition;
    [SerializeField] Transform _rangeAttackPosition;

    public D_DodgeState dodgeStateData  { get { return _dodgeStateData; } }

    Transform _head;
    Transform _leftArm;
    Transform _rightArm;

    IEnumerator _resetBodyParts;
    Vector3 _bodyLookAtDirection;
    float _bodyLookAtAngle;
    Quaternion _bodyLookAtRotation;
    Quaternion _headLookAtRotation;

    public override void Awake()
    {
        base.Awake();

        moveState = new E4_MoveState(this, stateMachine, "move", _moveStateData, this);
        idleState = new E4_IdleState(this, stateMachine, "idle", _idleStateData, this);
        playerDetectedState = new E4_PlayerDetectedState(this, stateMachine, "playerDetected", _playerDetectedStateData, this);
        lookForPlayerState = new E4_LookForPlayerState(this, stateMachine, "lookForPlayer", _lookForPlayerStateData, this);
        meleeAttackState = new E4_MeleeAttackState(this, stateMachine, "meleeAttack", _meleeAttackPosition, _meleeAttackStateData, this);
        stunState = new E4_StunState(this, stateMachine, "stun", _stunStateData, this);
        deadState = new E4_DeadState(this, stateMachine, "dead", _deadStateData, this);
        dodgeState = new E4_DodgeState(this, stateMachine, "dodge", _dodgeStateData, this);
        rangeAttackState = new E4_RangeAttackState(this, stateMachine, "rangeAttack", _rangeAttackPosition, _rangeAttackStateData, this);

        initialPosition = transform.position;
        initialRotation = transform.rotation;

        _head = transform.Find("Body").Find("MoveHead");
        _leftArm = transform.Find("Body").Find("MoveBowArm");
        _rightArm = transform.Find("Body").Find("MoveRightArm");
    }

    public override void OnEnable()
    {
        base.OnEnable();

        stateMachine.Initialize(moveState);
    }

    public override bool Damage(AttackDetails attackDetails)
    {
        base.Damage(attackDetails);

        Enemy1HitParticlePool.Instance.Get(transform.position, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0, 360)));

        if (isDead)
        {
            stateMachine.ChangeState(deadState);
        }
        else if (isStunned && stateMachine.currentState != stunState)
        {
            stateMachine.ChangeState(stunState);
        }

        return true;
    }

    public override void RotateBodyToPlayer()
    {
        if (_resetBodyParts != null)
            StopCoroutine(_resetBodyParts);

        _bodyLookAtDirection = (_playerTransform.position - _head.position).normalized;

        if (_bodyLookAtDirection.x > 0f)
        {
            if (facingDirection == -1)
                Flip();

            _bodyLookAtAngle = Vector2.SignedAngle(Vector2.right, _bodyLookAtDirection);
            _bodyLookAtRotation = Quaternion.AngleAxis(_bodyLookAtAngle, Vector3.forward);
            _headLookAtRotation = Quaternion.AngleAxis(_bodyLookAtAngle > 40 ? 40 : _bodyLookAtAngle, Vector3.forward);
        }
        else
        {
            if (facingDirection == 1)
                Flip();

            _bodyLookAtAngle = Vector2.SignedAngle(-Vector2.right, _bodyLookAtDirection);
            _bodyLookAtRotation = Quaternion.AngleAxis(-_bodyLookAtAngle, Vector3.forward);
            _headLookAtRotation = Quaternion.AngleAxis(-_bodyLookAtAngle > 40 ? 40 : -_bodyLookAtAngle, Vector3.forward);
        }

        _head.localRotation = Quaternion.Slerp(_head.localRotation, _headLookAtRotation, Time.deltaTime * 5f);
        _leftArm.localRotation = Quaternion.Slerp(_leftArm.localRotation, _bodyLookAtRotation, Time.deltaTime * 5f);
        _rightArm.localRotation = Quaternion.Slerp(_rightArm.localRotation, _bodyLookAtRotation, Time.deltaTime * 5f);
        _rangeAttackPosition.localRotation = Quaternion.Slerp(_rangeAttackPosition.localRotation, _bodyLookAtRotation, Time.deltaTime * 5f);
    }

    public override void ResetBodyPosition()
    {
        if (_resetBodyParts != null)
            StopCoroutine(_resetBodyParts);

        _resetBodyParts = ResetBodyParts();
        StartCoroutine(_resetBodyParts);
    }

    IEnumerator ResetBodyParts()
    {
        while (_rangeAttackPosition.localRotation.z > 0.01f)
        {
            _head.localRotation = Quaternion.Slerp(_head.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 5f);
            _leftArm.localRotation = Quaternion.Slerp(_leftArm.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 5f);
            _rightArm.localRotation = Quaternion.Slerp(_rightArm.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 5f);
            _rangeAttackPosition.localRotation = Quaternion.Slerp(_rangeAttackPosition.localRotation, Quaternion.Euler(0f, 0f, 0f), Time.deltaTime * 5f);

            yield return new WaitForFixedUpdate();
        }
    }

    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.DrawWireSphere(_meleeAttackPosition.position, _meleeAttackStateData.attackDetails[0].attackRadius);
    }

}