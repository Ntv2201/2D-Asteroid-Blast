using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Manager")]
    [SerializeField] private int m_score;
    [SerializeField] private int m_topScore;

    private void Awake()
    {
        ClearScore();
    }

    private void OnEnable()
    {
        GameEvents.Instance.onAddToScore += AddScore;
    }

    private void OnDisable()
    {
        GameEvents.Instance.onAddToScore -= AddScore;
    }

    /// <summary>
    /// adds an amount to the current score
    /// </summary>
    /// <param name="amount">How much we want to add</param>
    public void AddScore(int amount = 1)
    {
        m_score += amount;

        if(m_topScore < m_score)
        {
            m_topScore = m_score;
        }
    }

    /// <summary>
    /// clears the current score
    /// </summary>
    public void ClearScore()
    {
        m_score = 0;
    }

}
