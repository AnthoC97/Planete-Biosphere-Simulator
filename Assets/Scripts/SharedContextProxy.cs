using System.Collections.Generic;
using MoonSharp.Interpreter;

public class SharedContextProxy
{
    private Dictionary<string, DynValue> context;
    private Script script;

    public SharedContextProxy(Dictionary<string, DynValue> target,
            Script script)
    {
        context = target;
        this.script = script;
    }

    public DynValue GetEditable()
    {
        DynValue editableValue = DynValue.NewTable(script);

        foreach (var pair in context) {
            editableValue.Table[pair.Key] = pair.Value;
        }

        return editableValue;
    }

    public bool Synchronize(DynValue luaEditableContext)
    {
        if (luaEditableContext.Type != DataType.Table) {
            return false;
        }

        Table luaEditableContextTable = luaEditableContext.Table;

        foreach (var pair in luaEditableContextTable.Pairs) {
            context[pair.Key.String] = pair.Value;
        }

        return true;
    }
}
