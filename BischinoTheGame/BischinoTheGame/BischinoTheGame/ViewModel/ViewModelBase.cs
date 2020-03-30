using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Rooms.Helpers;

namespace BischinoTheGame.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, bool forceNotify = false, [CallerMemberName] string propertyName = "")
        {
            if (!forceNotify && EqualityComparer<T>.Default.Equals(storage, value))
                return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        protected virtual void Notify([CallerMemberName] string propertyName = "") => OnPropertyChanged(propertyName);

        protected void SetText(ref string storage, string value, int maxLength, [CallerMemberName] string propertyName = "")
        {
            if (value.IsNullOrEmpty() || char.IsWhiteSpace(value[0]))
                SetProperty(ref storage, null, forceNotify: true, propertyName);
            else if (value.Length > maxLength)
                Notify(propertyName);
            else
                SetProperty(ref storage, value.FirstUpperCase(), true, propertyName);
        }

        protected void SetRange<T>(ref T? storage, T? value, T min, T max, [CallerMemberName] string propertyName = "") where T : struct, IComparable<T>
        {
            if (value is null || (min.CompareTo(value.Value) <= 0 && max.CompareTo(value.Value) >= 0))
                SetProperty(ref storage, value, true, propertyName);
            else 
                Notify(propertyName);
        }
    }
}
