using UnityEngine;
using UnityEngine.UIElements;

public class CharacterInfoController : MonoBehaviour
{
    public CharacterInfoData_o characterInfo;
    public VisualTreeAsset uxml;

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        uxml.CloneTree(root);

        var nameLabel = root.Q<Label>("Chr_Name");
        var levelLabel = root.Q<Label>("Chr_Level");
        var hpLabel = root.Q<Label>("Chr_HP");
        var mpLabel = root.Q<Label>("Chr_MP");

        BindLabel(nameLabel, nameof(characterInfo.characterName));
        BindLabel(levelLabel, nameof(characterInfo.characterLevel));
        BindLabel(hpLabel, nameof(characterInfo.characterHealth));
        BindLabel(mpLabel, nameof(characterInfo.characterMana));
    }

    private void BindLabel(Label label, string propertyName)
    {
        label.text = GetPropertyValue(propertyName).ToString();
        characterInfo.PropertyChanged += (sender, args) =>
        {
            if (args.PropertyName == propertyName)
            {
                label.text = GetPropertyValue(propertyName).ToString();
            }
        };
    }

    private object GetPropertyValue(string propertyName)
    {
        return characterInfo.GetType().GetProperty(propertyName).GetValue(characterInfo, null);
    }
}