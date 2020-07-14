using System.IO;
using System;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

public class ScriptedBehaviour : MonoBehaviour
{
    private static Simulation simulation;

    private Script luaScript;
    public string scriptPath;

    public Dictionary<string, DynValue> sharedContext;

    public void Awake()
    {
        sharedContext = new Dictionary<string, DynValue>();

        if (simulation == null) {
            simulation =
                GameObject.Find("Simulation").GetComponent<Simulation>();
        }
    }

    public void Initialize()
    {
        Script.GlobalOptions.RethrowExceptionNested = true;

        Debug.Log("[Initialize] Application.dataPath: " + Application.dataPath);
        if (File.Exists(Application.dataPath + "/lua/" + scriptPath)) {
            luaScript = new Script();
            luaScript.Options.ScriptLoader =
                new MoonSharp.Interpreter.Loaders.FileSystemScriptLoader();

            ((MoonSharp.Interpreter.Loaders.ScriptLoaderBase)
             luaScript.Options.ScriptLoader).ModulePaths =
                new string[] { Application.dataPath + "/lua/?",
                    Application.dataPath + "/lua/?.lua"/*,
                    Application.dataPath + "/?",
                    Application.dataPath + "/?.lua"*/ };
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

            if (simulation != null) {
                luaScript.Globals["simulation"] = simulation;
            }

            try {
                luaScript.DoFile(Application.dataPath + "/lua/" + scriptPath);
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
        Initialize();

        var startFunction = luaScript.Globals["start"];

        if (startFunction != null) {
            try {
                luaScript.Call(startFunction);
            } catch (ScriptRuntimeException ex) {
                Debug.LogError("[Start] Script runtime exception: "
                        + ex.DecoratedMessage);
            }
        }

        // XXX Those rotations need to be done after initializing entity UI
        transform.LookAt(Vector3.zero);
        transform.Rotate(new Vector3(-90f, 0, 0));
    }

    public void Update()
    {
        var updateCallback = luaScript.Globals["update"];

        if (updateCallback != null) {
            try {
                luaScript.Call(updateCallback);
            } catch (ScriptRuntimeException ex) {
                Debug.LogError("[Update] Script runtime exception: "
                        + ex.DecoratedMessage);
            }
        }
    }
}
