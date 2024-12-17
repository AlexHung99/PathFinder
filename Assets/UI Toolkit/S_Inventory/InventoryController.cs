using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Systems.Inventory
{
    // 定義 ViewModel 類別
    public class ViewModel
    {
        // 定義只讀的容量屬性
        public readonly int Capacity;
        // 定義只讀的可綁定屬性 Coins
        public readonly BindableProperty<string> Coins;

        // ViewModel 的建構函式，接受 InventoryModel 和容量作為參數
        public ViewModel(InventoryModel model, int capacity)
        {
            Capacity = capacity;
            // 將 Coins 綁定到 model 的 Coins 屬性
            Coins = BindableProperty<string>.Bind(() => model.Coins.ToString());
        }
    }

    // 定義 InventoryController 類別
    public class InventoryController
    {
        // 定義只讀的 InventoryView 變數
        readonly InventoryView view;
        // 定義只讀的 InventoryModel 變數
        readonly InventoryModel model;
        // 定義只讀的容量變數
        readonly int capacity;

        // InventoryController 的建構函式，接受 InventoryView、InventoryModel 和容量作為參數
        InventoryController(InventoryView view, InventoryModel model, int capacity)
        {
            // 確保 view 不為 null
            Debug.Assert(view != null, "View is null");
            // 確保 model 不為 null
            Debug.Assert(model != null, "Model is null");
            // 確保容量大於 0
            Debug.Assert(capacity > 0, "Capacity is less than 1");
            this.view = view;
            this.model = model;
            this.capacity = capacity;

            // 啟動初始化協程
            view.StartCoroutine(Initialize());
        }

        // 綁定 InventoryData 到 model
        public void Bind(InventoryData data) => model.Bind(data);

        // 增加指定數量的金幣到 model
        public void AddCoins(int amount) => model.AddCoins(amount);

        // 初始化協程
        IEnumerator Initialize()
        {
            // 初始化 view
            yield return view.InitializeView(new ViewModel(model, capacity));

            // 增加 10 個金幣到 model
            model.AddCoins(10);

            // 註冊 OnDrop 事件處理程序
            view.OnDrop += HandleDrop;
            // 註冊 OnModelChanged 事件處理程序
            model.OnModelChanged += HandleModelChanged;

            // 刷新 view
            RefreshView();
        }

        // 處理拖曳事件
        void HandleDrop(Slot originalSlot, Slot closestSlot)
        {
            // 如果拖曳到相同的槽位或空槽位
            if (originalSlot.Index == closestSlot.Index || closestSlot.ItemId.Equals(SerializableGuid.Empty))
            {
                // 交換槽位
                model.Swap(originalSlot.Index, closestSlot.Index);
                return;
            }

            // TODO: 處理世界掉落
            // TODO: 處理跨背包掉落
            // TODO: 處理快捷欄掉落

            // 如果拖曳到非空槽位
            var sourceItemId = model.Get(originalSlot.Index).details.Id;
            var targetItemId = model.Get(closestSlot.Index).details.Id;

            // 如果目標槽位的物品與來源槽位的物品相同且可堆疊
            if (sourceItemId.Equals(targetItemId) && model.Get(closestSlot.Index).details.maxStack > 1)
            {
                // 合併物品
                model.Combine(originalSlot.Index, closestSlot.Index);
            }
            else
            {
                // 交換槽位
                model.Swap(originalSlot.Index, closestSlot.Index);
            }


        }

        // 處理模型變更事件
        void HandleModelChanged(IList<Item> items) => RefreshView();

        // 刷新 view
        void RefreshView()
        {
            for (int i = 0; i < capacity; i++)
            {
                var item = model.Get(i);
                if (item == null || item.Id.Equals(SerializableGuid.Empty))
                {
                    // 如果槽位為空，設置為空槽位
                    view.Slots[i].Set(SerializableGuid.Empty, null);
                }
                else
                {
                    // 設置槽位的物品
                    view.Slots[i].Set(item.Id, item.details.Icon, item.quantity);
                }
            }
        }

        #region Builder

        // 定義 Builder 類別，用於構建 InventoryController
        public class Builder
        {
            InventoryView view;
            IEnumerable<ItemDetails> itemDetails;
            int capacity = 20;

            // Builder 的建構函式，接受 InventoryView 作為參數
            public Builder(InventoryView view)
            {
                this.view = view;
            }

            // 設置初始物品
            public Builder WithStartingItems(IEnumerable<ItemDetails> itemDetails)
            {
                this.itemDetails = itemDetails;
                return this;
            }

            // 設置容量
            public Builder WithCapacity(int capacity)
            {
                this.capacity = capacity;
                return this;
            }

            // 構建 InventoryController
            public InventoryController Build()
            {
                InventoryModel model = itemDetails != null
                    ? new InventoryModel(itemDetails, capacity)
                    : new InventoryModel(Array.Empty<ItemDetails>(), capacity);

                return new InventoryController(view, model, capacity);
            }
        }

        #endregion Builder
    }
}