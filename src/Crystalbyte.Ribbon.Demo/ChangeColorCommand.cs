using System;
using System.Windows;
using System.Windows.Input;
using Crystalbyte.UI;

namespace Crystalbyte.Ribbon.Demo {
    public sealed class ChangeColorCommand : ICommand {
        private readonly ColorContext _context;

        public ChangeColorCommand(ColorContext context) {
            _context = context;
        }

        #region Implementation of ICommand

        public bool CanExecute(object parameter) {
            return true;
        }

        public void Execute(object parameter) {
            var window = Application.Current.MainWindow as RibbonWindow;
            if (window != null) {
                window.AccentBrush = _context.Brush;
            }
        }

        public event EventHandler CanExecuteChanged;

        public void OnCanExecuteChanged() {
            var handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion
    }
}
