using System.Collections.Generic;
using UnityEngine;

namespace Konfus.Utility.Design_Patterns
{
    /// <summary>
    /// Abstract class for commands.
    /// </summary>
    public abstract class Command
    {
        /// <summary>
        /// Method called to execute command.
        /// </summary>
        public abstract void Execute();
    }

    /// <summary>
    /// Command invoker. Collects command into buffer to execute them at once.
    /// </summary>
    public class CommandInvoker : MonoBehaviour
    {
        // Collected commands.
        private Queue<Command> commands = new Queue<Command>();

        /// <summary>
        /// Method used to add new command to the buffer.
        /// </summary>
        /// <param name="command">New command.</param>
        public void AddCommand(Command command)
        {
            commands.Enqueue(command);
        }

        /// <summary>
        /// Method used to execute all commands from the commands queue.
        /// </summary>
        public void ExecuteCommands()
        {
            foreach (var c in commands)
            {
                c.Execute();
            }

            commands.Clear();
        }
    }
}