using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class UISoundManipulator : PointerManipulator
{
    private SoundFXHandler m_focusSound;
    private SoundFXHandler m_submitSound;

    public UISoundManipulator(SoundFXHandler focusSound, SoundFXHandler submitSound)
    {
        m_focusSound = focusSound;
        m_submitSound = submitSound;
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<PointerEnterEvent>(OnPointerEnter);
        target.RegisterCallback<PointerDownEvent>(OnPointerDown);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<PointerEnterEvent>(OnPointerEnter);
        target.UnregisterCallback<PointerDownEvent>(OnPointerDown);
    }

    private void OnPointerEnter(PointerEnterEvent enterEvent)
    {
        m_focusSound.PlaySound();
    }

    private void OnPointerDown(PointerDownEvent downEvent)
    {
        m_submitSound.PlaySound();
    }


}
