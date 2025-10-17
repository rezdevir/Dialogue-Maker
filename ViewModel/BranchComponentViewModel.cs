using DialogMaker.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DialogMaker.ViewModel
{
    public class BranchComponentViewModel:INotifyPropertyChanged
    {
        private BranchComponentModel _BranchComponentBindig;
        public BranchComponentModel BranchComponentBindig
        {
            get => _BranchComponentBindig;
            set
            {
                _BranchComponentBindig= value;
                OnPropertyChanged();

            }
        
        }
            

        public BranchComponentViewModel()
        {

        }


        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
