using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aws.ViewModel
{
    public class MainVM : BaseVM
    {
        public BaseVM SelectedViewModel
        {
            get => _selectedVM;
            set
            {
                _selectedVM = value;
                OnPropertyChanged("SelectedViewModel");
            }
        }

        public RelayCommand GoToRekig
        {
            get
            {
                return _goToRekig ?? (_goToRekig = new RelayCommand(() =>
                {
                    if (_selectedVM is RekigPageVM)
                        return;
                    SetCurrentVM(_allVMs[0]);
                }));
            }
        }
        private RelayCommand _goToRekig;
        public RelayCommand GoToMain
        {
            get
            {
                return _goToMain ?? (_goToMain = new RelayCommand(() =>
                {
                    if (_selectedVM is MainPageVM)
                        return;
                    SetCurrentVM(_allVMs[1]);
                }));
            }
        }
        private RelayCommand _goToMain;
        private BaseVM _selectedVM;
        private List<BaseVM> _allVMs;

        public MainVM()
        {
            SelectedViewModel = new MainPageVM();
            _allVMs = new List<BaseVM>();
            CreateVMs();
        }

        private void CreateVMs()
        {
            _allVMs.Add(new RekigPageVM());
            _allVMs.Add(new MainPageVM());
        }

        private void SetCurrentVM(BaseVM vm)
        {
            if (vm == null)
                return;
            _selectedVM = vm;
            OnPropertyChanged("SelectedViewModel");
        }

    }
}
