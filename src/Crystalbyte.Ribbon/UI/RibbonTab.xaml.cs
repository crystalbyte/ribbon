#region Using directives

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

#endregion

namespace Crystalbyte.UI {
    public class RibbonTab : TabItem {

        #region Construction

        static RibbonTab() {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(RibbonTab),
                new FrameworkPropertyMetadata(typeof(RibbonTab)));
        }

        #endregion

        #region Class Overrides

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
            base.OnMouseLeftButtonUp(e);

            // Unfortunately the TabControl does not allow for tab deselections without selecting a new one.
            // Since the deselect tab remains selected internally it won't be automatically reselected on mouse up.
            // We need to set it manually.
            IsSelected = true;
        }

        #endregion

        #region Dependency Properties

        public Brush ContextualBrush {
            get { return (Brush)GetValue(ContextualBrushProperty); }
            set { SetValue(ContextualBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContextualBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContextualBrushProperty =
            DependencyProperty.Register("ContextualBrush", typeof(Brush), typeof(RibbonTab), new PropertyMetadata(null));

        public string ContextualText {
            get { return (string)GetValue(ContextualTextProperty); }
            set { SetValue(ContextualTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ContextualText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ContextualTextProperty =
            DependencyProperty.Register("ContextualText", typeof(string), typeof(RibbonTab), new PropertyMetadata(null));


        public bool IsContextual {
            get { return (bool)GetValue(IsContextualProperty); }
            set { SetValue(IsContextualProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IsContextual.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IsContextualProperty =
            DependencyProperty.Register("IsContextual", typeof(bool), typeof(RibbonTab), new PropertyMetadata(false));

        #endregion
    }
}