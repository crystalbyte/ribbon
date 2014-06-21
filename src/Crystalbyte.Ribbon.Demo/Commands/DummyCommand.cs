using System;
using System.Windows;
using System.Windows.Input;

namespace Crystalbyte.Ribbon.Demo.Commands {
    public sealed class DummyCommand : ICommand {

        #region Implementation of ICommand
        public bool CanExecute(object parameter) {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public void OnCanExecuteChanged() {
            var handler = CanExecuteChanged;
            if (handler != null) 
                handler(this, EventArgs.Empty);
        }

        public void Execute(object parameter) {
            MessageBox.Show("dummy");
        }

        #endregion
    }
}
