using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minikit.StateMachine
{
    public abstract class MKSMState
    {
        private MKStateMachine stateMachine;


        public MKSMState(MKStateMachine _stateMachine)
        {
            stateMachine = _stateMachine;
        }


        public abstract string GetStateName();

        public abstract void OnEnter();

        public abstract void OnExit();
    }
} // Minikit.StateMachine namespace
