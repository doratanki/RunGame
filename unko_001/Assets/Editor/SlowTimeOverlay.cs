#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Scene ビューに常時表示されるスロータイム操作オーバーレイ。
/// Scene ビュー右上のオーバーレイメニューから表示/非表示を切り替え可能。
/// </summary>
[Overlay(typeof(SceneView), "Slow Time", true)]
public class SlowTimeOverlay : Overlay
{
    private Label _valueLabel;

    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement();
        root.style.minWidth = 200;
        root.style.paddingLeft  = 8;
        root.style.paddingRight = 8;
        root.style.paddingTop   = 6;
        root.style.paddingBottom = 6;

        var title = new Label("Time Scale");
        title.style.unityFontStyleAndWeight = FontStyle.Bold;
        title.style.marginBottom = 4;
        root.Add(title);

        var row = new VisualElement();
        row.style.flexDirection = FlexDirection.Row;
        row.style.alignItems    = Align.Center;

        var slider = new Slider(0f, 1f);
        slider.style.flexGrow = 1;
        slider.value = Time.timeScale;

        _valueLabel = new Label(Time.timeScale.ToString("F2"));
        _valueLabel.style.minWidth = 32;
        _valueLabel.style.unityTextAlign = TextAnchor.MiddleRight;

        slider.RegisterValueChangedCallback(evt =>
        {
            Time.timeScale = evt.newValue;
            Time.fixedDeltaTime = 0.02f * evt.newValue;
            _valueLabel.text = evt.newValue.ToString("F2");
        });

        row.Add(slider);
        row.Add(_valueLabel);
        root.Add(row);

        var buttonRow = new VisualElement();
        buttonRow.style.flexDirection = FlexDirection.Row;
        buttonRow.style.marginTop = 6;

        foreach (var (label, value) in new (string, float)[] { ("x0.1", 0.1f), ("x0.25", 0.25f), ("x0.5", 0.5f), ("x1", 1f) })
        {
            var btn = new Button(() =>
            {
                Time.timeScale = value;
                Time.fixedDeltaTime = 0.02f * value;
                slider.SetValueWithoutNotify(value);
                _valueLabel.text = value.ToString("F2");
            });
            btn.text = label;
            btn.style.flexGrow = 1;
            buttonRow.Add(btn);
        }

        root.Add(buttonRow);
        return root;
    }
}
#endif
