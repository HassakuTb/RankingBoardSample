using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace RankingBoards
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] private Button signInButton;
        [SerializeField] private InputField nameField;
        [SerializeField] private InputField scoreField;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button fetchButton;

        [SerializeField] private RankingPanel rankingPanel;

        private FirebaseService service;
        private RankingBoard ranking;

        private void Start()
        {
            service = new FirebaseService();
            ranking = new RankingBoard(service);

            //  サインインの実行
            signInButton.onClick.AddListener(() =>
            {
                service.SignInAnonymouslyAsync().ContinueWith(task =>
                {
                    if (!task.IsFaulted)
                    {
                        //  成功時
                        Debug.Log("Authentication completed");
                        Debug.Log("uid : " + task.Result.UserId);
                    }
                    else
                    {
                        //  失敗時
                        Debug.LogException(task.Exception);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            });

            //  スコアの登録
            registerButton.onClick.AddListener(() =>
            {
                ranking.AddEntry(
                    entry: new RankingEntry(nameField.text, int.Parse(scoreField.text)),
                    onComplete: () =>
                    {
                        //  成功時
                        Debug.Log("registered");
                    },
                    onError: (exception) =>
                    {
                        //  失敗時
                        Debug.LogException(exception);
                    });
            });

            //  ランキングの取得
            fetchButton.onClick.AddListener(() =>
            {
                ranking.UpdateEntries(
                    onComplete: () =>
                    {
                        //  成功時
                        ranking.Entries.ForEach(e => Debug.Log(e.ToString()));
                        rankingPanel.SetRanking(ranking);
                    },
                    onError: (exception) =>
                    {
                        //  失敗時
                        Debug.LogException(exception);
                    });
            });
        }

        private void OnDisable()
        {
            service.SignOut().ContinueWith(task =>
            {
                Debug.Log("delete user done");
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
