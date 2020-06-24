using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoonSharp.Interpreter;

public class testScripting : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string scriptCode = @"    
		-- defines a factorial function
		function fact (n)
			if (n == 0) then
				return 1
			else
				return n*fact(n - 1)
			end
		end
        ";

        Script script = new Script();
        script.DoString(scriptCode);
        DynValue res = script.Call(script.Globals["fact"], 4);
        Debug.Log(res.Number);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
