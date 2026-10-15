using UnityEngine;

public class BoundedEntity : MonoBehaviour
{
    [Header("Bounds")]
    [SerializeField] protected Rect m_bounds;
    protected Rigidbody2D m_rigidbody;

    [Header("Health")]
    [SerializeField] protected int m_health;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    protected virtual void LateUpdate()
    {
        HandleBound();
    }

    private void HandleBound()
    {
        // handle bounds
        if (!m_bounds.Contains(transform.position))
        {
            Vector2 position = transform.position;
            if(position.x < m_bounds.xMin)
            {
                position.x = m_bounds.xMax;
            }
            if(position.x > m_bounds.xMax)
            {
                position.x = m_bounds.xMin;
            }

            if(position.y < m_bounds.yMin)
            {
                position.y = m_bounds.yMax;
            }
            if(position.y > m_bounds.yMax)
            {
                position.y = m_bounds.yMin;
            }
            m_rigidbody.position = position;
        }
    }
}
