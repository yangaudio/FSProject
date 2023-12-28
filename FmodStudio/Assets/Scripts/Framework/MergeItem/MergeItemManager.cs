using System.Collections.Generic;
using ThGold.Common;
using ThGold.Table;

namespace ThGold {
    public class MergeItemManager : MonoSingleton<MergeItemManager> {
        private List<MergeItem> MergeItemList;

        private void Initialize() {
            MergeItemList = new List<MergeItem>();
            foreach (var kvp in mergeitemsdata.Instance.Datas) {
                mergeitems data = kvp.Value;
                MergeItem item = new MergeItem(data.ResultID, data.ItemID1, data.ItemID2);
                MergeItemList.Add(item);
            }
        }

        public bool TryMergeItems(int itemId1, int itemId2, out int resultItemId) {
            resultItemId = -1;

            // 查找是否存在合成关系
            MergeItem mergeItem = MergeItemList.Find(item =>
                (item.ingredients.Contains(itemId1) && item.ingredients.Contains(itemId2)) ||
                (item.ingredients.Contains(itemId2) && item.ingredients.Contains(itemId1)));

            if (mergeItem != null) {
                resultItemId = mergeItem.resultItemId;
                return true; // 合成成功
            }

            return false; // 合成失败
        }
    }
}