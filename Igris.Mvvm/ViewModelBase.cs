using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Igris.Mvvm
{
    public abstract class ViewModelBase : INotifyPropertyChanged, IDisposable
    {
        private Dictionary<string, object> _propertyBag;
#if NETCOREAPP3_1_OR_GREATER
        private Dictionary<string, object> PropertyBag => _propertyBag ??= new Dictionary<string, object>();
#else
        private Dictionary<string, object> PropertyBag => _propertyBag ?? (_propertyBag = new Dictionary<string, object>());
#endif

        #region Events

        /// <summary>
        /// 
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        protected virtual void VerifyAccess()
        {
        }

        private static bool CompareValues<T>(T storage, T value)
        {
            return EqualityComparer<T>.Default.Equals(storage, value);
        }

        private static void GuardPropertyName(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }
        }

        [Conditional("DEBUG")]
        private void EnsureProperty([CallerMemberName] string propertyName = null)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                throw new ArgumentException("Property doesn't exist.", nameof(propertyName));
            }
        }

        #region PropertyName Methods

        internal static string GetPropertyName(LambdaExpression lambdaExpression)
        {
            MemberExpression memberExpression = lambdaExpression.Body is UnaryExpression
                ? (lambdaExpression.Body as UnaryExpression).Operand as MemberExpression
                : lambdaExpression.Body as MemberExpression;
            return memberExpression.Member.Name;
        }

        internal static string GetPropertyNameFast(LambdaExpression expression)
        {
#if NET5_0_OR_GREATER
            if (expression.Body is not MemberExpression memberExpression)
#else
            if (!(expression.Body is MemberExpression memberExpression))
#endif
            {
                throw new ArgumentException("MemberExpression is expected in expression.Body", nameof(expression));
            }
            const string vblocalPrefix = "$VB$Local_";
            MemberInfo member = memberExpression.Member;
            return member.MemberType == MemberTypes.Field
                && member.Name != null
                && member.Name.StartsWith(vblocalPrefix)
#if NETCOREAPP3_1_OR_GREATER
                ? member.Name[vblocalPrefix.Length..]
#else
                ? member.Name.Substring(vblocalPrefix.Length)
#endif
                : member.Name;
        }

        public static string GetPropertyName<T>(Expression<Func<T>> expression)
        {
            return GetPropertyNameFast(expression);
        }

        #endregion PropertyName Methods

        #region RaisePropertyChanged

        protected void RaisePropertyChanged(string propertyName)
        {
            EnsureProperty(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void RaisePropertyChanged()
        {
            RaisePropertiesChanged(null);
        }

        protected void RaisePropertiesChanged(params string[] propertyNames)
        {
            if (propertyNames == null || propertyNames.Length == 0)
            {
                RaisePropertyChanged(string.Empty);
                return;
            }
            foreach (string propertyName in propertyNames)
            {
                RaisePropertyChanged(propertyName);
            }
        }

        #endregion

        #region RaisePropertiesChanged

        protected void RaisePropertyChanged<T>(Expression<Func<T>> expression)
        {
            RaisePropertyChanged(GetPropertyName(expression));
        }

        protected void RaisePropertiesChanged<T1, T2>(Expression<Func<T1>> expression1, Expression<Func<T2>> expression2)
        {
            RaisePropertyChanged(expression1);
            RaisePropertyChanged(expression2);
        }

        protected void RaisePropertiesChanged<T1, T2, T3>(Expression<Func<T1>> expression1, Expression<Func<T2>> expression2, Expression<Func<T3>> expression3)
        {
            RaisePropertyChanged(expression1);
            RaisePropertyChanged(expression2);
            RaisePropertyChanged(expression3);
        }

        protected void RaisePropertiesChanged<T1, T2, T3, T4>(Expression<Func<T1>> expression1, Expression<Func<T2>> expression2, Expression<Func<T3>> expression3, Expression<Func<T4>> expression4)
        {
            RaisePropertyChanged(expression1);
            RaisePropertyChanged(expression2);
            RaisePropertyChanged(expression3);
            RaisePropertyChanged(expression4);
        }

        protected void RaisePropertiesChanged<T1, T2, T3, T4, T5>(Expression<Func<T1>> expression1, Expression<Func<T2>> expression2, Expression<Func<T3>> expression3, Expression<Func<T4>> expression4, Expression<Func<T5>> expression5)
        {
            RaisePropertyChanged(expression1);
            RaisePropertyChanged(expression2);
            RaisePropertyChanged(expression3);
            RaisePropertyChanged(expression4);
            RaisePropertyChanged(expression5);
        }

        #endregion RaisePropertiesChanged

        #region Property Methods

        protected T GetProperty<T>(Expression<Func<T>> expression)
        {
            return GetPropertyCore<T>(GetPropertyName(expression));
        }

        protected bool SetProperty<T>(ref T storage, T value, Expression<Func<T>> expression, Action changedCallback)
        {
            return SetProperty(ref storage, value, GetPropertyName(expression), changedCallback);
        }

        protected bool SetProperty<T>(ref T storage, T value, Expression<Func<T>> expression)
        {
            return SetProperty(ref storage, value, expression, null);
        }

        protected bool SetProperty<T>(Expression<Func<T>> expression, T value)
        {
            return SetProperty(expression, value, (Action)null);
        }

        protected bool SetProperty<T>(Expression<Func<T>> expression, T value, Action changedCallback)
        {
            string propertyName = GetPropertyName(expression);
            return SetPropertyCore(propertyName, value, changedCallback);
        }

        protected bool SetProperty<T>(Expression<Func<T>> expression, T value, Action<T> changedCallback)
        {
            string propertyName = GetPropertyName(expression);
            return SetPropertyCore(propertyName, value, changedCallback);
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, string propertyName, Action changedCallback)
        {
            VerifyAccess();
            if (CompareValues(storage, value))
            {
                return false;
            }

            storage = value;
            RaisePropertyChanged(propertyName);
            changedCallback?.Invoke();
            return true;
        }

        protected bool SetProperty<T>(ref T storage, T value, string propertyName)
        {
            return SetProperty(ref storage, value, propertyName, null);
        }

        #endregion Property Methods

        #region Value Methods

        protected T GetValue<T>([CallerMemberName] string propertyName = null)
        {
            GuardPropertyName(propertyName);
            return GetPropertyCore<T>(propertyName);
        }

        protected bool SetValue<T>(T value, [CallerMemberName] string propertyName = null)
        {
            return SetValue(value, default(Action), propertyName);
        }

        protected bool SetValue<T>(T value, Action changedCallback, [CallerMemberName] string propertyName = null)
        {
            return SetPropertyCore(propertyName, value, changedCallback);
        }

        protected bool SetValue<T>(T value, Action<T> changedCallback, [CallerMemberName] string propertyName = null)
        {
            return SetPropertyCore(propertyName, value, changedCallback);
        }

        protected bool SetValue<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            return SetValue(ref storage, value, default, propertyName);
        }

        protected bool SetValue<T>(ref T storage, T value, Action changedCallback, [CallerMemberName] string propertyName = null)
        {
            GuardPropertyName(propertyName);
            return SetProperty(ref storage, value, propertyName, changedCallback);
        }

        #endregion Value Methods

        #region PropertyCore Methods

        private T GetPropertyCore<T>(string propertyName)
        {
            return PropertyBag.TryGetValue(propertyName, out object val) ? (T)val : default;
        }

        private bool SetPropertyCore<T>(string propertyName, T value, Action changedCallback)
        {
            bool res = SetPropertyCore(propertyName, value, out T oldValue);
            if (res)
            {
                changedCallback?.Invoke();
            }
            return res;
        }

        private bool SetPropertyCore<T>(string propertyName, T value, Action<T> changedCallback)
        {
            bool res = SetPropertyCore(propertyName, value, out T oldValue);
            if (res)
            {
                changedCallback?.Invoke(oldValue);
            }
            return res;
        }

        protected virtual bool SetPropertyCore<T>(string propertyName, T value, out T oldValue)
        {
            VerifyAccess();
            oldValue = default;
            if (PropertyBag.TryGetValue(propertyName, out object val))
            {
                oldValue = (T)val;
            }

            if (CompareValues(oldValue, value))
            {
                return false;
            }

            lock (PropertyBag)
            {
                PropertyBag[propertyName] = value;
            }
            RaisePropertyChanged(propertyName);
            return true;
        }

        #endregion
    }
}
