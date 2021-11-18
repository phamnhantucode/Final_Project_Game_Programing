using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatController : MonoBehaviour
{
    [SerializeField] bool _combatEnabled;
    [SerializeField] float _inputTimer;
    [SerializeField] float _attack1Radius;
    [SerializeField] float _attack1Damage;

    [SerializeField] Transform _attack1HitBoxPos;

    [SerializeField] LayerMask _isDamageable;

    float _lastInputTime = Mathf.NegativeInfinity;

    float[] _attackDetails = new float[2];

    bool _gotInput;
    bool _isAttacking;
    bool _isFirstAttack;

    Animator _anim;
    PlayerController _pc;
    PlayerStats _ps;

    void Awake()
    {
        _anim = GetComponent<Animator>();
        _pc = GetComponent<PlayerController>();
        _ps = GetComponent<PlayerStats>();
    }

    void Start()
    {
        _anim.SetBool("canAttack", _combatEnabled);
    }

    void Update()
    {
        CheckCombatInput();
        CheckAttacks();
    }

    void CheckCombatInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_combatEnabled)
            {
                // attempt combat
                _gotInput = true;
                _lastInputTime = Time.time;
            }
        }
    }

    void CheckAttacks()
    {
        if (_gotInput)
        {
            //perform attack 1
            if (!_isAttacking)
            {
                _gotInput = false;
                _isAttacking = true;
                _isFirstAttack = !_isFirstAttack;
                _anim.SetBool("attack1", true);
                _anim.SetBool("firstAttack", _isFirstAttack);
                _anim.SetBool("isAttacking", _isAttacking);
            }
        }

        if (Time.time >= _lastInputTime + _inputTimer)
        {
            // wait for new input
            _gotInput = false;
        }
    }

    void Damage(float[] attackDetails)
    {
        if (!_pc.GetDashingStatus())
        {
            int enemyDirection;

            _ps.DecreaseHealth(attackDetails[0]);

            if (attackDetails[1] < transform.position.x)
            {
                enemyDirection = 1;
            }
            else
            {
                enemyDirection = -1;
            }

            _pc.Knockback(enemyDirection);
        }
    }

    void CheckAttackHitBox()
    {
        Collider2D[] detectedObjects = Physics2D.OverlapCircleAll(_attack1HitBoxPos.position, _attack1Radius, _isDamageable);

        _attackDetails[0] = _attack1Damage;
        _attackDetails[1] = transform.position.x;

        foreach (Collider2D collider in detectedObjects)
        {
            collider.transform.parent.SendMessage("Damage", _attackDetails);
        }
    }

    void FinishAttack1()
    {
        _isAttacking = false;
        _anim.SetBool("isAttacking", _isAttacking);
        _anim.SetBool("attack1", false);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(_attack1HitBoxPos.position, _attack1Radius);
    }

}
