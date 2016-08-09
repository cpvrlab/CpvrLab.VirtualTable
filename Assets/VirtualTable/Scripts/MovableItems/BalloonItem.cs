using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

namespace CpvrLab.VirtualTable
{

    [RequireComponent(typeof(Shootable))]
    public class BalloonItem : MovableItem
    {

        public Transform centerOfMass;
        public GameObject popEffect;
        public bool tempPopTest = false;
        public AudioClip popSound;
        public float boyancyForce = 0.3f;
        public bool autoDestroy = false;
        public float lifeTime = 10.0f;

        private Rigidbody _rigidbody;
        private Renderer _renderer;
        [SyncVar(hook = "SetColor")] public Color color = Color.black;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _renderer = GetComponent<Renderer>();
            if (centerOfMass != null)
                _rigidbody.centerOfMass = centerOfMass.localPosition;
            
            _renderer.material.color = color;


            if (autoDestroy)
                StartCoroutine(AutoDestroy());
        }
        
        public void SetColor(Color col)
        {
            color = col;
            _renderer.material.color = color;

            Debug.Log("SET COLOR " + col);
        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            
            var shootable = gameObject.AddComponent<Shootable>();
            shootable.OnHit.AddListener(OnHit);

        }

        void OnHit(Vector3 position, GamePlayer shooter)
        {
            Pop();
            RpcPop();
        }

        IEnumerator AutoDestroy()
        {
            yield return new WaitForSeconds(lifeTime);
            Pop();
        }

        void FixedUpdate()
        {
            _rigidbody.AddForce(Vector3.up * boyancyForce);

            if (tempPopTest)
                Pop();
        }

        [ClientRpc] void RpcPop()
        {
            Pop();
        }


        public void Pop()
        {
            var popParticle = Instantiate(popEffect, centerOfMass.position, centerOfMass.rotation) as GameObject;
            var popPS = popParticle.GetComponent<ParticleSystem>();
            var pr = popPS.GetComponent<Renderer>();
            pr.material.color = color;
            Destroy(popParticle, popPS.duration);

            var popAudio = popParticle.AddComponent<AudioSource>();
            popAudio.clip = popSound;
            popAudio.pitch = Random.Range(0.8f, 1.2f);

            popAudio.maxDistance = 25;
            popAudio.Play();

            Destroy(gameObject);
        }
    }

}
