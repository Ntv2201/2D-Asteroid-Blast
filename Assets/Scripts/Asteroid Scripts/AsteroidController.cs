using UnityEngine;

public class AsteroidController : BoundedEntity
{
    [SerializeField] private float m_forcePower; 
    [SerializeField] private float m_angularPower; 

    private void Start()
    {
        m_rigidbody.AddForce(Random.insideUnitCircle * m_rigidbody.mass * m_forcePower, ForceMode2D.Impulse);
        m_rigidbody.angularVelocity = Random.Range(-m_angularPower, m_angularPower);
    }
}
