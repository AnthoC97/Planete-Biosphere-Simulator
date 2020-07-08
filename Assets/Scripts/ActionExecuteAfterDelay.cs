using System.Linq;
using UnityEngine;
using MoonSharp.Interpreter;

public class ActionExecuteAfterDelay : Action
{
    private int framesElapsed = 0;
    private int delayFrames;
    private Script script;
    private string function;

    public ActionExecuteAfterDelay(int delayFrames, string function,
            Script script)
    {
        this.delayFrames = delayFrames;
        this.script = script;
        this.function = function;
    }

    public override (bool done, bool result) Execute()
    {
        framesElapsed += 1;

        if (framesElapsed > delayFrames) {
            try {
                var func = script.Globals[function];
                DynValue returnValue = script.Call(func);

                if (returnValue.IsNotNil()) {
                    return (true, returnValue.CastToBool());
                }

                return (true, true);
            } catch (ScriptRuntimeException ex) {
                Debug.LogError("[Execute] Could not execute function: "
                        + ex.DecoratedMessage);
            } catch (SyntaxErrorException ex) {
                Debug.LogError("[Execute] Could not execute function: "
                        + ex.DecoratedMessage);
            }

            return (true, false);
        }

        return (false, false);
    }

    public override bool CanBeExecuted()
    {
        return true;
    }

    public override void Cancel()
    {

    }
}
