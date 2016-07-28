using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace CpvrLab.VirtualTable
{

    class BalloonShooterPlayerData : GamePlayerData
    {
        public PrototypeGun gun;
        public int gamesWon;
        public float bestTime;
    }

    /// <summary>
    /// Balloon shooter game is played as follows: In the center of the play area a bunch of color coded
    /// balloons are released. Every payer gets an item that lets them pop thse balloons, for example a 
    /// gun or a bow. The players can't shoot immediatly however. The game will display a timer befor a single 
    /// balloon color is revelaed at which point the players have to shoot the matching balloon.
    /// 
    /// alternative idea:   Have a bunch of colored balloons released. Have a timer run down for maybe 30 seconds
    ///                     As the timer runs down more and more balloons will lose their color and fade to grey.
    ///                     Grey balloons reveal to the player that they aren't the target. As the timer nears zero
    ///                     so shrinks the possible number of balloons to shoot. Players have exactly three shots
    ///                     and a cooldown after each shot. So the players have to decide if they want to waste shots early
    ///                     for a possible win or save them for when theres only about 5 balloons left.
    ///                     
    ///                     remaining time and shots used could both count toward an overall score for the player.
    /// </summary>
    public class BalloonShooterGame : Game
    {

        public GameObject gunPrefab;
        public GameObject balloonPrefab;
        public int countdownTime = 5;
        public int balloonCount = 10;
        public Vector2 spawnExtents = new Vector2(1, 1);

        public Text gameStatusText;
        public Renderer goalIndicator;
        
        private float _startTime;
        private GameObject[] _balloons;
        private Color _goalColor;

        private BalloonShooterPlayerData GetConcretePlayerData(int index)
        {
            return (BalloonShooterPlayerData)_playerData[index];
        }

        protected override GamePlayerData CreatePlayerDataImpl(GamePlayer player)
        {
            return new BalloonShooterPlayerData();
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            Debug.Log("BalloonShooterGame: OnInitialize");

            // initialize our custom player data
            for (int i = 0; i < _playerData.Count; i++)
            {
                var pd = GetConcretePlayerData(i);

                // unequip all of the items the player is using
                pd.player.UnequipAll();

                // todo: disable item pickups for the player
                
                if (pd.gun == null)
                {
                    pd.gun = Instantiate(gunPrefab).GetComponent<PrototypeGun>();
                    NetworkServer.Spawn(pd.gun.gameObject);
                }

                pd.gun.isVisible = true;
                pd.player.Equip(pd.gun, true);
            }

            RpcSetStatusText("Get ready!");
            HandleSpawning();
        }

        protected override void OnStop()
        {
            base.OnStop();
            Debug.Log("BalloonShooterGame: Stopping");
            StopAllCoroutines();

            for (int i = 0; i < _playerData.Count; i++)
            {
                var pd = GetConcretePlayerData(i);
                pd.player.UnequipAll();

                // hide objects
                pd.gun.isVisible = false;


                foreach (var balloon in _balloons)
                {
                    Debug.Log("Removing balloons");
                    if (balloon != null)
                        NetworkServer.Destroy(balloon);
                }
            }
        }
        

        private void HandleSpawning()
        {
            int goal = Random.Range(0, balloonCount -1);
            float colorHueSteps = 1.0f / (float)balloonCount;

            float minHeight = 0.5f;
            float maxHeight = 2.0f;
            float balloonRadius = 0.35f; // m

            // very primitive way of choosing spawn point candidates. 
            // doesn't even work all of the time...
            List<Vector3> spawnPoints = new List<Vector3>();
            int spawnpointHorizontalSearchIterations = 5; // amount of tries to use before opting for a 

            _balloons = new GameObject[balloonCount];

            Vector3 spawnPoint = transform.position;
            spawnPoint.y = minHeight;
            for(int i = 0; i < balloonCount; i++)
            {

                for (int j = 0; j < spawnpointHorizontalSearchIterations; j++)
                {
                    float x = Random.Range(-spawnExtents.x, spawnExtents.x);
                    float y = Random.Range(minHeight, maxHeight);
                    float z = Random.Range(-spawnExtents.y, spawnExtents.y);
                    Vector3 candidate = new Vector3(x, y, z) + transform.position;

                    bool spawnPointAccepted = true;
                    foreach(var point in spawnPoints)
                    {
                        if(Vector3.Distance(point, candidate) < balloonRadius)
                        {
                            spawnPointAccepted = false;
                            break;
                        }
                    }

                    if (spawnPointAccepted)
                    {
                        spawnPoints.Add(candidate);
                        spawnPoint = candidate;
                        break;
                    }
                }

                // todo: spawn the balloons using an animation
                var go = Instantiate(balloonPrefab);
                go.transform.position = spawnPoint;
                NetworkServer.Spawn(go);

                _balloons[i] = go;

                var balloon = go.GetComponent<BalloonItem>();
                var shootable = go.GetComponent<Shootable>();
                balloon.color = Color.HSVToRGB(colorHueSteps * i, 1, 1);
                
                // attach the balloon to the ground
                // todo: we might want to do this in the balloon itself rather than here. Because if we do it here we have to do it for all the clients as well
                AttachBalloon(go, spawnPoint);
                RpcAttachBalloon(go, spawnPoint);

                if(i == goal)
                {
                    shootable.OnHit.AddListener(GoalShot);
                    _goalColor = balloon.color;
                }
            }

            EnableInput(false);
            
            StartCoroutine(Countdown());
        }

        private void EnableInput(bool enable)
        {
            foreach (var pd in _playerData)
            {
                ((BalloonShooterPlayerData)pd).gun.inputEnabled = enable;
            }
        }

        private IEnumerator Countdown()
        {
            float timer = countdownTime;
            float colorCycleSpeed = 0.5f;
            float currentHue = 0.0f;

            while(timer >  0.0f)
            {
                RpcSetStatusText("Get ready: " + timer.ToString("F2"));
                timer -= Time.deltaTime;

                // cycle the indicator through all of the possible colors
                RpcSetGoalColor(Color.HSVToRGB(currentHue, 1, 1));
                currentHue += colorCycleSpeed * Time.deltaTime;
                while(currentHue > 1.0f)
                    currentHue -= 1.0f;

                yield return null;
            }

            // todo: reenable user input so they can shoot
            RpcSetStatusText("Fire!");
            RpcSetGoalColor(_goalColor);
            _startTime = Time.time;
            EnableInput(true);
        }
        


        [ClientRpc] void RpcAttachBalloon(GameObject go, Vector3 position)
        {
            if (isServer) return; // don't do this if we're a host client
            AttachBalloon(go, position);
        }

        void AttachBalloon(GameObject go, Vector3 position)
        {
            var spring = go.AddComponent<SpringJoint>();
            spring.autoConfigureConnectedAnchor = false;
            spring.connectedAnchor = new Vector3(position.x, transform.position.y, position.z);
            spring.maxDistance = position.y + 0.1f;
            spring.minDistance = position.y;
            spring.anchor = Vector3.zero;
        }

        [ClientRpc] void RpcSetStatusText(string text)
        {
            gameStatusText.text = text;
        }
        [ClientRpc] void RpcSetGoalColor(Color color)
        {
            goalIndicator.material.SetColor("_EmissionColor", color);
        }

        private void GoalShot(Vector3 position, GamePlayer shooter)
        {
            RpcSetStatusText(shooter.displayName + " WINS! (" + (Time.time -_startTime).ToString("F2") + "s)");
            
            foreach (var balloon in _balloons)
            {
                if (balloon != null)
                    NetworkServer.Destroy(balloon);
            }

            // auto restart the game after a certain time
            StartCoroutine(AutoRestartGame(4.0f));
            //Stop();            
        }

        IEnumerator AutoRestartGame(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);
            HandleSpawning();
        }
    }

}
