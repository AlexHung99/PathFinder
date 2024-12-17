using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.Inventory
{
    // 定義 Slot 類別，繼承自 VisualElement
    public class Slot : VisualElement
    {
        // 定義 Icon 屬性，表示槽位的圖標
        public Image Icon;
        // 定義 StackLabel 屬性，表示堆疊數量的標籤
        public Label StackLabel;
        // 定義 Index 屬性，返回槽位在父元素中的索引
        public int Index => parent.IndexOf(this);
        // 定義 ItemId 屬性，表示槽位中的物品 ID，預設為空
        public SerializableGuid ItemId { get; private set; } = SerializableGuid.Empty;
        // 定義 BaseSprite 屬性，表示槽位的基礎圖標
        public Sprite BaseSprite;

        // 定義 OnStartDrag 事件，當開始拖曳時觸發
        public event Action<Vector2, Slot> OnStartDrag = delegate { };

        // Slot 類別的建構函式
        public Slot()
        {
            // 創建 Icon 元素，並設置為槽位的子元素
            Icon = this.CreateChild<Image>("slotIcon");
            // 創建 StackLabel 元素，並設置為槽位的子元素
            StackLabel = this.CreateChild("slotFrame").CreateChild<Label>("stackCount");
            // 註冊 PointerDown 事件的回調函式
            RegisterCallback<PointerDownEvent>(OnPointerDown);
        }

        // 當 PointerDown 事件觸發時的回調函式
        void OnPointerDown(PointerDownEvent evt)
        {
            // 如果按下的不是左鍵或槽位為空，則返回
            if (evt.button != 0 || ItemId.Equals(SerializableGuid.Empty)) return;

            // 觸發 OnStartDrag 事件
            OnStartDrag.Invoke(evt.position, this);
            // 停止事件的傳播
            evt.StopPropagation();
        }

        // 設置槽位的物品
        public void Set(SerializableGuid id, Sprite icon, int qty = 0)
        {
            // 設置 ItemId
            ItemId = id;
            // 設置 BaseSprite
            BaseSprite = icon;

            // 設置 Icon 的圖像
            Icon.image = BaseSprite != null ? icon.texture : null;

            // 設置 StackLabel 的文字
            StackLabel.text = qty > 1 ? qty.ToString() : string.Empty;
            // 設置 StackLabel 的可見性
            StackLabel.visible = qty > 1;
        }

        // 移除槽位的物品
        public void Remove()
        {
            // 設置 ItemId 為空
            ItemId = SerializableGuid.Empty;
            // 清空 Icon 的圖像
            Icon.image = null;
        }
    }
}