using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Minikit.StateMachine
{
    public abstract class MKStateMachine
    {
        public UnityEvent<MKSMState, MKSMState> OnStateChanged = new();
        public UnityEvent OnStarted = new();
        public UnityEvent OnStopped = new();

        public MKSMState state
        {
            get
            {
                return __state;
            }
            private set
            {
                __state?.OnExit();

                MKSMState oldState = __state;
                __state = value;
                OnStateChanged.Invoke(oldState, __state);

                __state?.OnEnter();
            }
        }
        private MKSMState __state;

        private MKSMState entryState;
        private List<MKSMState> states = new();
        private Dictionary<MKSMState, List<MKSMTransition>> stateTransitionsByState = new();
        private MonoBehaviour monoBehaviour;
        private Coroutine tickCoroutine;


        public MKStateMachine(MonoBehaviour _monoBehaviour)
        {
            monoBehaviour = _monoBehaviour;
        }


        /// <returns> The entry state for the state machine (from the list of states passed out) </returns>
        protected abstract MKSMState CreateStates(out List<MKSMState> _states, out List<MKSMTransition> _transitions);

        private void Setup()
        {
            List<MKSMTransition> transitions;
            entryState = CreateStates(out states, out transitions);

            foreach (MKSMTransition transition in transitions)
            {
                if (stateTransitionsByState.ContainsKey(transition.currentState))
                {
                    stateTransitionsByState[transition.currentState].Add(transition);
                }
                else
                {
                    stateTransitionsByState.Add(transition.currentState, new List<MKSMTransition>() { transition });
                }
            }
        }

        public void Start()
        {
            if (entryState == null)
            {
                Setup();
            }

            if (tickCoroutine == null)
            {
                OnStarted.Invoke();

                state = entryState;

                tickCoroutine = monoBehaviour.StartCoroutine(Tick());
            }
        }

        public void Stop()
        {
            if (tickCoroutine != null)
            {
                monoBehaviour.StopCoroutine(tickCoroutine);
                tickCoroutine = null;

                state = null;

                OnStopped.Invoke();
            }
        }

        public void Restart()
        {
            Stop();
            Start();
        }

        public virtual IEnumerator Tick()
        {
            while (state != null)
            {
                // Handle any transitions that want to take place
                foreach (MKSMTransition transition in stateTransitionsByState[state])
                {
                    if (transition.CanTransition())
                    {
                        state = transition.otherState;
                        break;
                    }
                }

                yield return null;
            }
        }
    }
} // Minikit.StateMachine namespace
