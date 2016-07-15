using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CpvrLab.AVRtar {

    public class VelocityInfo : MonoBehaviour {

        private int averageSampleCount = 20;

        public Vector3 avrgVelocity { get { return _avrgVelocity; } }
        public Vector3 avrgAngularVelocity { get { return _averageAngularVelocity; } }
        public float avrgVelocityMagnitude { get { return _avrgVelocityMag; } }
        public float avrgAngularVelocityMagnitude { get { return _avrgAngularVelocityMag; } }

        Vector3 _prevPosition;
        Quaternion _prevRotation;

        Vector3 _velocity;
        Vector3 _velocitySum;
        Vector3 _avrgVelocity;
        float _avrgVelocityMag;
        double _velocityMagSum;
                
        Vector3 _angularVelocity;
        Vector3 _angularVelocitySum;
        Vector3 _averageAngularVelocity;
        float _avrgAngularVelocityMag;
        double _angularVelocityMagSum;

        Queue<Vector3> _velocityCache = new Queue<Vector3>();
        Queue<Vector3> _angularVelocityCache = new Queue<Vector3>();
        Queue<float> _velocityMagCache = new Queue<float>();
        Queue<float> _angularVelocityMagCache = new Queue<float>();

        int _iterations = 0;
        int _cacheIndex = 0;


        void Awake()
        {
            UpdatePrevState();
        }

        void LateUpdate()
        {
            _velocity = (transform.position - _prevPosition) / Time.deltaTime;
            _angularVelocity = (transform.rotation.eulerAngles - _prevRotation.eulerAngles) / Time.deltaTime;


            //Debug.Log("Vel: " + _velocity + " Avrg. vel: " + avrgVelocity + " Avrg. vel. mag.: " + avrgVelocityMagnitude + " Avrg. angular vel.: " + avrgAngularVelocity + " Avrg. angular vel. mag.: " + avrgAngularVelocityMagnitude);


            UpdatePrevState();
        }

        void UpdatePrevState()
        {
            // update previous state
            _prevPosition = transform.position;
            _prevRotation = transform.rotation;

            // update velocity rolling average
            _avrgVelocity = CalcRollingAvrgVec3(_velocity, ref _velocityCache, ref _velocitySum, averageSampleCount);

            // update velocity magnitude rolling average
            _avrgVelocityMag = CalcRollingAvrgf(_velocity.magnitude, ref _velocityMagCache, ref _velocityMagSum, averageSampleCount);
            _avrgVelocityMag = Mathf.Max(_avrgVelocityMag, 0.0f);
            // update angular velocity average
            _averageAngularVelocity = CalcRollingAvrgVec3(_angularVelocity, ref _angularVelocityCache, ref _angularVelocitySum, averageSampleCount);

            // update angular velocity magnitude rolling average
            _avrgAngularVelocityMag = CalcRollingAvrgf(_angularVelocity.magnitude, ref _angularVelocityMagCache, ref _angularVelocityMagSum, averageSampleCount);
            _avrgAngularVelocityMag = Mathf.Max(_avrgAngularVelocityMag, 0.0f);
        }


        // calculate running average over a set of samples given the sum of that set
        float CalcRollingAvrgf(float newSample, ref Queue<float> samples, ref double sum, int maxSamples = 20)
        {
            // dequeue the oldest sample and remove it from the sum
            if(samples.Count >= maxSamples) {
                sum -= samples.Dequeue();
            }

            // add new sample
            samples.Enqueue(newSample);
            sum += newSample;

            // calculate average and mean
            return (float)(sum / (double)samples.Count);
        }

        // calculate running average over a set of samples given the sum of that set
        Vector3 CalcRollingAvrgVec3(Vector3 newSample, ref Queue<Vector3> samples, ref Vector3 sum, int maxSamples = 20)
        {
            // dequeue the oldest sample and remove it from the sum
            if(samples.Count >= maxSamples) {
                sum -= samples.Dequeue();
            }

            // add new sample
            samples.Enqueue(newSample);
            sum += newSample;

            // calculate average and mean
            return sum / (float)samples.Count;
        }


            static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp;
            temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Handles.color = new Color(0.0f, 0.0f, 1.0f, 0.2f);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, _velocity.normalized);
            Handles.ArrowCap(0, transform.position, rotation, _velocity.magnitude * 0.2f);

            Handles.color = new Color(0.0f, 0.6f, 1.0f);
            rotation = Quaternion.FromToRotation(Vector3.forward, _avrgVelocity.normalized);
            Handles.ArrowCap(0, transform.position, rotation, _avrgVelocity.magnitude * 0.2f);
        }
#endif
    }
}