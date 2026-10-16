using UnityEngine;

public class VFXCleanUpHandler : MonoBehaviour
{
    [SerializeField] private float m_currentLifeTime;
    [SerializeField] private float m_maxLifeTime;

    private void Update()
    {
        DestroyVFX();
    }

    private void DestroyVFX()
    {
        if(m_currentLifeTime > m_maxLifeTime)
        {
            Destroy(gameObject);
        }
        m_currentLifeTime += Time.deltaTime;
    }

}
