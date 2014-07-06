using System;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    public class CommandProcessor
    {
        public static string[] SplitCommand(string fullText) {
            fullText = fullText.Trim(' ');
            if (fullText.Contains(" ")) {
                List<string> parsed = new List<string>();
                bool startNewLine = true;
                int currentLine = -1;
                bool isInQuotes = false;
                for (int i = 0; i < fullText.Length; i++) {
                    if (startNewLine) {
                        parsed.Add("");
                        currentLine++;
                        startNewLine = false;
                    }
                    char curChar = fullText[i];
                    if (curChar == ' ' && isInQuotes == false) {
                        startNewLine = true;
                    } else if (curChar == '"') {
                        isInQuotes = !isInQuotes;
                    } else {
                        parsed[currentLine] += curChar;
                    }
                }
                return parsed.ToArray();
            } else {
                return new string[] { fullText };
            }
        }

        public static string JoinArgs(string[] args) {
            string joinedArgs = "";
            for (int i = 1; i < args.Length; i++) {
                joinedArgs += args[i] + " ";
            }
            return joinedArgs;
        }

        /// <summary>
        /// Parses the command
        /// </summary>
        /// <returns>The parsed command arguments</returns>
        public static Command ParseCommand(string command) {
            string fullCommandLine = command.Trim();
            List<string> parsed = new List<string>();
            bool startNewLine = true;
            int currentLine = -1;
            bool isInQuotes = false;
            for (int i = 0; i < fullCommandLine.Length; i++) {
                if (startNewLine) {
                    parsed.Add("");
                    currentLine++;
                    startNewLine = false;
                }
                char curChar = fullCommandLine[i];
                if (curChar == ' ' && isInQuotes == false) {
                    startNewLine = true;
                } else if (curChar == '"') {
                    isInQuotes = !isInQuotes;
                } else {
                    parsed[currentLine] += curChar;
                }
            }
            return new Command(fullCommandLine, parsed);
        }
    }
}
