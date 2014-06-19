#region Using directives

using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

#endregion

namespace Crystalbyte {
    public abstract class NotificationObject : INotifyPropertyChanged, INotifyPropertyChanging {
        /// <summary>
        ///     Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName"> The property that has a new value. </param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "Method used to raise an event")]
        protected virtual void RaisePropertyChanged(string propertyName) {
            var handler = PropertyChanged;
            if (handler != null) {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        ///     Raises this object's PropertyChanged event for each of the properties.
        /// </summary>
        /// <param name="propertyNames"> The properties that have a new value. </param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "Method used to raise an event")]
        protected void RaisePropertyChanged(params string[] propertyNames) {
            if (propertyNames == null)
                throw new ArgumentNullException("propertyNames");

            foreach (var name in propertyNames) {
                RaisePropertyChanged(name);
            }
        }

        /// <summary>
        ///     Raises this object's PropertyChanged event.
        /// </summary>
        /// <typeparam name="T"> The type of the property that has a new value </typeparam>
        /// <param name="propertyExpression"> A Lambda expression representing the property that has a new value. </param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "Method used to raise an event")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Cannot change the signature")]
        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression) {
            var propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            RaisePropertyChanged(propertyName);
        }

        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        ///     Raises this object's PropertyChanging event.
        /// </summary>
        /// <param name="propertyName"> The property that has a new value. </param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "Method used to raise an event")]
        protected virtual void RaisePropertyChanging(string propertyName) {
            var handler = PropertyChanging;
            if (handler != null) {
                handler(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        /// <summary>
        ///     Raises this object's PropertyChanging event for each of the properties.
        /// </summary>
        /// <param name="propertyNames"> The properties that have a new value. </param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "Method used to raise an event")]
        protected void RaisePropertyChanging(params string[] propertyNames) {
            if (propertyNames == null)
                throw new ArgumentNullException("propertyNames");

            foreach (var name in propertyNames) {
                RaisePropertyChanging(name);
            }
        }

        /// <summary>
        ///     Raises this object's PropertyChanging event.
        /// </summary>
        /// <typeparam name="T"> The type of the property that has a new value </typeparam>
        /// <param name="propertyExpression"> A Lambda expression representing the property that has a new value. </param>
        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate",
            Justification = "Method used to raise an event")]
        [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",
            Justification = "Cannot change the signature")]
        protected void RaisePropertyChanging<T>(Expression<Func<T>> propertyExpression) {
            var propertyName = PropertySupport.ExtractPropertyName(propertyExpression);
            RaisePropertyChanging(propertyName);
        }
    }
}