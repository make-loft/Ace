﻿using System;
using System.Windows.Input;
using Aero.Evocators;

namespace Aero.Input
{
    public class Mediator : ICommand
    {
        private class WeakListener : IDisposable
        {
            private readonly ICommand _command;
            private readonly WeakReference _weakMediator;

            public WeakListener(ICommand command, Mediator mediator)
            {
                _command = command;
                _weakMediator = new WeakReference(mediator);
                _command.CanExecuteChanged += CommandOnCanExecuteChanged;
            }

            private void CommandOnCanExecuteChanged(object sender, EventArgs eventArgs)
            {
                var mediator = _weakMediator.Target as Mediator;
                if (mediator == null) _command.CanExecuteChanged -= CommandOnCanExecuteChanged;
                else mediator.RaiseCanExecuteChanged(sender, eventArgs);
            }

            public void Dispose()
            {
                _command.CanExecuteChanged -= CommandOnCanExecuteChanged;
            }
        }

        private ICommand _command;
        private bool _lastCanExecuteState;
        private WeakReference _weakSender;
        private WeakReference _weakEvocator;
        private WeakListener _weakListener;

        ~Mediator()
        {
            if (_weakListener != null) _weakListener.Dispose();
        }

        public Mediator()
        {
        }

        public Mediator(object sender, CommandEvocator evocator)
        {
            Initialize(sender, evocator);
        }

        public void Initialize(object sender, CommandEvocator evocator)
        {
            _weakSender = new WeakReference(sender);
            _weakEvocator = new WeakReference(evocator);
            _command = evocator.Command;
            _weakListener = new WeakListener(_command, this); //_command.CanExecuteChanged += RaiseCanExecuteChanged;

            var contextCommand = _command as Command;
            if (contextCommand != null) contextCommand.RaiseCanExecuteChanged();
        }

        public void SetSender(object sender)
        {
            _weakSender = new WeakReference(sender);
        }

        public bool CanExecute(object parameter)
        {
            if (_weakEvocator == null) return true;
            var sender = _weakSender.Target;
            var evocator = _weakEvocator.Target as CommandEvocator;
            if (evocator == null || _command == null) return false;

            var canExecuteEventArgs = new CanExecuteEventArgs(evocator.Command, parameter);
            evocator.EvokePreviewCanExecute(sender, canExecuteEventArgs);
            if (!canExecuteEventArgs.CanExecute && canExecuteEventArgs.Handled) return false;
            evocator.EvokeCanExecute(sender, canExecuteEventArgs);
            if (_lastCanExecuteState == canExecuteEventArgs.CanExecute)
                return canExecuteEventArgs.CanExecute;

            _lastCanExecuteState = canExecuteEventArgs.CanExecute;
            CanExecuteChanged(this, EventArgs.Empty);
            return canExecuteEventArgs.CanExecute;
        }

        public void Execute(object parameter)
        {
            if (_weakSender == null) return;
            var sender = _weakSender.Target;
            var evocator = _weakEvocator.Target as CommandEvocator;
            if (evocator == null || _command == null) return;

            evocator.EvokePreviewExecuted(sender, new ExecutedEventArgs(_command, parameter));
            evocator.EvokeExecuted(sender, new ExecutedEventArgs(_command, parameter));

            var contextCommand = _command as Command;
            if (contextCommand != null) contextCommand.RaiseCanExecuteChanged();
        }

        public void RaiseCanExecuteChanged(object o, EventArgs args)
        {
            CanExecuteChanged(o, args);
        }

        public event EventHandler CanExecuteChanged = (sender, args) => { };
    }
}