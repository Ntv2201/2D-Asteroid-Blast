using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Setiing")]
    [SerializeField] private float m_forwardSpeed;
    [SerializeField] private float m_maxLifeTime;
    [SerializeField] private float m_currentLifeTime;

    [Header("Bullet Hit Effect")]
    [SerializeField] private GameObject m_bulletHitFX;

    private void Update()
    {
        transform.position += (transform.up * m_forwardSpeed) * Time.deltaTime;

        m_currentLifeTime += Time.deltaTime;

        if(m_currentLifeTime > m_maxLifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        SpawnVFX();
        Destroy(gameObject);
    }

    private void SpawnVFX()
    {
        GameObject hit = GameObject.Instantiate(m_bulletHitFX);
        hit.transform.position = transform.position;
    }

}
