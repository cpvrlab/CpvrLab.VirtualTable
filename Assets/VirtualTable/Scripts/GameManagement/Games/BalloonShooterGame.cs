using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace CpvrLab.VirtualTable
{

    class BalloonShooterPlayerData : GamePlayerData
    {
        public PrototypeGun gun;
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
    ///                     so shrinks the possible options of balloons to shoot. Players have exactly three shots
    ///                     and a cooldown after each shot used. So the players have to decide if they want to waste shots early
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
        
        enum GameState
        {
            Spawning,
            Countdown,
            Shooting
        };

        private float _timer;
        private GameState _state;
        private GameObject[] _balloons;

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

                // equip a light painter to the players main slot
                if (pd.gun == null)
                {
                    pd.gun = Instantiate(gunPrefab).GetComponent<PrototypeGun>();
                    NetworkServer.Spawn(pd.gun.gameObject);
                }

                pd.gun.isVisible = true;
                pd.player.Equip(pd.gun, true);
            }

            _state = GameState.Spawning;
        }

        protected override void OnStop()
        {
            base.OnStop();
            Debug.Log("BalloonShooterGame: Stopping");


            for (int i = 0; i < _playerData.Count; i++)
            {
                var pd = GetConcretePlayerData(i);
                pd.player.UnequipAll();

                // hide objects
                pd.gun.isVisible = false;

                //NetworkServer.Destroy(pd.gun.gameObject);
                //Destroy(pd.gun.gameObject);
                //pd.gun = null;           
            }
        }

        public override void OnUpdate()
        {
            if (!isServer) return;
            base.OnUpdate();

            switch (_state)
            {
                case GameState.Spawning: SpawningState(); break;
                case GameState.Countdown: CountdownState(); break;
                case GameState.Shooting: ShootingState(); break;
            }
        }

        private void SpawningState()
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
                    Vector3 candidate = new Vector3(x, y, z);

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
                        Debug.Log("Found spawn point for " + i + " after " + j + " iterations");
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

                Debug.Log("go " + go);

                // attach the balloon to the ground
                // todo: we might want to do this in the balloon itself rather than here. Because if we do it here we have to do it for all the clients as well
                AttachBalloon(go, spawnPoint);
                RpcAttachBalloon(go, spawnPoint);

                if(i == goal)
                {
                    shootable.OnHit.AddListener(GoalShot);
                }
            }

            _state = GameState.Countdown;
        }

        private void CountdownState() { }
        private void ShootingState() { }


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

        private void GoalShot(Vector3 position, GamePlayer shooter)
        {
            Debug.Log("Winner Winner Chicken Dinner, player " + shooter.displayName + "won the game");

            foreach(var balloon in _balloons)
            {
                if (balloon != null)
                    NetworkServer.Destroy(balloon);
            }

            Stop();            
        }
    }

}
