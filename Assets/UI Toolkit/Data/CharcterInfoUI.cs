using Unity.Properties; // 引入 Unity.Properties 命名空間
using UnityEngine; // 引入 UnityEngine 命名空間
using UnityEngine.UIElements; // 引入 UnityEngine.UIElements 命名空間

public class CharcterInfoUI : MonoBehaviour // 定義 CharcterInfoUI 類別，繼承自 MonoBehaviour
{
    [SerializeField] CharacterInfoData characterInfoData; // 序列化欄位 characterInfoData，類型為 CharacterInfoData
    private void OnEnable() // 當物件啟用時調用此方法
    {
        VisualElement root = GetComponent<UIDocument>().rootVisualElement; // 獲取 UIDocument 的根 VisualElement
        Label chrHealthLabel = root.Q<Label>("chrHealthLabel"); // 查找 ID 為 "chrHealthLabel" 的 Label 元素

        // 設置 chrHealthLabel 的綁定
        //chrHealthLabel.SetBinding(nameof(chrHealthLabel.text), new DataBinding
        //{
        //    dataSource = characterInfoData, // 設置資料來源為 characterInfoData
        //    dataSourcePath = new PropertyPath(nameof(characterInfoData.CharacterHealthString)), // 設置資料來源路徑
        //    bindingMode = BindingMode.ToTarget // 設置綁定模式為 ToTarget
        //});
    }
    void Update() // 每幀調用一次此方法
    {
        if (Input.GetKey(KeyCode.Q)) // 如果按下 Q 鍵
        {
            characterInfoData.CharacterHealth--; // 減少 characterInfoData 的 CharacterHealth 屬性
        }

        if (Input.GetKey(KeyCode.E)) // 如果按下 E 鍵
        {
            characterInfoData.CharacterHealth++; // 增加 characterInfoData 的 CharacterHealth 屬性
        }
    }
}