using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

public class ScriptedBehaviour : MonoBehaviour
{
    private Script luaScript;
    public string scriptPath;

    public Dictionary<string, DynValue> sharedContext;

    public void Awake()
    {
        Initialize();
    }

    public void Initialize()
    {
        sharedContext = new Dictionary<string, DynValue>();

        Script.GlobalOptions.RethrowExceptionNested = true;

        UserData.RegisterAssembly();
        UserData.RegisterType<Vector3>();
        UserData.RegisterType<Transform>();
        UserData.RegisterType<Color>();
        UserData.RegisterType<GameObject>();
        UserData.RegisterType<LuaAPI>();
        UserData.RegisterType<Entity>();
        UserData.RegisterType<Simulation>();
        UserData.RegisterType<Time>();
        UserData.RegisterType<Input>();
        //UserData.RegisterType<PhysicsAPI>();
        UserData.RegisterType<Physics>();
        UserData.RegisterType<KeyCode>();
        UserData.RegisterType<UnityEngine.Random>();
        UserData.RegisterType<Collider>();
        UserData.RegisterType<MoveAction>();
        UserData.RegisterType<ActionScripted>();
        UserData.RegisterType<ActionGetAsleep>();
        UserData.RegisterType<ActionSleep>();
        UserData.RegisterType<ActionExecuteAfterDelay>();
        UserData.RegisterType<Script>();
        UserData.RegisterType<SharedContextProxy>();

        if (File.Exists(Application.dataPath + "/../" + scriptPath)) {
            luaScript = new Script();
            luaScript.Options.ScriptLoader =
                new MoonSharp.Interpreter.Loaders.FileSystemScriptLoader();

            ((MoonSharp.Interpreter.Loaders.ScriptLoaderBase)
             luaScript.Options.ScriptLoader).ModulePaths =
                new string[] { Application.dataPath + "/../?",
                    Application.dataPath + "/?",
                    Application.dataPath + "/?.lua" };
            luaScript.Options.DebugPrint = Debug.Log;

            LuaAPI.Register(luaScript);
            luaScript.Globals["this"] = luaScript;
            luaScript.Globals["gameObject"] = gameObject;
            luaScript.Globals["sharedContext"] =
                new SharedContextProxy(sharedContext, luaScript);
            luaScript.Globals["Vector3"] = typeof(Vector3);
            luaScript.Globals["GO"] = typeof(GameObject);
            luaScript.Globals["destroy"] =
                (Action<UnityEngine.Object>)GameObject.Destroy;
            luaScript.Globals["Time"] = typeof(Time);
            luaScript.Globals["Input"] = typeof(Input);
            //luaScript.Globals["PhysicsAPI"] = typeof(PhysicsAPI);
            luaScript.Globals["Physics"] = typeof(Physics);
            luaScript.Globals["KeyCode"] = UserData.CreateStatic<KeyCode>();
            luaScript.Globals["Random"] = typeof(UnityEngine.Random);
            luaScript.Globals["MoveAction"] = typeof(MoveAction);
            luaScript.Globals["ActionScripted"] = typeof(ActionScripted);
            luaScript.Globals["ActionGetAsleep"] = typeof(ActionGetAsleep);
            luaScript.Globals["ActionSleep"] = typeof(ActionSleep);
            luaScript.Globals["ActionExecuteAfterDelay"] =
                typeof(ActionExecuteAfterDelay);

            Entity entity = gameObject.GetComponent<Entity>();
            if (entity != null) {
                luaScript.Globals["entity"] = entity;
            }

            Simulation simulation =
                GameObject.Find("Simulation").GetComponent<Simulation>();
            if (simulation != null) {
                luaScript.Globals["simulation"] = simulation;
            }

            try {
                luaScript.DoFile(scriptPath);
            } catch (ScriptRuntimeException ex) {
                Debug.LogError("[Initialize] Could not read script: "
                        + ex.DecoratedMessage);
            } catch (SyntaxErrorException ex) {
                Debug.LogError("[Initialize] Could not read script: "
                        + ex.DecoratedMessage);
            }
        } else {
            Debug.LogError("The specified script file <"+ scriptPath
                    + "> doesn't exist.");
        }
    }

    public void Start()
    {
        if (luaScript.Globals["start"] != null) {
            try {
                luaScript.Call(luaScript.Globals["start"]);
            } catch (ScriptRuntimeException ex) {
                Debug.LogError("[Start] Script runtime exception: "
                        + ex.DecoratedMessage);
            }
        }
    }

    public void Update()
    {
        if (luaScript.Globals["update"] != null) {
            try {
                luaScript.Call(luaScript.Globals["update"]);
            } catch (ScriptRuntimeException ex) {
                Debug.LogError("[Update] Script runtime exception: "
                        + ex.DecoratedMessage);
            }
        }
    }
}
