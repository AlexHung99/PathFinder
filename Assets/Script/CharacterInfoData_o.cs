using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterInfo_", menuName = "Data/Character Info")]
public class CharacterInfoData_o : ScriptableObject, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    [SerializeField] private string _characterName;
    [SerializeField, Min(0)] private int _characterLevel;
    [SerializeField, Min(0)] private int _characterMaxHealth;
    [SerializeField, Min(0)] private int _characterHealth;
    [SerializeField, Min(0)] private int _characterMaxMana;
    [SerializeField, Min(0)] private int _characterMana;

    public string characterName
    {
        get => _characterName;
        set
        {
            if (_characterName != value)
            {
                _characterName = value;
                OnPropertyChanged(nameof(characterName));
            }
        }
    }

    public int characterLevel
    {
        get => _characterLevel;
        set
        {
            if (_characterLevel != value)
            {
                _characterLevel = value;
                OnPropertyChanged(nameof(characterLevel));
            }
        }
    }

    public int characterMaxHealth
    {
        get => _characterMaxHealth;
        set
        {
            if (_characterMaxHealth != value)
            {
                _characterMaxHealth = value;
                OnPropertyChanged(nameof(characterMaxHealth));
            }
        }
    }

    public int characterHealth
    {
        get => _characterHealth;
        set
        {
            if (_characterHealth != value)
            {
                _characterHealth = value;
                OnPropertyChanged(nameof(characterHealth));
            }
        }
    }

    public int characterMaxMana
    {
        get => _characterMaxMana;
        set
        {
            if (_characterMaxMana != value)
            {
                _characterMaxMana = value;
                OnPropertyChanged(nameof(characterMaxMana));
            }
        }
    }

    public int characterMana
    {
        get => _characterMana;
        set
        {
            if (_characterMana != value)
            {
                _characterMana = value;
                OnPropertyChanged(nameof(characterMana));
            }
        }
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}