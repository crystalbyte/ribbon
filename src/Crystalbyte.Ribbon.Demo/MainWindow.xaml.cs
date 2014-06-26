using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Crystalbyte.Ribbon.Demo {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow  {
        public MainWindow() {
            InitializeComponent();

            CommandBindings.Add(new CommandBinding(ApplicationCommands.Help, (sender, e) => MessageBox.Show("help")));
        }
    }
}
