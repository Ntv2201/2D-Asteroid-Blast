using System;
using UnityEngine;

public class GameEvents 
{
    private static GameEvents m_instance;
    public static GameEvents Instance
    {
        get
        {
            if(m_instance == null)
            {
                m_instance = new GameEvents();
            }
            return m_instance;
        }
    }

    public event Action<int, bool> onAddToScore;
    public event Action onPlayerDie;
    public event Action onGameOver;
    public event Action onRetry;

    public void AddToScore(int amount = 1, bool playSound = false)
    {
        onAddToScore?.Invoke(amount, playSound);
    }

    public void OnPlayerDie()
    {
        onPlayerDie?.Invoke();
    }

    public void OnGameOver()
    {
        onGameOver?.Invoke();
    }

    public void OnRetry()
    {
        onRetry?.Invoke();
    }

}
