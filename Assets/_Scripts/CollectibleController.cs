using System.Net;
using UnityEngine;

public class CollectibleController : MonoBehaviour
{
    [SerializeField] private int m_pickUpScore;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out ShipController controller))
        {
            GameEvents.Instance.AddToScore(m_pickUpScore);
            Destroy(gameObject);
        }
    }
}
