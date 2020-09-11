using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Helper
{
    public class PythonInstance
    {
        private ScriptEngine engine;
        private ScriptScope scope;
        private ScriptSource source;
        private CompiledCode compiled;
        private object pythonClass;
        public PythonInstance(string scriptPath, string className = "PyClass")
        {
            //creating engine and stuff
            engine = Python.CreateEngine();
            scope = engine.CreateScope();

            //loading and compiling code
            source = engine.CreateScriptSourceFromString(scriptPath);
            compiled = source.Compile();

            //now executing this code (the code should contain a class)
            compiled.Execute(scope);

            //now creating an object that could be used to access the stuff inside a python script
            //pythonClass = engine.Operations.Invoke(scope.GetVariable(className));
        }
        public static string RunFromCmd(string rCodeFilePath)
        {
            string file = rCodeFilePath;
            string result = string.Empty;

            try
            {

                var info = new ProcessStartInfo(@"D:\Python tutorial\python.exe");
                info.Arguments = rCodeFilePath;

                info.RedirectStandardInput = false;
                info.RedirectStandardOutput = true;
                info.UseShellExecute = false;
                info.CreateNoWindow = true;

                using (var proc = new Process())
                {
                    proc.StartInfo = info;
                    proc.Start();
                    proc.WaitForExit();
                    if (proc.ExitCode == 0)
                    {
                        result = proc.StandardOutput.ReadToEnd();
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception("R Script failed: " + result, ex);
            }
        }

        public void SetVariable(string variable, dynamic value)
        {
            scope.SetVariable(variable, value);
        }

        public dynamic GetVariable( string className)
        {
            dynamic variable = scope.GetVariable(className);

            return scope.GetVariable(variable);
        }

        public void CallMethod(string method, params dynamic[] arguments)
        {
            engine.Operations.InvokeMember(pythonClass, method, arguments);
        }

        public dynamic CallFunction(string method, params dynamic[] arguments)
        {
            return engine.Operations.InvokeMember(pythonClass, method, arguments);
        }

    }
}
