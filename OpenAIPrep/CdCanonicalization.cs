using System;
using System.Collections.Generic;

namespace DeepDiveTechnicals.OpenAIPrep
{
    /// <summary>
    /// that returns the normalized absolute path after applying newDir to currentDir.
    /// Rules:
    /// 
    /// currentDir is always an absolute path like "/", "/usr/bin", "/a/b/c".
    /// 
    /// newDir can be:
    /// 
    /// An absolute path(starts with /)
    /// 
    /// A relative path(no leading /)
    /// 
    /// You must handle:
    /// 
    /// . → current directory
    /// 
    /// .. → parent directory
    /// 
    /// Multiple slashes (e.g. "////a///b" should be treated as "/a/b").
    /// 
    /// If we go “above root” (e.g.Cd("/", "..")), let’s say we return null (or you can throw, but pick one and stick to it).
    /// 
    /// Output should:
    /// 
    /// Always be absolute.
    /// 
    /// Be normalized(no.or..left, no duplicate slashes, no trailing slash except for root).
    /// </summary>
    public class CdCanonicalization
    {
        private const char Root = '/';

        // example "C:/users", "C:/users/apeppas" -> "C:/users/apeppas"
        // example "C:/users", "C:/users///apeppas" -> "C:/users/apeppas"
        // example "C:/users", "..///source" -> "C:/source"
        public string Cd(string currentDir, string newDir)
        {
            if (string.IsNullOrEmpty(newDir))
            {
                return currentDir;
            }

            var components = new Stack<string>();
            foreach (var component in currentDir.Split('/', StringSplitOptions.RemoveEmptyEntries))
            {
                if (component == "..")
                {
                    components.TryPop(out _);
                    continue;
                }
                components.Push(component);
            }

            try
            {
                if (newDir.StartsWith(Root))
                {
                    if (newDir.Length>=2 && newDir[1] == Root)
                    {
                        throw new Exception("Invalid command"); // this is an invalid command /////////users
                    }
                    return newDir;
                }
           
                return CdInternally(components, newDir);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        private string CdInternally(Stack<string> currentDir, string newDir)
        {
            var dotCounter = 0;
            var currentComponent = string.Empty;
            foreach(var chara in newDir)
            {
                if (chara == '/')
                {
                    if (currentComponent.Length > 0)
                    {
                        currentDir.Push(currentComponent);
                        currentComponent = string.Empty; // reset
                    }

                    dotCounter = 0; // reset upon slicing
                    continue;
                }

                if (chara == '.')
                {
                    dotCounter++;
                    if (dotCounter == 2)
                    {
                        dotCounter = 0; //reset
                        if (!currentDir.TryPop(out _))
                        {
                            throw new Exception("You're out of limit");
                        }
                    }
                    continue;
                }

                currentComponent += chara;
            }

            if (currentComponent.Length > 0)
            {
                if (dotCounter==1)
                {
                    currentComponent = $".{currentComponent}"; // this handles hidden files like .config
                }

                currentDir.Push(currentComponent);
                dotCounter = 0; // reset upon flush
            }

            return ComposeDir(currentDir);
        }

        internal static string ComposeDir(Stack<string> finalDir)
        {
            var items = finalDir.Count;
            var output = new string[items];
            for (var i = items - 1; i >= 0; i--)
            {
                output[i] = finalDir.Pop();
            }

            var finalOutput = string.Join('/', output);
            if (finalOutput.Length > 0)
            {
                return $"{Root}{finalOutput}"; //prepend Root
            }
            else
            {
                return "/"; // you are at the Root.
            }
        }
    }

    public class CdCanonicalizationTests
    {
        private readonly List<Tuple<int, string, string, string>> Scenarios = new List<Tuple<int, string, string, string>>
        {
            new(0, "/users", "/users/apeppas", "/users/apeppas" ),
            new(2, "/users", "..///source", "/source"),
            new(3, "/", "./users/alex/desktop", "/users/alex/desktop"),
            new(4, "/users/alex/desktop", "../../../source", "/source"),
            new(5, "/users/alex/desktop", "..///../john", "/users/john"),
            new(6, "/", ".", "/"),
            new(7, "/a/b", "./c", "/a/b/c"),
            new(8, "/a/b", "", "/a/b"),
            new(9, "/", "//////", "Invalid command"),
            new(10, "/a/b", "../../..", "You're out of limit"),
            new(11, "/", "..", "You're out of limit"),
            new(12, "/users/apeppas", "../.config", "/users/.config" ),
            new(13, "/users/apeppas/../unknown", "../.config", "/users/.config" ),
        };

        public void Cd_VariousScenarios_Success()
        {
            var canonicalization = new CdCanonicalization();

            foreach (var (scenarioInex, currentDir, newDir, expectedDir) in Scenarios)
            {
                var actualDir = canonicalization.Cd(currentDir, newDir);
                if (expectedDir != actualDir)
                {
                    Console.WriteLine($"ScenarioIndex: {scenarioInex}, Warning: expected {expectedDir} but found {actualDir}.");
                }
            }
        }
    }
}
