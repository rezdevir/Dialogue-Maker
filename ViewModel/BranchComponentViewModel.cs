using DialogMaker.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DialogMaker.ViewModel
{
    public class BranchComponentViewModel:INotifyPropertyChanged
    {
        public ICommand DeleteCommand {  get; set; }    
        public ICommand UpCommand { get; set; }
        public ICommand DownCommand { get; set; }

        public delegate void BranchCommandDelegate(string command,string branch_id);

        public BranchCommandDelegate OnBranchCommand;

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
            DeleteCommand = new RelayCommand((_)=> Delete());
            UpCommand = new RelayCommand((_)=> Up());
            DownCommand = new RelayCommand((_)=> Down());
        }

        private void Delete()
        {
            OnBranchCommand?.Invoke("delete", BranchComponentBindig.BranchID);
        }
        private void Up()
        {
            OnBranchCommand?.Invoke("Up", BranchComponentBindig.BranchID);
        }
        private void Down()
        {
            OnBranchCommand?.Invoke("Down", BranchComponentBindig.BranchID);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    }
}
