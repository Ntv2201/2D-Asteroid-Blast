using UnityEngine;
using UnityEngine.UIElements;

public class HUD : MonoBehaviour
{
    [SerializeField] ScoreManager m_scoreManager;
    private UIDocument m_uiDocument;
    

    private void OnEnable()
    {
        m_uiDocument = gameObject.GetComponent<UIDocument>();
        VisualElement root = m_uiDocument.rootVisualElement;

        Label scoreLabel = root.Q<Label>("ScoreValue");
        scoreLabel.dataSource = m_scoreManager;

    }
}
