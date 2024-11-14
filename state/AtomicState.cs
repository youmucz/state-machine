using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot.Collections;
using Minikit.StateMachine;

/// <summary>
/// This is a state that has no sub_states.
/// </summary>
[Tool, GlobalClass, Icon("res://addons/state-machine/assets/atomic_state.svg")]
public partial class AtomicState : State
{
    public AtomicState()
    {
    }
#if TOOLS
    public override string[] _GetConfigurationWarnings()
    {
        var warnings = base._GetConfigurationWarnings().ToList();
        
        var found = false;
        foreach (var child in GetChildren())
        {
            if (child is Transition)
            {
                found = true;
                break;
            }
        }
        
        if (!found) warnings.Add("The AtomicState must have exactly one Transition child.");

        return  warnings.ToArray();
    }
#endif

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
