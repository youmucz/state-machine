using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minikit.StateMachine
{
    public abstract class MKSMTransition
    {
        public MKSMState currentState { get; private set; }
        public MKSMState otherState { get; private set; }


        public MKSMTransition(MKSMState _currentState, MKSMState _otherState)
        {
            currentState = _currentState;
            otherState = _otherState;
        }


        public abstract bool CanTransition();
    }
} // Minikit.StateMachine namespace
