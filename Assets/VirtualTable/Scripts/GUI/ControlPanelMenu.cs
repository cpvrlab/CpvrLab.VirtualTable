using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

namespace CpvrLab.VirtualTable
{

    public class ControlPanelMenu : NetworkBehaviour
    {
        public RectTransform localPlayerSettings;
        public RectTransform spectatorSettings;
        public RectTransform gameSettings;
        public RectTransform adminSettings;
        public RectTransform container;

        public Dropdown gameSelect;
        public Button gameStartButton;
        public Button gameStopButton;
        public Button kickPlayerButton;
        public Button resetAllPlayersButton;
        public Button resetAllItemsButton;
        public Button spectateToggleButton;
        public Button disconnectButton;

        public float[] heights;

        public float edgeActivationRange = 10.0f;


        private bool _rebuild = true;
        private bool _isVisible = false;
        private Animator _animator;
        private GameManager _gameManager;
        private VTNetworkManager _networkManager;

        private GamePlayer _localPlayer = null;
        private bool _isAdmin = false;
        private bool _isObserver = false;



        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        void Start()
        {
            _networkManager = NetworkManager.singleton as VTNetworkManager;
            _gameManager = GameManager.instance;

            if(GameManager.instance == null)
                Debug.Log("GAME MANAGER IS NULL");
            else
                Debug.Log("Game manager is not null");

            if (GamePlayer.localPlayer != null)
                _isObserver = GamePlayer.localPlayer.isObserver;
            else
                GamePlayer.OnLocalPlayerCreated += LocalPlayerCreated;

            _isAdmin = NetworkServer.active;
            _rebuild = true;

            // connect button events
            // we could also do this in the editor but I like it better this way because it is more centralized.
            gameStartButton.onClick.AddListener(OnStartGameClicked);
            gameStopButton.onClick.AddListener(OnStopGameClicked);
            kickPlayerButton.onClick.AddListener(OnKickPlayerClicked);
            resetAllPlayersButton.onClick.AddListener(OnResetAllPlayersClicked);
            resetAllItemsButton.onClick.AddListener(OnResetAllItemsClicked);
            spectateToggleButton.onClick.AddListener(OnSpectateClicked);
            disconnectButton.onClick.AddListener(OnDisconnectClicked);

            StartCoroutine(tempGetGameManagerInstance());
        }

        IEnumerator tempGetGameManagerInstance()
        {
            while (_gameManager == null)
            {
                _gameManager = GameManager.instance;
                yield return null;
            }
        }

        void OnStartGameClicked()
        {
            Debug.Log("OnStartGameClicked");
            if (_gameManager == null) Debug.Log("GameManger is NULL");   
            _gameManager.StartGame(gameSelect.value);
        }

        void OnStopGameClicked()
        {
            Debug.Log("OnStopGameClicked");
            _gameManager.StopGame();
        }

        void OnKickPlayerClicked()
        {
            Debug.Log("OnKickPlayerClicked");
        }

        void OnResetAllPlayersClicked()
        {
            Debug.Log("OnResetAllPlayersClicked");
        }

        void OnResetAllItemsClicked()
        {
            Debug.Log("OnResetAllItemsClicked");
        }

        void OnSpectateClicked()
        {
            Debug.Log("OnSpectateClicked");
        }

        void OnDisconnectClicked()
        {
            Debug.Log("OnDisconnectClicked");
        }
        
        void LocalPlayerCreated(GamePlayer player)
        {
            _localPlayer = player;
            _isObserver = player.isObserver;
            ForceRebuild();
        }

        void Update()
        {

            UpdateVisible();
            if (_rebuild) RebuildMenu();
        }

        public void ForceRebuild()
        {
            _rebuild = true;
        }

        void UpdateGamesList()
        {
            if (_gameManager == null)
                return;

            gameSelect.options.Clear();

            for(int i = 0; i < _gameManager.games.Length; i++)
            {
                var game = _gameManager.games[i];
                gameSelect.options.Add(new Dropdown.OptionData(game.gameName));
            }
            

            gameSelect.RefreshShownValue();
        }

        void RebuildMenu()
        {            
            float margin = 10.0f;
            float currentY = 0.0f;
            currentY -= localPlayerSettings.rect.height + margin;
            
            // show or hide spectator settings
            spectatorSettings.gameObject.SetActive(_isObserver);
            if (_isObserver)
                currentY -= spectatorSettings.rect.height + margin;
            
            // game settings position
            var newPos = gameSettings.anchoredPosition;
            newPos.y = currentY;
            gameSettings.anchoredPosition = newPos;
            currentY -= gameSettings.rect.height + margin;
            
            // admin settings position
            newPos = adminSettings.anchoredPosition;
            newPos.y = currentY;
            adminSettings.anchoredPosition = newPos;
            adminSettings.gameObject.SetActive(_isAdmin);
            
            // retrieve available games list
            UpdateGamesList();            
            _rebuild = false;
        }


        void UpdateVisible()
        {
            var x = Input.mousePosition.x;
            var y = Input.mousePosition.y;

            if (_isVisible)
            {
                var rect = container.rect;
                if (x < 0.0f || rect.width < x || y < 0.0f || rect.height < y)
                    HideMenu();
            }
            else
            {
                if (!(x < 0.0f || edgeActivationRange < x || y < 0.0f || Screen.height < y))
                    ShowMenu();
            }
        }


        void HideMenu()
        {
            _isVisible = false;
            _animator.SetBool("isVisible", _isVisible);
        }

        void ShowMenu()
        {
            _isVisible = true;
            _animator.SetBool("isVisible", _isVisible);
        }
    }

}