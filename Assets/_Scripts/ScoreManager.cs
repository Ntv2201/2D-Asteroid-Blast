using UnityEngine;
using UnityEngine.Rendering;

public class ScoreManager : MonoBehaviour
{
    [Header("Score Manager")]
    [SerializeField] private int m_score;
    [SerializeField] private int m_topScore;
    [SerializeField] private int m_currentLives;
    [SerializeField] private int m_maxLives;
    [SerializeField] private SoundFXHandler m_collectibleSound;

    private void Awake()
    {
        ClearScore();
    }

    private void OnEnable()
    {
        GameEvents.Instance.onAddToScore += AddScore;
        GameEvents.Instance.onPlayerDie += OnPlayerDie;
        GameEvents.Instance.onRetry += OnRetry;
    }

    private void OnDisable()
    {
        GameEvents.Instance.onAddToScore -= AddScore;
        GameEvents.Instance.onPlayerDie -= OnPlayerDie;
        GameEvents.Instance.onRetry -= OnRetry;
    }


    /// <summary>
    /// adds an amount to the current score
    /// </summary>
    /// <param name="amount">How much we want to add</param>
    public void AddScore(int amount = 1, bool playSound = false)
    {
        m_score += amount;

        if(m_topScore < m_score)
        {
            m_topScore = m_score;
        }
        if (playSound)
        {
            m_collectibleSound.PlaySound();
        }
    }

    /// <summary>
    /// clears the current score
    /// </summary>
    public void ClearScore()
    {
        m_score = 0;
        m_currentLives = m_maxLives;
    }

    private void OnPlayerDie()
    {
        m_currentLives--;
        if(m_currentLives <= 0)
        {
            GameEvents.Instance.OnGameOver();
            Time.timeScale = 0; 
        }
    }

    private void OnRetry()
    {
        Time.timeScale = 1;
        ClearScore();
    }

}
