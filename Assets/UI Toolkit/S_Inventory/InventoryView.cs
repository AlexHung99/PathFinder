using System.Collections;
using Unity.Properties;
using UnityEngine;
using UnityEngine.UIElements;

namespace Systems.Inventory
{
    // 定義一個 InventoryView 類別，繼承自 StorageView
    public class InventoryView : StorageView
    {
        // 定義一個序列化的字串變數 panelName，預設值為 "Inventory"
        [SerializeField] string panelName = "Inventory";

        // 覆寫 StorageView 的抽象方法 InitializeView
        public override IEnumerator InitializeView(ViewModel viewModel)
        {
            // 初始化 Slots 陣列，大小為 viewModel 的容量
            Slots = new Slot[viewModel.Capacity];
            // 獲取根元素
            root = document.rootVisualElement;
            // 清空根元素的所有子元素
            root.Clear();

            // 將樣式表添加到根元素
            root.styleSheets.Add(styleSheet);

            // 創建一個容器元素，並將其添加到根元素
            container = root.CreateChild("container");

            // 創建一個 inventory 元素，並添加拖曳操作
            var inventory = container.CreateChild("inventory").WithManipulator(new PanelDragManipulator());
            // 創建 inventoryFrame 元素，並添加到 inventory
            inventory.CreateChild("inventoryFrame");
            // 創建 inventoryHeader 元素，並添加一個標籤，顯示 panelName
            inventory.CreateChild("inventoryHeader").Add(new Label(panelName));

            // 創建 slotsContainer 元素，並添加到 inventory
            var slotsContainer = inventory.CreateChild("slotsContainer");

            // 根據 viewModel 的容量，創建對應數量的 Slot 元素，並添加到 slotsContainer
            for (int i = 0; i < viewModel.Capacity; i++)
            {
                var slot = slotsContainer.CreateChild<Slot>("slot");
                Slots[i] = slot;
                //slot.tabIndex = 1000;
                slot.tabIndex = 10; // 設定原始槽位的 tabIndex 為 0
                slot.style.top = -10; // 設定原始槽位的 top 屬性為 0
                slot.style.left = 10; // 設定原始槽位的 left 屬性為 0
                //slot.style.opacity = 0.5f; // 設定槽位的透明度為 50%
                foreach(var child in slot.Children())
                {
                    //ebug.Log(child.GetType());
                    if (typeof(Image) == child.GetType())
                    {
                        child.tabIndex = 15;

                    }
                    
                    if (typeof(VisualElement) == child.GetType())
                    {
                        child.style.opacity = 0.5f;

                    }

                }
                //slot.CreateChild("slotFrame").tabIndex = 5;
            }

            // 創建 coins 元素，並添加到 inventory
            var coins = inventory.CreateChild("coins");
            // 創建一個標籤元素，並添加到 coins
            var coinsLabel = new Label();
            coins.CreateChild("coinsIcon");
            coins.Add(coinsLabel);
            // 將 coins 的資料來源設置為 viewModel 的 Coins
            coins.dataSource = viewModel.Coins;

            // 綁定 coinsLabel 的文字屬性到 viewModel 的 Coins
            coinsLabel.SetBinding(nameof(Label.text), new DataBinding
            {
                dataSourcePath = new PropertyPath(nameof(BindableProperty<string>.Value)),
                bindingMode = BindingMode.ToTarget
            });

            // 創建 ghostIcon 元素，並添加到根元素
            ghostIcon = container.CreateChild("ghostIcon");

            // 等待一幀
            yield return null;
        }
    }
}