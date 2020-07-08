using System.IO;
using System;
using UnityEngine;
using MoonSharp.Interpreter;

public class ActionScripted : Action
{
    private Script luaScript;
    private string scriptPath;

    public ActionScripted(string scriptPath, GameObject actor)
    {
        this.scriptPath = scriptPath;

        UserData.RegisterAssembly();
        UserData.RegisterType<Vector3>();
        UserData.RegisterType<Transform>();
        UserData.RegisterType<Color>();
        UserData.RegisterType<GameObject>();
        UserData.RegisterType<LuaAPI>();

        if (File.Exists(Application.dataPath + "/../" + scriptPath)) {
            luaScript = new Script();
            luaScript.Options.ScriptLoader =
                new MoonSharp.Interpreter.Loaders.FileSystemScriptLoader();

            ((MoonSharp.Interpreter.Loaders.ScriptLoaderBase)
             luaScript.Options.ScriptLoader).ModulePaths =
                new string[] { Application.dataPath + "/?",
                    Application.dataPath + "/?.lua" };
            luaScript.Options.DebugPrint = Debug.Log;

            LuaAPI.Register(luaScript);
            luaScript.Globals["actor"] = actor;
            luaScript.Globals["Vector3"] = typeof(Vector3);
            luaScript.Globals["destroy"] =
                (Action<UnityEngine.Object>)GameObject.Destroy;

            DynValue ret = luaScript.DoFile(scriptPath);
        }
    }

    public override (bool done, bool result) Execute()
    {
        try {
            DynValue ret = luaScript.Call(luaScript.Globals["execute"]);
            return (ret.Tuple[0].Boolean, ret.Tuple[1].Boolean);
        } catch (ScriptRuntimeException ex) {
            Debug.LogError("[Execute] Could not execute function: "
                    + ex.DecoratedMessage);
        } catch (SyntaxErrorException ex) {
            Debug.LogError("[Execute] Could not execute function: "
                    + ex.DecoratedMessage);
        }

        return (true, false);
    }

    public override bool CanBeExecuted()
    {
        if (luaScript.Globals["canBeExecuted"] == null) {
            return true;
        }

        try {
            return luaScript.Call(luaScript.Globals["canBeExecuted"]).Boolean;
        } catch (ScriptRuntimeException ex) {
            Debug.LogError("[CanBeExecuted] Could not execute function: "
                    + ex.DecoratedMessage);
        } catch (SyntaxErrorException ex) {
            Debug.LogError("[CanBeExecuted] Could not execute function: "
                    + ex.DecoratedMessage);
        }

        return false;
    }

    public override void Cancel()
    {
        luaScript.Call(luaScript.Globals["cancel"]);
    }

    public void SetGlobal(string key, DynValue val)
    {
        luaScript.Globals[key] = val;
    }
}
