
using UnityEngine;

[DefaultExecutionOrder(-1)]
public class InputManager : Singleton<InputManager>
{
    public Inputs input;

    protected override void Awake()
    {
        base.Awake();
        input = new Inputs();
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }
}
