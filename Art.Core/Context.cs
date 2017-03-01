using System.Collections.Generic;
using System.Windows.Input;
using Aero.Input;

namespace Aero
{
    public static class Context
    {
        private static readonly Dictionary<string, Command> Container = new Dictionary<string, Command>();

        public static Command Get(string key)
        {
            if (!Container.ContainsKey(key))
                Container.Add(key, new Command(key));
            return Container[key];
        }

        public static void UpdateCanExecuteState()
        {
            foreach (var command in Container.Values)
            {
                command.RaiseCanExecuteChanged();
            }
        }

        public static Command Make { get { return Get("Make"); } }
        public static Command Login { get { return Get("Login"); } }
        public static Command Logout { get { return Get("Logout"); } }
        public static Command Refresh { get { return Get("Refresh"); } }
        public static Command Enter { get { return Get("Exit"); } }
        public static Command Exit { get { return Get("Exit"); } }
        public static Command New { get { return Get("New"); } }
        public static Command Open { get { return Get("Open"); } }
        public static Command Save { get { return Get("Save"); } }
        public static Command Dry { get { return Get("Dry"); } }
        public static Command Rate { get { return Get("Rate"); } }
        public static Command Buy { get { return Get("Buy"); } }
        public static Command Back { get { return Get("Back"); } }
        public static Command Next { get { return Get("Next"); } }
        public static Command Cut { get { return Get("Cut"); } }
        public static Command Copy { get { return Get("Copy"); } }
        public static Command Paste { get { return Get("Paste"); } }
        public static Command Delete { get { return Get("Delete"); } }
        public static Command Zoom { get { return Get("Zoom"); } }
        public static Command Move { get { return Get("Move"); } }
        public static Command Rotate { get { return Get("Rotate"); } }
        public static Command Navigate { get { return Get("Navigate"); } }

        public class Set
        {
            public static Command Add { get { return Get("Set.Add"); } }
            public static Command Remove { get { return Get("Set.Remove"); } }
            public static Command Clear { get { return Get("Set.Clear"); } }
        }

        public static Mediator GetMediator(this ContextObject contextObject, string commandKey, object sender)
        {
            return new Mediator(sender, contextObject[Get(commandKey)]);
        }

        public static Mediator GetMediator(this ContextObject contextObject, ICommand command, object sender)
        {
            return new Mediator(sender, contextObject[command]);
        }

        public static ContextSet<T> ToSet<T>(this IEnumerable<T> collection)
        {
            return new ContextSet<T>(collection);
        }
    }
}