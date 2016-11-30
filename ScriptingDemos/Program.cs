﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace ScriptingDemos
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Please provide an option: ");
                var opt = Console.ReadLine();

                switch (int.Parse(opt))
                {
                    case 1:
                        RunSimple();
                        break;
                    case 2:
                        RunWithHost();
                        break;
                    case 3:
                        RunRepl();
                        break;
                    default:
                        Console.WriteLine("Invalid option.");
                        break;
                }

                Console.WriteLine();
                Console.WriteLine(" ******************************* ");
                Console.WriteLine();
            }
        }


        private static void RunSimple()
        {
            var code = File.ReadAllText(GetScriptTestFile("test1.csx"));

            //note: we block here, because we are in Main method, normally we could await as scripting APIs are async
            var result = CSharpScript.EvaluateAsync<int>(code).Result;

            //result is now 5
            Console.WriteLine(result);
        }

        private static void RunRepl()
        {
            ScriptState<object> scriptState = null;
            while (true)
            {
                Console.Write("* ");
                var input = Console.ReadLine();
                scriptState = scriptState == null ?
                    CSharpScript.RunAsync(input, ScriptOptions.Default.AddImports("System")).Result :
                    scriptState.ContinueWithAsync(input).Result;
            }
        }

        private static void RunWithHost()
        {
            var code = File.ReadAllText(GetScriptTestFile("test2.csx"));

            //note: we block here, because we are in Main method, normally we could await as scripting APIs are async
            var result = CSharpScript.EvaluateAsync<int>(code, null, new ScriptHost { Number = 5 }).Result;
            //result is now 25
            Console.WriteLine(result);
        }

        private static string GetScriptTestFile(string filename)
        {
            return Path.Combine(Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath), "Fixtures", filename);
        }
    }
}
