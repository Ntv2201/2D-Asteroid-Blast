using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class HUD : MonoBehaviour
{
    [SerializeField] ScoreManager m_scoreManager;
    private UIDocument m_uiDocument;
    private VisualElement m_gameOverScreen;

    [SerializeField] private SoundFXHandler m_focusSound;
    [SerializeField] private SoundFXHandler m_submitSound;

    private void OnEnable()
    {
        m_uiDocument = gameObject.GetComponent<UIDocument>();
        VisualElement root = m_uiDocument.rootVisualElement;

        VisualElement container = root.Q<VisualElement>("Container");
        container.dataSource = m_scoreManager;
        
        m_gameOverScreen = root.Q<VisualElement>("GameOver");

        VisualElement retryButton = m_gameOverScreen.Q<VisualElement>("RetryButton");
        VisualElement quitButton = m_gameOverScreen.Q<VisualElement>("QuitButton");

        Clickable retryClickable = new Clickable(() =>{
            HandleRetryEvent();
        });

        Clickable quitClickable = new Clickable(() =>{
            OnQuit();
        });

        retryButton.AddManipulator(retryClickable);
        quitButton.AddManipulator(quitClickable);

        retryButton.AddManipulator(new UISoundManipulator(m_focusSound, m_submitSound));
        quitButton.AddManipulator(new UISoundManipulator(m_focusSound, m_submitSound));

        GameEvents.Instance.onGameOver += OnGameOver;

    }

    private void OnQuit()
    {
        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
        #endif
    }

    private void HandleRetryEvent()
    {
        GameEvents.Instance.OnRetry();
        m_gameOverScreen.AddToClassList("hidden");
        
    }

    private void OnDisable()
    {
        GameEvents.Instance.onGameOver -= OnGameOver;
    }

    private void OnGameOver()
    {
        m_gameOverScreen.RemoveFromClassList("hidden");
    
    }

}
