using System.Collections.Generic;
using Godot;
using Godot.Collections;

using Minikit.StateMachine;


[Tool, GlobalClass, Icon("res://addons/state-machine/assets/atomic_state.svg")]
public partial class State : NodeStateMachine
{
    public bool Active { get; set; }
    public Array<State> States { get; private set; } = new();
    public Array<Transition> Transitions { get; set; } = new ();
    
    private StateMachine _stateMachine;
    
    protected State()
    {
        
    }

    public override void _Ready()
    {
        base._Ready();
        
        if (Engine.IsEditorHint()) return;
        
        GD.Print($"State {GetStateName()} Ready.");
        
        foreach (var child in GetChildren())
        {
            switch (child)
            {
                case Transition transition:
                    Transitions.Add(transition);
                    break;
                case State state:
                    States.Add(state);
                    break;
            }
        }
    }
#if TOOLS
    public override string[] _GetConfigurationWarnings()
    {
        var warnings = new List<string>();
        
        var parent = GetParent();
        var found = false;
        while (IsInstanceValid(parent))
        {
            if (parent is StateMachine)
            {
                found = true;
                break;
            }
            
            parent = parent.GetParent();
        }
        
        if (!found) warnings.Add($"The parent '{parent}' is not a state machine.");
        
        return warnings.ToArray();
    }
#endif
    
    public virtual void Setup(StateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public virtual string GetStateName() {return GetType().Name;}
    
    /// <summary>
    /// // Handle any transitions that want to take place
    /// </summary>
    /// <returns></returns>
    public virtual State ProcessTransition() 
    {         
        if (!Active) return null;
        
        foreach (var transition in Transitions)
        {
            if (transition.CanTransition())
            {
                return transition.TargetState;
            }
        }

        return null;
    }

    public virtual void OnEnter() { Active = true; }

    public virtual void OnExit() { Active = false; }
}
