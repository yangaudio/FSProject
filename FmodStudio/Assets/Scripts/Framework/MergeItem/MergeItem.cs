using System.Collections.Generic;

namespace ThGold {
    [System.Serializable]
    public class MergeItem {
        public List<int> ingredients; // 合成所需的物品 ID 列表
        public int resultItemId; // 合成完成后生成的物品 ID

        public MergeItem(int _resultItemId, int id1, int id2) {
            ingredients = new List<int>();
            ingredients.Add(id1);
            ingredients.Add(id2);
            resultItemId = _resultItemId;
        }
    }
}