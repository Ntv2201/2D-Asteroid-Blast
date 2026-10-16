using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : BoundedEntity
{
    
    private float m_forwarInput;
    private float m_turnInput;
    [Header("Move Settings")]
    [SerializeField] private float m_turnSpeed;
    [SerializeField] private float m_moveSpeed;
    [SerializeField] private float m_maxMoveSpeed;
    [SerializeField] private float m_stoppingPower;

    [Header("Bullet")]
    [SerializeField] private GameObject m_bulletPrefabs;
    [SerializeField] private float m_fireDelay;
    [SerializeField] private float m_fireCount;
    [SerializeField] private bool m_isFiring;

    [SerializeField] private bool m_isDead;

    protected override void OnEnable()
    {
        base.OnEnable();

        GameEvents.Instance.onRetry += OnRetry;

    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameEvents.Instance.onRetry -= OnRetry;
    }


    private void OnMove(InputValue value)
    {
        Vector2 moveInputDirection = value.Get<Vector2>();
        
        m_turnInput = moveInputDirection.x;
        m_forwarInput = moveInputDirection.y;
    }

    private void OnAttack(InputValue value)
    {
        m_isFiring = value.Get<float>() > 0;
    }

    protected override void LateUpdate()
    {
        if(m_isDead) return;

        base.LateUpdate();

        HandleMove();
        HandleTurn();
        
    }

    private void HandleTurn()
    {
        // handle turn
        m_rigidbody.rotation -= (m_turnInput * (m_turnSpeed * 100f)) * Time.deltaTime;
    }

    private void HandleMove()
    {
        // handle movement
        if (m_forwarInput > 0)
        {
            m_rigidbody.AddRelativeForceY((m_forwarInput * (m_moveSpeed * 100f)) * Time.deltaTime);

        }
        else if (m_forwarInput < 0)
        {
            m_rigidbody.linearVelocity = Vector2.Lerp(m_rigidbody.linearVelocity, Vector2.zero, m_stoppingPower * Time.deltaTime);
        }       

        // cap the move speed
        if(m_rigidbody.linearVelocity.magnitude > m_maxMoveSpeed)
        {
            m_rigidbody.linearVelocity = m_rigidbody.linearVelocity.normalized * m_maxMoveSpeed;
        }

        if (m_isFiring)
        {
            TrySpawnBullet();
        }
        else
        {
            m_fireCount = m_fireDelay;
        }
    }

    private void TrySpawnBullet()
    {
        if(m_fireCount > m_fireDelay)
        {
            m_fireCount = 0;

            GameObject bullet = Instantiate(m_bulletPrefabs);
            bullet.transform.position = transform.position + (transform.up * 3f);
            bullet.transform.up = transform.up;
        }
        else
        {
            m_fireCount += Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent(out AsteroidController asteroid))
        {
            LoseHealth();
        }
    }

    protected void SetPlayerAsDead()
    {
        m_isDead = true;
        m_spriteRenderer.enabled = false;
        m_collider.enabled = false;
        m_rigidbody.simulated = false;
    }

    protected override void OnDie()
    {
        base.OnDie();       

        GameEvents.Instance.OnPlayerDie();

        SetPlayerAsDead();

        StartCoroutine(RespawnPlayerCoroutine());
    }



    private IEnumerator RespawnPlayerCoroutine()
    {
        yield return new WaitForSeconds(.5f);
        m_rigidbody.position = Vector2.zero;
        m_rigidbody.transform.position = Vector3.zero;
        m_rigidbody.linearVelocity = Vector2.zero;
        m_rigidbody.rotation = 0;
        m_spriteRenderer.enabled = true;
        ResetHealth();

        yield return new WaitForSeconds(.5f);
        m_isDead = false;
        m_rigidbody.simulated = true;

        yield return new WaitForSeconds(2f);
        m_collider.enabled = true;
    }

    private void OnRetry()
    {
        SetPlayerAsDead();
        StartCoroutine(RespawnPlayerCoroutine());
    }

}
