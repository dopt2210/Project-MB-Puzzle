using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UIElements;

[System.Serializable]
public class OptionUI
{
    [SerializeField] private VisualTreeAsset optionPanelAsset;

    private VisualElement _optionElement;
    private List<Button> _optionButtons = new();
    private Dictionary<Button, VisualElement> _buttonToPanelMap = new();
    private VisualElement _currentPanel;
    private Button _lastFocusedNavButton;

    public VisualElement OptionElement => _optionElement;
    public List<Button> OptionButtons => _optionButtons;
    public Dictionary<Button, VisualElement> ButtonToPanelMap => _buttonToPanelMap;

    public void Initialize(VisualElement root, AudioMixer audioMixer)
    {
        _optionElement = optionPanelAsset.CloneTree();

        var graphicElement = _optionElement.Q<VisualElement>("Graphic");
        var volumeElement = _optionElement.Q<VisualElement>("Volume");
        var guideElement = _optionElement.Q<VisualElement>("Guide");

        _optionButtons = _optionElement.Query<Button>().ToList();

        _buttonToPanelMap[_optionButtons[0]] = graphicElement;
        _buttonToPanelMap[_optionButtons[1]] = volumeElement;
        _buttonToPanelMap[_optionButtons[2]] = guideElement;

        var sliders = _optionElement.Query<Slider>().ToList();
        if (sliders.Count >= 2)
        {
            sliders[0].RegisterValueChangedCallback(evt => UITools.UpdateMusicVolume(evt.newValue, audioMixer));
            sliders[1].RegisterValueChangedCallback(evt => UITools.UpdateSFXVolume(evt.newValue, audioMixer));
        }

        foreach (var button in _optionButtons)
        {
            button.clicked += ClickButton;
        }
        foreach (Button button in _optionButtons)
        {
            button.RegisterCallback<FocusInEvent>(UITools.OnPointerEnter);
        }

        SetFocusPanelEvent();

    }
    void ClickButton()
    {
        SoundManager.Instance.PlaySound2D("Click");
    }
    private void SetFocusPanelEvent()
    {
        foreach (var kvp in ButtonToPanelMap)
        {

            var btn = kvp.Key;
            var panel = kvp.Value;

            btn.RegisterCallback<FocusInEvent>(evt =>
            {
                ShowOnlyThisPanel(kvp.Value);

            });
        }
    }

    #region panel display
    private void EnterPanelMode(Button navButton, VisualElement panel)
    {
        _lastFocusedNavButton = navButton;
        foreach (var k in _buttonToPanelMap.Keys)
        {
            k.focusable = false;
        }

        EnablePanelFocusable(panel);
        FocusFirstElementIn(panel);
    }
    private void ExitPanelMode()
    {
        foreach (var k in ButtonToPanelMap.Keys)
        {
            k.focusable = true;
        }
        DisablePanelsFocusable(ButtonToPanelMap);
        if (_lastFocusedNavButton != null)
        {
            _lastFocusedNavButton.Focus();
        }
    }
    private void FocusFirstElementIn(VisualElement panel)
    {
        var firstFocusable = panel.Query<VisualElement>().Where(x => x.focusable).First();
        if (firstFocusable != null)
        {
            firstFocusable.Focus();
        }
    }
    private void EnablePanelFocusable(VisualElement panel)
    {
        foreach (var child in panel.Children())
        {
            child.focusable = true;
        }
    }
    private void DisablePanelsFocusable(Dictionary<Button, VisualElement> panels)
    {
        foreach (var panel in panels.Values)
        {
            foreach (var child in panel.Children())
            {
                child.focusable = false;
            }
        }
    }
    private void ShowOnlyThisPanel(VisualElement targetPanel)
    {
        DisablePanelsFocusable(ButtonToPanelMap);
        if (_currentPanel != null)
            _currentPanel.style.display = DisplayStyle.None;

        targetPanel.style.display = DisplayStyle.Flex;
        _currentPanel = targetPanel;
    }
    #endregion

}
