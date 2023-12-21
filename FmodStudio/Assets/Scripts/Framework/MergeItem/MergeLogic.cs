using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ThGold {
    public static class MergeLogic {
        public static bool TryMergeItems(int itemId1, int itemId2, List<MergeItem> mergeItems, out int resultItemId) {
            resultItemId = -1;

            // 查找是否存在合成关系
            MergeItem mergeItem = mergeItems.Find(item =>
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