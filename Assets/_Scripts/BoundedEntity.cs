using Unity.Profiling;
using UnityEngine;
using UnityEngine.Rendering;

public class BoundedEntity : MonoBehaviour
{
    protected Rigidbody2D m_rigidbody;
    protected SpriteRenderer m_spriteRenderer;
    protected Collider2D m_collider;

    [Header("Bounds")]
    [SerializeField] protected Rect m_bounds;

    [Header("Health")]
    [SerializeField] protected int m_health;
    [SerializeField] protected int m_maxHealth = 1;

    protected virtual void OnEnable()
    {
        ResetHealth();
    }

    protected virtual void OnDisable()
    {
        
    }

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_collider = GetComponent<Collider2D>();
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

    protected void LoseHealth()
    {
        
        m_health--;
        if(m_health <= 0)
        {
            OnDie();
        }
    }

    protected void ResetHealth()
    {
        m_health = m_maxHealth;
    }

    protected void SpawnVFX(GameObject vfxPrefabs)
    {
        GameObject vfx = Instantiate(vfxPrefabs);
        vfx.transform.position = transform.position;
    }
    protected virtual void OnDie()
    {
        
    }


}
