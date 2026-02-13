using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Dtimu.ViewModels
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public virtual void RaiseEvent([CallerMemberName] string prop = null)
        {
            PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(prop));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}
