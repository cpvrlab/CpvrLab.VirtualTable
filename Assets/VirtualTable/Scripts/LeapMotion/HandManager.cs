using UnityEngine;
using System.Collections;
using System;
using RootMotion.FinalIK;
using Leap.Unity;

/**
    TODO: 
    1. Allow an artist to define a hand pose target for an untracked hand
        > IF the leapmotion loses tracking the hand will fade into the untracked 
          pose. For example this pose could rest on the table!
    2. put this script in a namespace
    3. rename this script maybe? not sure. HandManager could come from leap

    */
//namespace CpvrLab.AVRtar {
//    public class HandManager : MonoBehaviour {

//        private enum HandType {
//            LeftHand = 0,
//            RightHand = 1,
//            Count = 2
//        }

//        private enum FadeDir {
//            In = 1,
//            Out = 0
//        }

//        public enum LeapHandModels {
//            AvatarHands,
//            ImageHands,

//        }

//        // animation curve managing hand tracking fade in interpolation
//        public AnimationCurve handTrackingFadeInCurve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);

//        // If the leap motion tracking confidence rating becomes lower
//        // than this threshold then we smoothly interpolate the hands
//        // into a resting position
//        [Range(0, 1)]
//        public float confidenceThreshold = 0.12f;
        

//        public HandController avatarHandController;
//        public HandController imageHandController;
//        private HandController _activeController;

//        public bool useRelaxHandGoals = true;
//        public HandPoseLerp leftHandGoal = null;
//        public HandPoseLerp rightHandGoal = null;
//        public GameObject leftLeapHandGoal = null;
//        public GameObject rightLeapHandGoal = null;

//        // reference to a coroutine handle that helps us with transition animations
//        // TODO:    abstract this low level animation stuff with the avatar hand fade in/out.
//        //          One possibility would be to have a class that keeps these variables
//        //          per hand and is able to fade in and out on its own.
//        private IEnumerator[] _handFadeCoroutine = { null, null };
//        private bool[] _avatarHandActive = { false, false }; // weather or not weight is 1 or 0 or on its way there
//        private float[] _avatarHandWeight = { 0.0f, 0.0f };
//        private float[] _avatarHandStartWeight = { 0.0f, 0.0f };
//        private Leap.Frame[] _prevLeapFrame = { null, null };
//        private float[] _leapAutoFadeoutTimer = { 0.0f, 0.0f };
//        private float[] _leapConfidence = { 0.0f, 0.0f };



//        void Start()
//        {
//            // enable the avatar hands by default
//            avatarHandController.gameObject.SetActive(true);
//            imageHandController.gameObject.SetActive(false);

//            _activeController = avatarHandController;
//        }

//        void Update()
//        {
//            // switch the enabled leap model if necessary
//            //if (activeLeapHandModel != _leapHandModel)
//            //    EnableLeapHandModel(activeLeapHandModel);

//            // check confidence
//            CheckLeapHandConfidence();

//            // if the currently active hand model requires auto fade in then do that
//            // TODO: make this more generic and usable by other hand models
//            AutoFadeAvatarHands(HandType.LeftHand);
//            AutoFadeAvatarHands(HandType.RightHand);
//        }
//        // @todo rename this function
//        void CheckLeapHandConfidence()
//        {
//            if(_activeController == null)
//                return;

//            // hack: the custom hand controller used for our avatar hands
//            //       doesn't populate the graphics model list. So here we
//            //       use the left and right graphics model references directly to get the confidence rating
//            if(_activeController == avatarHandController) {
//                UpdateCachedConfidence(_activeController.leftGraphicsModel.GetLeapHand());
//                UpdateCachedConfidence(_activeController.rightGraphicsModel.GetLeapHand());
//            }
//            else {
//                foreach(HandModel hm in _activeController.GetAllGraphicsHands()) {
//                    UpdateCachedConfidence(hm.GetLeapHand());
//                }
//            }

//        }

//        void UpdateCachedConfidence(Leap.Hand leapHand)
//        {
//            if(leapHand == null)
//                return;

//            HandType hand = (leapHand.IsLeft) ? HandType.LeftHand : HandType.RightHand;
//            _leapConfidence[(int)hand] = 0.0f;


//            // @todo    expose this setting in the inspector
//            float waitTime = 0.5f;
//            // only set the confidence variable from the leap readings if we have 
//            // recent data for that hand available to us
//            // Note:    we do this because if the user quickly moves his hands out of
//            //          view of the leapmotion, that Leap.Hand object might still report
//            //          a confidence rating above our threshold. This is because the leap
//            //          only updates the Leap.Hand object if a hand is visible.
//            if(_prevLeapFrame[(int)hand] != null) {
//                // increment auto fade out timer for this hand
//                _leapAutoFadeoutTimer[(int)hand] += Time.deltaTime;
//                if(_prevLeapFrame[(int)hand].Id != leapHand.Frame.Id) {
//                    // reset the timer if we just got a new frame
//                    _leapAutoFadeoutTimer[(int)hand] = 0.0f;
//                }
//                // finally retrieve the confidence rating from the leap if our data is recent
//                if(_leapAutoFadeoutTimer[(int)hand] < waitTime)
//                    _leapConfidence[(int)hand] = leapHand.Confidence;
//            }

//            _prevLeapFrame[(int)hand] = leapHand.Frame;
//        }

//        public void EnableLeapHandModel(LeapHandModels handModel)
//        {
//            if(handModel == LeapHandModels.AvatarHands) {
//                imageHandController.gameObject.SetActive(false);
//                avatarHandController.gameObject.SetActive(true);
//                _activeController = avatarHandController;
//            }
//            else {
//                avatarHandController.gameObject.SetActive(false);
//                imageHandController.gameObject.SetActive(true);
//                _activeController = imageHandController;
//            }
//        }

//        public void EnableAllHands()
//        {
//            // it's working for now but it's not nice at all
//            imageHandController.gameObject.SetActive(true);
//            avatarHandController.gameObject.SetActive(true);
//            _activeController = avatarHandController;
//        }

//        void AutoFadeAvatarHands(HandType hand)
//        {
//            // fade out if below confidence threshold or avatar hands disabled
//            if(!avatarHandController.isActiveAndEnabled || _leapConfidence[(int)hand] < confidenceThreshold)
//                FadeAvatarHandInOut(hand, FadeDir.Out, 0.7f);
//            else
//                FadeAvatarHandInOut(hand, FadeDir.In, 0.3f);
//        }

//        void SetAvatarHandFade(HandType hand, float fade)
//        {
//            float fadeClamped = Mathf.Clamp(fade, 0.0f, 1.0f);
//            _avatarHandWeight[(int)hand] = fadeClamped;
            
//            // if we're using relax hand goals then we fade out by 
//            // changing the lerp alpha of our hand pose lerp references
//            // 0 = relaxed
//            if(useRelaxHandGoals) {
//                if(hand == HandType.LeftHand)
//                    leftHandGoal.alpha = fadeClamped;
//                else
//                    rightHandGoal.alpha = fadeClamped;

//                //Debug.Log("Fade value = " + fadeClamped);
//            }
//            // else we fade out by setting the IK weights for the hand
//            // @todo:   reimplement this
//            /*else {
//                IKEffector effector;
//                if(hand == HandType.LeftHand)
//                    effector = ik.solver.GetEffector(FullBodyBipedEffector.LeftHand);
//                else
//                    effector = ik.solver.GetEffector(FullBodyBipedEffector.RightHand);

//                effector.positionWeight = _avatarHandWeight[(int)hand];
//                effector.rotationWeight = _avatarHandWeight[(int)hand];
//            }*/
//        }

//        void FadeAvatarHandInOut(HandType hand, FadeDir direction, float duration = 0.3f, Action complete = null)
//        {
//            if(!_avatarHandActive[(int)hand] && direction == FadeDir.Out ||
//                _avatarHandActive[(int)hand] && direction == FadeDir.In) {
//                // make sure the complete callback gets called if there is no fade inout running
//                // then just call the complete callback, else there will be a call made to complete
//                if(complete != null) {
//                    if(_avatarHandWeight[(int)hand] == (float)direction)
//                        complete();
//                }
//                return;
//            }


//            _avatarHandActive[(int)hand] = direction == FadeDir.In;

//            // call coroutine
//            _avatarHandStartWeight[(int)hand] = _avatarHandWeight[(int)hand];
//            CallTimedHelper(ref _handFadeCoroutine[(int)hand],
//                (float value) => {
//                    float weight = Mathf.Lerp(_avatarHandStartWeight[(int)hand], (float)direction, value);
//                    SetAvatarHandFade(hand, weight);
//                },
//                duration, handTrackingFadeInCurve, complete);
//        }

//        // stops any running coroutine for a given handle and starts a new one with the given parameters
//        void CallTimedHelper(ref IEnumerator handle, Action<float> callback, float duration = 0.5f, AnimationCurve easing = null, Action complete = null)
//        {

//            // stop the coroutine if one is already running
//            if(handle != null) {
//                StopCoroutine(handle);
//            }

//            // define the new coroutine
//            handle = CallTimedCoroutine(callback, duration, easing, complete);

//            // start the coroutine
//            StartCoroutine(handle);
//        }

//        // coroutine that calls a set action callback every update with a normalized
//        // time parameter. An easing curve can be provided to controll parameters value over time.
//        // This animation curve must be in a normalized range [0, 1]
//        IEnumerator CallTimedCoroutine(Action<float> callback, float duration = 0.5f, AnimationCurve easing = null, Action complete = null)
//        {
//            float timer = 0.0f;
//            float timeFactor = 1.0f / duration;
//            float value = 0.0f;
//            float normalizedTime = 0.0f;

//            while(timer < duration) {
//                timer += Time.deltaTime;
//                normalizedTime = timer * timeFactor;
//                if(normalizedTime > 1.0f)
//                    normalizedTime = 1.0f;

//                // calculate the eased value
//                value = normalizedTime;
//                if(easing != null)
//                    value = easing.Evaluate(normalizedTime);

//                //Debug.Log("Timed coroutine: " + value);

//                // invoke the callback
//                callback(value);

//                yield return null;
//            }

//            // call complete callback if it exists
//            if(complete != null)
//                complete();
//        }
//    }

//}

/*
TODO: can we put the animation helper stuff into its own class if that class is a member of a
        monobehaviour? can we use coroutines then?

    */
