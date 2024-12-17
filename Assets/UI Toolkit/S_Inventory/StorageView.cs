using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.Inventory
{
    // 定義一個抽象類別 StorageView，繼承自 MonoBehaviour
    public abstract class StorageView : MonoBehaviour
    {
        // 定義一個 Slot 陣列，用來存放所有的槽位
        public Slot[] Slots;

        // 定義兩個受保護的序列化欄位，用來存放 UIDocument 和 StyleSheet
        [SerializeField] protected UIDocument document;
        [SerializeField] protected StyleSheet styleSheet;

        // 定義一個靜態的 VisualElement，用來表示 ghostIcon
        protected static VisualElement ghostIcon;

        // 定義兩個靜態變數，用來追蹤拖曳狀態和原始槽位
        static bool isDragging;
        static Slot originalSlot;

        // 定義兩個受保護的 VisualElement，用來表示根元素和容器
        protected VisualElement root;
        protected VisualElement container;

        // 定義一個事件，用來在拖曳結束時觸發
        public event Action<Slot, Slot> OnDrop;

        // Start 方法，在物件初始化時呼叫
        void Start()
        {
            // 為每個槽位註冊 OnPointerDown 事件
            foreach (var slot in Slots)
            {
                slot.OnStartDrag += OnPointerDown;
            }

            // 為 ghostIcon 註冊 PointerMoveEvent 和 PointerUpEvent 事件
            ghostIcon.RegisterCallback<PointerMoveEvent>(OnPointerMove);
            ghostIcon.RegisterCallback<PointerUpEvent>(OnPointerUp);
        }

        // 抽象方法 InitializeView，必須在子類別中實作
        public abstract IEnumerator InitializeView(ViewModel viewModel);

        // 靜態方法 OnPointerDown，在開始拖曳時呼叫
        static void OnPointerDown(Vector2 position, Slot slot)
        {
            isDragging = true; // 設定拖曳狀態為 true
            originalSlot = slot; // 設定原始槽位

            SetGhostIconPosition(position); // 設定 ghostIcon 的位置

            // 設定 ghostIcon 的背景圖片為原始槽位的圖片
            originalSlot.Icon.image = null; // 清空原始槽位的圖片
            originalSlot.StackLabel.visible = false; // 隱藏原始槽位的堆疊標籤
            ghostIcon.style.backgroundImage = originalSlot.BaseSprite.texture;

            ghostIcon.style.visibility = Visibility.Visible; // 顯示 ghostIcon
            // TODO: 在 ghostIcon 上顯示堆疊大小
        }

        // 方法 OnPointerMove，在拖曳過程中呼叫
        void OnPointerMove(PointerMoveEvent evt)
        {
            if (!isDragging) return; // 如果沒有在拖曳，直接返回

            SetGhostIconPosition(evt.position); // 設定 ghostIcon 的位置
        }

        // 方法 OnPointerUp，在拖曳結束時呼叫
        void OnPointerUp(PointerUpEvent evt)
        {
            if (!isDragging) return; // 如果沒有在拖曳，直接返回

            // 找到與 ghostIcon 重疊的最近的槽位
            Slot closestSlot = Slots
                .Where(slot => slot.worldBound.Overlaps(ghostIcon.worldBound))
                .OrderBy(slot => Vector2.Distance(slot.worldBound.position, ghostIcon.worldBound.position))
                .FirstOrDefault();

            if (closestSlot != null)
            {
                OnDrop?.Invoke(originalSlot, closestSlot); // 觸發 OnDrop 事件
            }
            else
            {
                originalSlot.Icon.image = originalSlot.BaseSprite.texture; // 恢復原始槽位的圖片
            }

            isDragging = false; // 設定拖曳狀態為 false
            originalSlot = null; // 清空原始槽位
            ghostIcon.style.visibility = Visibility.Hidden; // 隱藏 ghostIcon
        }

        // 靜態方法 SetGhostIconPosition，用來設定 ghostIcon 的位置
        static void SetGhostIconPosition(Vector2 position)
        {
            ghostIcon.style.top = position.y - ghostIcon.layout.height / 2; // 設定 ghostIcon 的 top 屬性
            ghostIcon.style.left = position.x - ghostIcon.layout.width / 2; // 設定 ghostIcon 的 left 屬性
        }

        // 方法 OnDestroy，在物件銷毀時呼叫
        void OnDestroy()
        {
            // 為每個槽位取消註冊 OnPointerDown 事件
            foreach (var slot in Slots)
            {
                slot.OnStartDrag -= OnPointerDown;
            }
        }
    }
}