using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class HealthBar : VisualElement
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
            if(fillPercentage == value) return;
            fillPercentage = value;
            foreground.style.width = new StyleLength(new Length(fillPercentage, LengthUnit.Percent));

            // 當 background 的 boderRadius 不為 0 的情況下（範例中為 5 個像素）。
            // fillPercent 超過一定的閾值 (BorderRaidusThreshold) 時，我們必須同樣的為 foreground 的右半部加上相同的 borderRadius。
            // 否則 foreground 渲染出的方塊將會覆蓋 background, 那樣並不美觀。
            foreground.style.borderTopRightRadius = new StyleLength(fillPercentage >= BorderRaidusThreshold ? BorderRaidus : 0);
            foreground.style.borderBottomRightRadius = new StyleLength(fillPercentage >= BorderRaidusThreshold ? BorderRaidus : 0);
        }
    }

    Color foregroundColor;
    [CreateProperty, UxmlAttribute]
    Color ForgroundColor
    {
        get => foregroundColor;
        set
        {
            if (foregroundColor == value) return;
            foregroundColor = value;
            foreground.style.backgroundColor = new StyleColor(foregroundColor);
        }
    }
    public HealthBar()
    {
        background = new VisualElement();
        foreground = new VisualElement();
        background.name = "Background";
        foreground.name = "Foreground";

        // foreground 的 width 將會隨著 fillPercent 動態變動，因此我們不能將它寫死。
        // 但是，foreground 元素的 height 則可以依它的父級元素，也就是 background 元素 height 的 100% 來設定。
        // 當這裡的 Legnth 結構體建構子的第二個參數傳入 LengthUnit.Percent 時，第一個參數傳入的浮點數參數將會是百分比值。
        foreground.style.height = new StyleLength(new Length(100, LengthUnit.Percent));


        Add(background);
        background.Add(foreground);
        //var uxml = Resources.Load<VisualTreeAsset>("HealthBar");
        //uxml.CloneTree(this);
    }
}
