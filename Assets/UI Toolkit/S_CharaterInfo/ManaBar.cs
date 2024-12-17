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

            // �� background �� boderRadius ���� 0 �����p�U�]�d�Ҥ��� 5 �ӹ����^�C
            // fillPercent �W�L�@�w���H�� (BorderRaidusThreshold) �ɡA�ڭ̥����P�˪��� foreground ���k�b���[�W�ۦP�� borderRadius�C
            // �_�h foreground ��V�X������N�|�л\ background, ���˨ä����[�C
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