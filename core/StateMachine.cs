using System;
using System.Collections;
using System.Collections.Generic;
using Godot;
using HCoroutines;

namespace Minikit.StateMachine
{
    [Tool, GlobalClass, Icon("res://addons/state-machine/assets/state_machine.svg")]
    public partial class StateMachine : NodeStateMachine
    {
        [Export] public bool AutoStart = false;
        
        public readonly Action<State, State> OnStateChanged = delegate { };
        public readonly Action OnStarted = delegate { };
        public readonly Action OnStopped = delegate { };

        public State State
        {
            get => _state;
            private set
            {
                _state?.OnExit();

                var oldState = _state;
                _state = value;
                OnStateChanged.Invoke(oldState, _state);

                _state?.OnEnter();
            }
        }
        private State _state;
        private State _entryState;
        private List<State> _states = new();
        
        private readonly Dictionary<State, List<Transition>> _stateTransitionsByState = new();
        private Coroutine _tickCoroutine;

        public override void _Ready()
        {
            if (Engine.IsEditorHint()) return;
            
            Setup();
            
            if (AutoStart) Start();
        }
#if TOOLS
        public override string[] _GetConfigurationWarnings()
        {
            if (GetChildCount() != 1)
            {
                return new[] { "Entered state-machine must have exactly one child (ParallelState or CompoundState)" };
            }

            foreach (var child in GetChildren())
            {
                if (child is ParallelState parallelState || child is CompoundState compoundState) break;
                
                return new[] { "Entered state-machine must have exactly one child (ParallelState or CompoundState)" };
            }
            
            return base._GetConfigurationWarnings();
        }
#endif
        
        /// <returns> The entry state for the state machine (from the list of states passed out) </returns>
        private void Setup()
        {
            foreach (var child in GetChildren())
            {
                _entryState = child switch
                {
                    ParallelState parallelState => parallelState,
                    CompoundState compoundState => compoundState,
                    State state => state,
                    _ => _entryState
                };
            }
            
            _entryState.Setup(this);
            
            // _entryState = CreateStates(out _states, out var transitions);
            //
            // foreach (var transition in transitions)
            // {
            //     if (_stateTransitionsByState.TryGetValue(transition.CurrentState, out var value))
            //     {
            //         value.Add(transition);
            //     }
            //     else
            //     {
            //         _stateTransitionsByState.Add(transition.CurrentState, new List<Transition>() { transition });
            //     }
            // }
        }

        public void Start()
        {
            GD.Print("Starting state-machine ...");
            
            if (_entryState == null)
            {
                Setup();
            }

            if (_tickCoroutine == null)
            {
                OnStarted.Invoke();

                State = _entryState;
                
                _tickCoroutine = CoroutineManager.Instance.StartCoroutine(Tick());
            }
        }

        public void Stop()
        {
            GD.Print("Stopping state-machine ...");
            
            if (_tickCoroutine != null)
            {
                CoroutineManager.Instance.StopCoroutine(_tickCoroutine);
                _tickCoroutine = null;

                State = null;

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
            while (State != null)
            {
                var state = State.ProcessTransition();
                
                if (state != null)
                {
                    State = state;
                }
                
                yield return null;
            }
        }

        public IEnumerable<State> GetStates()
        {
            return _states;
        }
    }
}
