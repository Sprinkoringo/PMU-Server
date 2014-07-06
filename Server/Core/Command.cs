/*The MIT License (MIT)

Copyright (c) 2014 PMU Staff

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/


namespace Server
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class Command
    {
        #region Fields

        List<string> commandArgs = new List<string>();
        string fullCommand;

        #endregion Fields

        #region Constructors

        internal Command(string fullCommand, List<string> command)
        {
            this.fullCommand = fullCommand;
            commandArgs = command;
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// Gets the command line arguments for the program
        /// </summary>
        public List<string> CommandArgs
        {
            get { return commandArgs; }
        }

        /// <summary>
        /// Gets the full, unparsed command string
        /// </summary>
        public string FullCommand {
            get { return fullCommand; }
        }

        #endregion Properties

        #region Indexers

        public string this[int index]
        {
            get { return commandArgs[index]; }
        }

        #endregion Indexers

        #region Methods

        /// <summary>
        /// Checks if a certain argument is included in the command line
        /// </summary>
        /// <param name="argToFind">The argument to look for</param>
        /// <returns>True if the argument exists; False if it doesn't exist.</returns>
        public bool ContainsCommandArg(string argToFind)
        {
            return commandArgs.Contains(argToFind);
        }

        /// <summary>
        /// Retrives the index of a certain argument in the command line.
        /// </summary>
        /// <param name="argToFind"></param>
        /// <returns>The index of the argument if it was found; otherwise, returns -1</returns>
        public int FindCommandArg(string argToFind)
        {
            return commandArgs.IndexOf(argToFind);
        }

        #endregion Methods
    }
}