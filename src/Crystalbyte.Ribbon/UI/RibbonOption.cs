#region Using directives

using System.Windows.Media;

#endregion

namespace Crystalbyte.UI {
    public sealed class RibbonOption : NotificationObject {

        #region Private Fields

        private bool _isSelected;

        #endregion

        public string Title { get; set; }

        public bool IsSelected {
            get { return _isSelected; }
            set {
                if (_isSelected == value) {
                    return;
                }

                RaisePropertyChanging(() => IsSelected);
                _isSelected = value;
                RaisePropertyChanged(() => IsSelected);
            }
        }

        public string Description { get; set; }

        public ImageSource ImageSource { get; set; }

        public RibbonState Visibility { get; set; }
    }
}