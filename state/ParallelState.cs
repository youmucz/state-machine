using Godot;
using System;
using System.Linq;
using Minikit.StateMachine;

[Tool, GlobalClass, Icon("res://addons/state-machine/assets/parallel_state.svg")]
public partial class ParallelState : State
{
    public ParallelState()
    {
    }

#if TOOLS
    public override string[] _GetConfigurationWarnings()
    {
        var warnings = base._GetConfigurationWarnings().ToList();
        
        if (GetChildren().TakeWhile(child => child is not (ParallelState or CompoundState or AtomicState)).Any())
        {
            warnings.Add("The CompoundState must have exactly one child (ParallelState or CompoundState or Atomic State)");
        }
        
        return warnings.ToArray();
    }
#endif
    
    public override void OnEnter()
    {
        
    }

    public override void OnExit()
    {
        
    }
}
