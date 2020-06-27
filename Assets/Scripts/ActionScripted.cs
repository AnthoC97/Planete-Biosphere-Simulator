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

        if (File.Exists(Application.dataPath + "/../" + scriptPath)) {
            luaScript = new Script();
            luaScript.Options.ScriptLoader =
                new MoonSharp.Interpreter.Loaders.FileSystemScriptLoader();

            ((MoonSharp.Interpreter.Loaders.ScriptLoaderBase)
             luaScript.Options.ScriptLoader).ModulePaths =
                new string[] { Application.dataPath + "/?",
                    Application.dataPath + "/?.lua" };
            luaScript.Options.DebugPrint = Debug.Log;

            luaScript.Globals["actor"] = actor;
            luaScript.Globals["distance"] =
                (Func<Vector3, Vector3, float>)Vector3.Distance;
            luaScript.Globals["destroy"] =
                (Action<UnityEngine.Object>)GameObject.Destroy;

            DynValue ret = luaScript.DoFile(scriptPath);
        }
    }

    public override (bool done, bool result) Execute()
    {
        DynValue ret = luaScript.Call(luaScript.Globals["execute"]);
        return (ret.Tuple[0].Boolean, ret.Tuple[1].Boolean);

        //populationSize = (int)luaScript.Globals.Get("populationSize").Number;
    }

    public override bool CanBeExecuted()
    {
        return luaScript.Call(luaScript.Globals["canBeExecuted"]).Boolean;
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
