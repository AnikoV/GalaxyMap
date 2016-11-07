using System;
using System.ComponentModel;

namespace GalaxyMap.Utils
{
    internal class MagicAttribute : Attribute { }

    [Magic]
    public abstract class PropertyChangedBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void RaisePropertyChanged(string propName)
        {
            var eventCopy = PropertyChanged;
            eventCopy?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
