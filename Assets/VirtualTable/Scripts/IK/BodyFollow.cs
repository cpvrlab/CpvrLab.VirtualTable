using UnityEngine;
using System.Collections;

namespace CpvrLab.AVRtar {

    /// <summary>
    /// This script has to be added to a biped character model to follow head rotation and movements
    /// by playing appropriate animations and updating body position.
    /// </summary>
    public class BodyFollow : MonoBehaviour {

        [Tooltip("Follow head position.")]
        public bool followPosition = true;

        [Tooltip("Follow head rotation.")]
        public bool followRotation = true;

        [Tooltip("Play an animation while rotating the body.")]
        public bool useRotationAnimation = true;

        [Tooltip("Play an animation while moving the body.")]
        public bool useLocomotionAnimation = true;

        [Tooltip("Deceleration when turning the body towards the head.")]
        public float turnDeceleration = 500.0f;

        [Tooltip("Acceleration when turning the body towards the head.")]
        public float turnAcceleration = 1000.0f;

        [Tooltip("Max speed when turning the body towards the head")]
        public float maxTurnSpeed = 200.0f;

        [Tooltip("Maximum angle the head can rotate relative to the body.")]
        [Range(0, 360)]
        public float headRotationLimit = 180.0f;


        [Tooltip("Maximum angle the head can rotates relative to the body.")]
        public float locomotionAnimThreshold = 0.8f;

        // todo: same inspector as fbbik with foldout for 
        public GameObject headGoal;
        public Vector3 headForward = Vector3.forward;
        public Vector3 headUp = Vector3.up;        
        
        private Animator _animator;

        private float _turnVelocity;
        private int _turnDirection;

        [Space]
        [Header("Advanced")]
        public float animatorLocomotionSpeed = 1.5f;

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        void FixedUpdate()
        {
            UpdateBodyRotation();

            UpdateBodyPosition();

            UpdateAnimatorParameters();
        }



        private void UpdateBodyRotation()
        {
            float angle = GetRelativeHeadAngle();
            float angleAbs = Mathf.Abs(angle);
            _turnDirection = angle > 0.0f ? 1 : -1;

            if(angleAbs < 5.0f) {
                // early out if the delta angle is too low
                _turnVelocity = 0.0f;
                return;
            }

            // keep the angle inside the headRotationLimit
            if(angleAbs > headRotationLimit * 0.5f) {
                float angleDelta = angleAbs - headRotationLimit * 0.5f;
                angleDelta *= _turnDirection;
                transform.Rotate(transform.up, angleDelta);
            }

            // calculate a new turn speed based on our goal angle
            UpdateTurnSpeed(angleAbs);

            Quaternion goalRotation = transform.localRotation * Quaternion.AngleAxis(angle, Vector3.up);
            transform.localRotation = Quaternion.RotateTowards(transform.localRotation, goalRotation, _turnVelocity * Time.deltaTime);
        }

        private void UpdateBodyPosition()
        {
            

            transform.position = new Vector3(headGoal.transform.position.x, transform.position.y, headGoal.transform.position.z);

        }

        private void UpdateAnimatorParameters()
        {
            VelocityInfo velocityInfo = headGoal.GetComponent<VelocityInfo>();

            // At the moment our animator has two states, one for turning and one for strafing
            // This might not be the ideal solution and a combination of a strafing locomotion
            // and turning locomotion animatorcontroller might look better. But for now we'll use this
            // That also means we need to switch between the turning and moving states and that's what we're doing here



            // locomotion
            Vector3 localVelocity = Quaternion.Inverse(transform.rotation) * velocityInfo.avrgVelocity;  //velocityInfo.averageVelocity;
            float velocityMag = velocityInfo.avrgVelocityMagnitude;

            // normalize velocity relative to the max speed of our animated character
            localVelocity /= animatorLocomotionSpeed;
            velocityMag /= animatorLocomotionSpeed;
            

            float strafe = Mathf.Min(localVelocity.x, 1.0f);
            float forward = Mathf.Min(localVelocity.z, 1.0f);

            _animator.SetFloat("Strafe", strafe);
            _animator.SetFloat("Forward", forward);

            _animator.SetFloat("Turn", _turnDirection * velocityInfo.avrgAngularVelocityMagnitude * 0.01f, 0.1f, Time.deltaTime);
            //_animator.SetFloat("Turn", 0.00003f);
            // turning



            if(velocityInfo.avrgVelocityMagnitude > locomotionAnimThreshold) {
                _animator.SetBool("DoTurning", false);
                _animator.speed = velocityMag;
            }
            else {
                _animator.SetBool("DoTurning", true);
                _animator.speed = 1.0f;
            }




            //Debug.Log("Velocity: " + velocityInfo.averageVelocity + "Local velocity: " + localVelocity);


        }

        private void UpdateTurnSpeed(float distance)
        {
            // TODO:    It would be cool to have a continous curve for acceleration and
            //          deceleration up to max speed. We use a trapezoid for now. 
            //      |
            //    v |     /-------------\
            //      |    /               \
            //      ------------------------- t
            

            // 1. current speed required to reach our destination with a constant deceleration
            float decVel = Mathf.Sqrt(2.0f * turnDeceleration * distance);

            // 2. current speed + acceleration
            float currentSpeed = Time.fixedDeltaTime * turnAcceleration + _turnVelocity;

            // 3. clamp the velocity at max speed and mix the two acceleration parts together
            _turnVelocity = Mathf.Min(decVel, currentSpeed, maxTurnSpeed);
        }

        // calculates relative rotation between head and torso forward vectors
        // given a head direction vector in world space
        public float GetRelativeHeadAngle()
        {
            Transform head = headGoal.transform;
            Vector3 avatarForward = Vector3.forward;
            Vector3 avatarUp = Vector3.up;

            // transform world headDir into local space and project it onto the forward-right plane
            Vector3 localHeadDir = transform.worldToLocalMatrix * (head.rotation * headForward);
            localHeadDir = Vector3.ProjectOnPlane(localHeadDir, avatarUp);
            localHeadDir.Normalize();

            // Check the sign of the dot product between head and torso normals to see if our head is upside down
            // (upside down happens if we lift our chin so high that we are looking backwards)
            // flip the direction of the head direction if it's upside down
            if(Vector3.Dot(head.rotation * headUp, transform.rotation * avatarUp) < 0.0f)
                localHeadDir *= -1.0f;

            // calculate the angle between the projected head direction and the forward vector
            float angle = Mathf.Acos(Vector3.Dot(localHeadDir, avatarForward));

            // calculate the cross product in local space
            Vector3 cross = Vector3.Cross(avatarForward, localHeadDir);

            // determine the direction of the rotation using the dot product
            float dotFactor = Vector3.Dot(cross, avatarUp);
            angle *= (dotFactor > 0.0f) ? 1.0f : -1.0f;

            return angle * Mathf.Rad2Deg;
        }
    }

}