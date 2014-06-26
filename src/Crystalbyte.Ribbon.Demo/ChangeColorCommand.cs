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
            if (window == null) {
                return;
            }

            var type = parameter as string;
            if (string.IsNullOrEmpty(type)) {
                return;
            }

            if (type == "accent") {
                window.AccentBrush = _context.Brush;
                return;
            }

            if (type == "hover") {
                window.HoverBrush = _context.Brush;
                return;
            }

            if (type == "foreground") {
                window.Foreground = _context.Brush;
            }

            if (type == "background") {
                window.Background = _context.Brush;
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
