using System;
using JetBrains.Annotations;
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
    
}
