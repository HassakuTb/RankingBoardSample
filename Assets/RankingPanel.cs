using System.Linq;
using UnityEngine;

namespace RankingBoards
{
    /// <summary>
    /// ランキングボードUI
    /// </summary>
    public class RankingPanel : MonoBehaviour
    {
        [Header("prefab")]
        [SerializeField] private RankingEntryPanel entryPanelPrefab;

        [Space]
        [SerializeField] private Transform content;

        /// <summary>
        /// 現在表示されているランキングを削除する
        /// </summary>
        private void RemoveAllContent()
        {
            foreach(Transform t in content)
            {
                Destroy(t.gameObject);
            }

            content.DetachChildren();
        }

        /// <summary>
        /// ランキング表示を更新する
        /// </summary>
        /// <param name="ranking"></param>
        public void SetRanking(RankingBoard ranking)
        {
            RemoveAllContent();

            foreach(var element in ranking.Entries.Select((entry, index) => new { entry, index }))
            {
                RankingEntryPanel panel = RankingEntryPanel.CreateRankingEntryPanel(
                    prefab: entryPanelPrefab,
                    number: element.index + 1,
                    entry: element.entry);

                panel.transform.SetParent(content, worldPositionStays: false);
            }
        }
    }
}
