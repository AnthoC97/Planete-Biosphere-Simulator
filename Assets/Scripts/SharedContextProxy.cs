using System.Collections.Generic;
using MoonSharp.Interpreter;

public class SharedContextProxy
{
    private Dictionary<string, DynValue> context;

    public SharedContextProxy(Dictionary<string, DynValue> target)
    {
        context = target;
    }

    public DynValue this[string key]
    {
        get => context[key];
        set => context[key] = value;
    }
}
