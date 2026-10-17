using System;
using System.Collections;
using JetBrains.Annotations;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.VFX;

public class ShipController : BoundedEntity
{
    
    private float m_forwarInput;
    private float m_turnInput;
    
    [Header("Move Settings")]
    [SerializeField] private float m_turnSpeed;
    [SerializeField] private float m_moveSpeed;
    [SerializeField] private float m_maxMoveSpeed;
    [SerializeField] private float m_boostSpeed;
    [SerializeField] private float m_maxBoostingSpeed;
    [SerializeField] private float m_stoppingPower;

    [Header("Death VFX")]
    [SerializeField] private GameObject m_deathVFXPrefab;
    [SerializeField] private bool m_isDead;
    [SerializeField] private bool m_isInvulnerable;

    [Header("Visual Effect")]
    [SerializeField] private VisualEffect m_leftBoostFX;
    [SerializeField] private VisualEffect m_rightBoostFX;
    [SerializeField] private GameObject m_shieldFX;
    [SerializeField] private Material m_fullScreenFX;
    [SerializeField] private AnimationCurve m_fullScreenEase;
    [SerializeField] private float m_easeTime;
    [SerializeField] private bool m_isRunningRoutine;
    private Coroutine m_currentHitCoroutine;

    [Header("Sound FX")]
    [SerializeField] private SoundFXHandler m_fireSFX;
    [SerializeField] private SoundFXHandler m_hitSFX;
    [SerializeField] private SoundFXHandler m_dieSFX;
    [SerializeField] private SoundFXHandler m_thrustSFX;
    [SerializeField] private SoundFXHandler m_boostSFX;

    [Header("Bullet")]
    [SerializeField] private GameObject m_bulletPrefabs;
    [SerializeField] private float m_fireDelay;
    [SerializeField] private float m_fireCount;
    [SerializeField] private bool m_isFiring;
    [SerializeField] private bool m_isBoosting;

    protected override void OnEnable()
    {
        base.OnEnable();

        GameEvents.Instance.onRetry += OnRetry;
        ResetFullScreenFX();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GameEvents.Instance.onRetry -= OnRetry;
        ResetFullScreenFX();
    }

    private void OnMove(InputValue value)
    {
        Vector2 moveInputDirection = value.Get<Vector2>();
        
        m_turnInput = moveInputDirection.x;
        m_forwarInput = moveInputDirection.y;

        if(m_forwarInput > 0 )
        {
            m_thrustSFX.FadeIn();
            EnableBoostFX(true, true);
        }
        else
        {
            EnableBoostFX(false, false);
            m_thrustSFX.FadeOut();
        }

    }

    private void OnAttack(InputValue value)
    {
        m_isFiring = value.Get<float>() > 0;
    }

    private void OnBoost(InputValue value)
    {
        m_isBoosting = value.Get<float>() > 0;

        if (m_isBoosting)
        {
            m_boostSFX.FadeIn();
            EnableBoostFX(true, true);
        }
        else
        {
            m_boostSFX.FadeOut();
        }
        
        if(!m_isBoosting && m_forwarInput == 0)
        {
            EnableBoostFX(false, false);
        }

        m_leftBoostFX.SetBool("isBoosted", m_isBoosting);
        m_rightBoostFX.SetBool("isBoosted", m_isBoosting);
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
        if (m_isBoosting)
        {
            m_rigidbody.AddRelativeForceY((m_boostSpeed * 100f) * Time.deltaTime);
        }

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
        if(m_isBoosting && m_rigidbody.linearVelocity.magnitude > m_maxBoostingSpeed)
        {
            m_rigidbody.linearVelocity = m_rigidbody.linearVelocity.normalized * m_maxBoostingSpeed;
        }
        else if(!m_isBoosting && m_rigidbody.linearVelocity.magnitude > m_maxMoveSpeed)
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

            m_fireSFX.PlaySound();
        }
        else
        {
            m_fireCount += Time.deltaTime;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(m_isInvulnerable) return;

        if(collision.gameObject.TryGetComponent(out AsteroidController asteroid))
        {
            StartCoroutine(InvulnerableRoutine());
            m_hitSFX.PlaySound();
            PlayHitFullScreenFX();
            LoseHealth();
            
        }
    }

    private IEnumerator InvulnerableRoutine()
    {
        yield return new WaitForSeconds(.3f);

        m_isInvulnerable = true;

        yield return new WaitForSeconds(2f);
        m_isInvulnerable = false;
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

        m_dieSFX.PlaySound();

        SpawnVFX(m_deathVFXPrefab);

        SetPlayerAsDead();

        StartCoroutine(RespawnPlayerRoutine());
    }

    private IEnumerator RespawnPlayerRoutine()
    {
        yield return new WaitForSeconds(1f);

        m_rigidbody.position = Vector3.zero;
        m_rigidbody.transform.position = Vector3.zero;
        m_rigidbody.linearVelocity = Vector2.zero;
        m_rigidbody.angularVelocity= 0;   
        m_spriteRenderer.enabled = true;
        ResetHealth();
        m_shieldFX.SetActive(true);
        m_collider.enabled = true;
        m_rigidbody.simulated = true;
        m_isDead = false;
        m_isInvulnerable = true;

        yield return new WaitForSeconds(3f);
        m_shieldFX.SetActive(false);
        m_isInvulnerable = false;
    }

    private void OnRetry()
    {
        SetPlayerAsDead();
        StartCoroutine(RespawnPlayerRoutine());
    }

    private void EnableBoostFX(bool left, bool right)
    {
        m_leftBoostFX.enabled = left;
        m_rightBoostFX.enabled = right;
    }

    private void PlayHitFullScreenFX()
    {
        if (m_isRunningRoutine) return;

        StartCoroutine(HitFullScreenFXRoutine());
    }

    private IEnumerator HitFullScreenFXRoutine()
    {
        m_isRunningRoutine = true;
        try
        {
            float time = 0f;
        
            while(time < 1f)
            {
                yield return new WaitForSeconds(m_easeTime/100f);
                time += 0.01f;    
                m_fullScreenFX.SetFloat("_Amount", m_fullScreenEase.Evaluate(time));
            }

            while(time > 0f)
            {
                yield return new WaitForSeconds(m_easeTime/100f);
                time -= 0.01f;    
                m_fullScreenFX.SetFloat("_Amount", m_fullScreenEase.Evaluate(time));
            }
        }
        finally
        {
            m_isRunningRoutine = false;
        }
        
    }

    public void ResetFullScreenFX()
    {
        if (m_currentHitCoroutine != null)
        {
            StopCoroutine(m_currentHitCoroutine);
            m_currentHitCoroutine = null;
        }

        m_isRunningRoutine = false;

        // Trả material/shader về trạng thái gốc (Amount = 0)
        if (m_fullScreenFX != null)
        {
            m_fullScreenFX.SetFloat("_Amount", 0f);
        }
    }

}
