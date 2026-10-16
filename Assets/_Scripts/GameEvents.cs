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

    public event Action<int> onAddToScore;

    
    public void AddToScore(int amount)
    {
        onAddToScore?.Invoke(amount);
    }

}
