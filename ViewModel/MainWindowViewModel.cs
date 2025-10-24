using DialogMaker.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DialogMaker.ViewModel
{
    public class MainWindowViewModel:INotifyPropertyChanged
    {

        public static MainWindowViewModel Instance { get; private set; }    
        public ObservableCollection<BranchComponentViewModel> BranchesCollection { get; set; } = new ObservableCollection<BranchComponentViewModel>(); 
       
        public MainWindowViewModel()
        {
            if(Instance==null)
                Instance = this;
           
        }

        public void DeleteFromBranches(string branchId)
        {
            int index = -1;
            int iterator = 0;
            foreach (var branch in BranchesCollection)
            {
               
                if (branch.BranchComponentBindig.BranchID == branchId)
                    index = iterator;
                iterator++;
            }
            BranchesCollection.RemoveAt(index); 

        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
