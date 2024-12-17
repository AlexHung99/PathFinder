using Unity.Properties;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInfo_", menuName = "Data/Character Info")]
public class CharacterInfoData : ScriptableObject
{
    [DontCreateProperty, SerializeField] private string _characterName;
    [DontCreateProperty, SerializeField, Min(1)] private int _characterLevel;
    [DontCreateProperty, SerializeField, Min(0)] private int _characterMaxHealth;
    [DontCreateProperty, SerializeField, Min(0)] private int _characterHealth;
    [DontCreateProperty, SerializeField, Min(0)] private int _characterMaxMana;
    [DontCreateProperty, SerializeField, Min(0)] private int _characterMana;
    [DontCreateProperty, SerializeField] Gradient healthBarGradient;


    [CreateProperty] string CharacterLevelString => $"Level:{_characterLevel}";
    [CreateProperty] string CharacterNameString => $"{_characterName}";
    [CreateProperty] string CharacterMaxHealthString => $"MaxHealth:{_characterMaxHealth}";
    [CreateProperty] public string CharacterHealthString => $"Health:{_characterHealth} / {_characterMaxHealth}";
    [CreateProperty] public string CharacterManaString => $"Mana:{_characterMana} / {_characterMaxMana}";

    [CreateProperty] int HealthPercentageInt => Mathf.RoundToInt(HealthPercentage * 100);
    [CreateProperty] string HealthPercentageString => HealthPercentage.ToString("P0", System.Globalization.CultureInfo.InvariantCulture);
    [CreateProperty] int ManaPercentageInt => Mathf.RoundToInt(ManaPercentage * 100);
    [CreateProperty] string ManaPercentageString => ManaPercentage.ToString("P0", System.Globalization.CultureInfo.InvariantCulture);

    [CreateProperty] Color HealthBarColor => healthBarGradient.Evaluate(HealthPercentage);

    float HealthPercentage => (float)_characterHealth / _characterMaxHealth;
    float ManaPercentage => (float)_characterMana / _characterMaxMana;

    public int CharacterHealth
    {
        get => _characterHealth;
        set
        {
            if (_characterHealth == value) return;
            _characterHealth = Mathf.Clamp(value, 0, _characterMaxHealth);
        }
    }

}
