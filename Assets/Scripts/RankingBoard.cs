using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RankingBoards
{
    /// <summary>
    /// ひとつのレコード情報
    /// </summary>
    public class RankingEntry
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">名前</param>
        /// <param name="score">スコア</param>
        public RankingEntry(string name, int score)
        {
            Name = name;
            Score = score;
        }

        /// <summary>
        /// 登録された名前
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 登録されたスコア
        /// </summary>
        public int Score { get; private set; }

        /// <summary>
        /// ToString実装
        /// ほげ, 12345
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}, {1}", Name, Score);
        }
        
        /// <summary>
        /// 通信結果で取得されるjsObjectからのコンバートを行う
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>新しいEntryインスタンス</returns>
        public static RankingEntry CreateFromEntryObject(object obj)
        {
            Dictionary<object, object> entry = (Dictionary<object, object>)obj;
            return new RankingEntry(
                name: (string)entry["name"],
                score: Convert.ToInt32(entry["score"])
                );
        }
    }

    /// <summary>
    /// ランキング情報
    /// </summary>
    public class RankingBoard
    {
        //  ランキングを取得する際のレコード数
        private const int FetchCount = 20;

        private readonly FirebaseService service;

        /// <summary>
        /// 取得されたすべてのレコード
        /// </summary>
        public List<RankingEntry> Entries { get; private set; } = new List<RankingEntry>();

        /// <summary>
        /// Firebaseインスタンスを指定して初期化
        /// </summary>
        /// <param name="service"></param>
        public RankingBoard(FirebaseService service)
        {
            this.service = service;
        }

        /// <summary>
        /// レコードの登録
        /// コールバックはメインスレッドで実行される
        /// </summary>
        /// <param name="entry">登録するレコード</param>
        /// <param name="onComplete">成功時処理</param>
        /// <param name="onError">失敗時処理</param>
        public void AddEntry(RankingEntry entry, Action onComplete, Action<AggregateException> onError)
        {
            service.AddEntryAsync(entry).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    onError?.Invoke(task.Exception);
                }
                else
                {
                    onComplete?.Invoke();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        /// <summary>
        /// トップのランキングを取得する
        /// コールバックはメインスレッドで実行される
        /// </summary>
        /// <param name="onComplete">成功時処理</param>
        /// <param name="onError">失敗時処理</param>
        public void UpdateEntries(Action onComplete, Action<AggregateException> onError)
        {
            service.GetTopEntriesAsync(FetchCount).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    onError?.Invoke(task.Exception);
                }
                else
                {
                    Entries = new List<RankingEntry>(task.Result);
                    onComplete?.Invoke();
                }
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
