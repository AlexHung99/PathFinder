using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;


[UxmlElement]
public partial class ManaBar : VisualElement
{
    readonly VisualElement background;
    readonly VisualElement foreground;

    int fillPercentage = 50;
    const float BorderRaidus = 5;
    const float BorderRaidusThreshold = 98;

    [CreateProperty, UxmlAttribute, Range(0, 100)]
    int FillPercentage
    {
        get => fillPercentage;
        set
        {
            if (fillPercentage == value) return;
            fillPercentage = value;
            foreground.style.width = new StyleLength(new Length(fillPercentage, LengthUnit.Percent));

            // 當 background 的 boderRadius 不為 0 的情況下（範例中為 5 個像素）。
            // fillPercent 超過一定的閾值 (BorderRaidusThreshold) 時，我們必須同樣的為 foreground 的右半部加上相同的 borderRadius。
            // 否則 foreground 渲染出的方塊將會覆蓋 background, 那樣並不美觀。
            foreground.style.borderTopRightRadius = new StyleLength(fillPercentage >= BorderRaidusThreshold ? BorderRaidus : 0);
            foreground.style.borderBottomRightRadius = new StyleLength(fillPercentage >= BorderRaidusThreshold ? BorderRaidus : 0);
        }
    }
    public ManaBar()
    {
        background = new VisualElement();
        foreground = new VisualElement();
        background.name = "Background";
        foreground.name = "Foreground";

        foreground.style.height = new StyleLength(new Length(100, LengthUnit.Percent));

        Add(background);
        background.Add(foreground);
    }
}