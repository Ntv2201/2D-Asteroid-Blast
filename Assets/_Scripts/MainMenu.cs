using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    private UIDocument m_uiDocument;

    [SerializeField] SoundFXHandler m_focusSound;
    [SerializeField] SoundFXHandler m_submitSound;

    private VisualElement m_playButton;
    private VisualElement m_quitButton;

    private void OnEnable()
    {
        m_uiDocument = GetComponent<UIDocument>();
        VisualElement root = m_uiDocument.rootVisualElement;

        m_playButton = root.Q<VisualElement>("PlayButton");
        m_quitButton = root.Q<VisualElement>("QuitButton");

        Clickable playClickable = new Clickable(() =>
        {
            OnPlay();
        });

        Clickable quitClickable = new Clickable(() =>
        {
            OnQuit();
        });

        m_playButton.AddManipulator(playClickable);
        m_playButton.AddManipulator(new UISoundManipulator(m_focusSound, m_submitSound));

        m_quitButton.AddManipulator(quitClickable);
        m_quitButton.AddManipulator(new UISoundManipulator(m_focusSound, m_submitSound));



    }

    // public enum SceneNames
    // {
    //     MainMenu = 0,
    //     GamePlay = 1
    // }

    private void OnQuit()
    {
        Application.Quit();

        #if UNITY_EDITOR
            UnityEditor.EditorApplication.ExitPlaymode();
        #endif
    }

    private void OnPlay()
    {
        SceneManager.LoadScene(1);
    }

}
