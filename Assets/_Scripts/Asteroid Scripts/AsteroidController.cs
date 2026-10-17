using System;
using UnityEngine;
using UnityEngine.VFX;

public class AsteroidController : BoundedEntity
{
    [Header("Death VFX")]
    [SerializeField] private GameObject m_asteroidDeathVFX;

    [SerializeField] private float m_forcePower; 
    [SerializeField] private float m_angularPower;
    [SerializeField] private float m_constantPower;
    [SerializeField] private int m_size;

    public int Size => m_size;

    public event Action<AsteroidController> onAsteroidDie;


    private void Start()
    {
        m_rigidbody.AddForce(UnityEngine.Random.insideUnitCircle * m_rigidbody.mass * m_forcePower, ForceMode2D.Impulse);
        m_rigidbody.angularVelocity = UnityEngine.Random.Range(-m_angularPower, m_angularPower);
    }

    private void FixedUpdate()
    {
        Vector3 direction = transform.up;

        m_rigidbody.AddForce((direction * m_rigidbody.mass * m_constantPower) * Time.deltaTime, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Bullet bullet))
        {
            LoseHealth();
        }
    }

    protected override void OnDie()
    {
        base.OnDie();

        SpawnVFX(m_asteroidDeathVFX);
        onAsteroidDie ?.Invoke(this);

    }
}
