using Firebase.Auth;
using Firebase.Functions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RankingBoards
{
    /// <summary>
    /// Firebase
    /// </summary>
    public class FirebaseService
    {
        private readonly FirebaseAuth auth;
        private readonly FirebaseFunctions functions;

        public FirebaseService()
        {
            auth = FirebaseAuth.DefaultInstance;
            functions = FirebaseFunctions.GetInstance("asia-northeast1");
        }

        /// <summary>
        /// 認証状態かどうか
        /// </summary>
        /// <returns></returns>
        public bool IsSignedIn()
        {
            return auth.CurrentUser != null;
        }

        /// <summary>
        /// 匿名認証を行う
        /// </summary>
        /// <returns></returns>
        public async Task<FirebaseUser> SignInAnonymouslyAsync()
        {
            return await auth.SignInAnonymouslyAsync();
        }

        /// <summary>
        /// サインアウト
        /// 実際にはユーザーの削除
        /// </summary>
        public Task SignOut()
        {
            return auth.CurrentUser.DeleteAsync().ContinueWith(task =>
            {
                auth.SignOut();
            });
        }

        /// <summary>
        /// Functions addEntryを実行する
        /// require: 認証済み
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public async Task<object> AddEntryAsync(RankingEntry entry)
        {
            object data = new Dictionary<object, object>
            {
                { "name", entry.Name },
                { "score", entry.Score },
            };

            return await functions.GetHttpsCallable("addEntry").CallAsync(data)
                .ContinueWith(task =>
                {
                    return task.Result.Data;
                });
        }

        /// <summary>
        /// Functions getTopEntriesを実行する
        /// require: 認証済み
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public async Task<IEnumerable<RankingEntry>> GetTopEntriesAsync(int count)
        {
            object data = new Dictionary<object, object>
            {
                { "count", count },
            };

            return await functions.GetHttpsCallable("getTopEntries").CallAsync(data)
                .ContinueWith(task =>
                {
                    var result = (Dictionary<object, object>)task.Result.Data;
                    return ((List<object>)result["entries"])
                        .Select(e => RankingEntry.CreateFromEntryObject(e));
                });
        }
    }
}
