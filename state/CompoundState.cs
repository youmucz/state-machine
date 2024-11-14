using Godot;
using System;
using System.Linq;
using Minikit.StateMachine;

[Tool, GlobalClass, Icon("res://addons/state-machine/assets/compound_state.svg")]
public partial class CompoundState : State
{
    [Export]
    private State InitState
    {
        set
        {
            _initState = value;
            UpdateConfigurationWarnings();
        }
        
        get => _initState;
    }
    private State _initState;
    
    private State _activeState = null;
    
    public CompoundState()
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
        
        if (InitState == null)
        {
            warnings.Add("The CompoundState must have exactly one init state (ParallelState or CompoundState)" );
        }
        
        return warnings.ToArray();
    }
#endif

    public override State ProcessTransition()
    {
        State state = null;
        
        if (IsInstanceValid(_activeState))
        {
            state = _activeState.ProcessTransition();
        }
        
        return state;
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        
        if (!IsInstanceValid(_activeState) && Active)
        {
            _activeState = InitState;
            _activeState.OnEnter();
        }
    }

    public override void OnExit()
    {
        base.OnExit();

        if (_activeState is not null)
        {
            _activeState.OnExit();
            _activeState = null;
        }
    }
}
