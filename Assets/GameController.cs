using System;
using LowoUN.UI;
using UnityEngine;

namespace LowoUN.Scene {
    public class GameController : MonoBehaviour {
        public static GameController Instance { get; private set; }
        private GameState oldState = GameState.None;
        private GameState curState = GameState.None;
        // public bool isLoginSucc => oldState == GameState.None || oldState == GameState.Splash;
        public bool isBattle { get { return curState == GameState.Battle; } }
        public bool isLobby { get { return curState == GameState.Lobby; } }

        void Awake () {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad (this);
            }
        }

        public void SetState (GameState state) {
            Debug.Log ($"scene_ [Old] Scene: {oldState.ToString ()} [Cur] Scene: {curState.ToString ()}");
            if (curState == state)
                return;

            UIManager.Instance.ShowLoadingUI (state);

            // GuideManager.Instance.DestroyGuidePanel ();
            // PlayerData.Instance.SaveAllData ();

            EndScene ();

            switch (state) {
                case GameState.Splash:
                    SceneLoader.Instance.LoadScene_New ("Splash", UIManager.Instance.LoadingProgress, () => { StartScene (state, SplashController.Instance.Enter___); });
                    break;
                case GameState.Login:
                    SceneLoader.Instance.LoadScene_New ("Login", UIManager.Instance.LoadingProgress, () => { StartScene (state, LoginController.Instance.Enter___); });
                    break;
                case GameState.Lobby:
                    SceneLoader.Instance.LoadScene_New ("Lobby", UIManager.Instance.LoadingProgress, () => { StartScene (state, LobbyController.Instance.Enter___); });
                    break;
                case GameState.Battle:
                    SceneLoader.Instance.LoadScene_New ("Battle", UIManager.Instance.LoadingProgress, () => { StartScene (state, BattleController.Instance.Enter___); });
                    break;
                default:
                    Debug.LogError ($"scene_ No Game Sate Found: {state}");
                    break;
            }
        }

        public void StartScene (GameState state, Action cb) {
            Debug.Log ($"scene_ 切场景之前的 GameController StartScene() [Old] Scene: {oldState.ToString ()}");
            Debug.Log ($"scene_ 切场景之前的 GameController StartScene() [Cur] Scene: {curState.ToString ()}");
            oldState = curState;
            curState = state;
            Debug.Log ($"scene_ 本次切换场景 GameController StartScene() [Old] Scene: {oldState.ToString ()}");
            Debug.Log ($"scene_ 本次切换场景 GameController StartScene() [Cur] Scene: {curState.ToString ()}");

            // AudioManager.Instance.PlayBGM(bgm);

            UIManager.Instance.HideLoadingUI ();

            cb ();
        }

        private void EndScene () {
            if ((int) curState < (int) GameState.None) {
                Debug.LogError ($"scene_ GameController EndScene: {curState.ToString ()} is invalid!!!");
                return;
            }

            Debug.Log ($"scene_ GameController EndScene: {curState.ToString ()}");

            void CommonEnd () {
                //1，清理 脚本相关
                // LuaEngine.Instance.EndScene (curSceneName);
                //2，清理 资源
                Resources.UnloadUnusedAssets ();
                //3，清理 C#内存
                System.GC.Collect ();
            }

            CommonEnd ();
            switch (curState) {
                case GameState.Login:
                    LoginController.Instance.Exit___ ();
                    break;
                case GameState.Lobby:
                    LobbyController.Instance.Exit___ ();
                    break;
                case GameState.Battle:
                    BattleController.Instance.Exit___ ();
                    break;
                default:
                    Debug.Log ($"scene_ No Game Sate Found: {curState.ToString ()}");
                    break;
            }
        }

        // void Update () { }

        // void FixedUpdate () { }

        void OnGUI () {
            GUI.skin.button.fontSize = 24;

            if (GUI.Button (new Rect (10, 10, 160, 40), ">>> Login"))
                SetState (GameState.Login);

            if (GUI.Button (new Rect (180, 10, 160, 40), ">>> Lobby"))
                SetState (GameState.Lobby);

            if (GUI.Button (new Rect (360, 10, 160, 40), ">>> Battle"))
                SetState (GameState.Battle);
        }
    }
}