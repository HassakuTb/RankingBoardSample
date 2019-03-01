using UnityEngine;
using UnityEngine.UI;

namespace RankingBoards
{
    /// <summary>
    /// ランキングボード内のひとつのレコード
    /// </summary>
    public class RankingEntryPanel : MonoBehaviour
    {
        [SerializeField] private Text numberText;
        [SerializeField] private Text nameText;
        [SerializeField] private Text scoreText;

        /// <summary>
        /// レコードのGameObjectを生成する
        /// </summary>
        /// <param name="prefab">コピー元オブジェクト</param>
        /// <param name="number">順位</param>
        /// <param name="entry">レコード情報</param>
        /// <returns></returns>
        public static RankingEntryPanel CreateRankingEntryPanel(RankingEntryPanel prefab, int number, RankingEntry entry)
        {
            RankingEntryPanel panel = Instantiate(prefab);
            panel.SetRankingEntry(number, entry);

            return panel;
        }

        /// <summary>
        /// UI要素を設定する
        /// </summary>
        /// <param name="number">順位</param>
        /// <param name="entry">レコード情報</param>
        private void SetRankingEntry(int number, RankingEntry entry)
        {
            numberText.text = number.ToString();
            nameText.text = entry.Name;
            scoreText.text = entry.Score.ToString();
        }
    }
}
