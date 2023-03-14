using Aws.Model;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Aws.ViewModel
{
    public class MainPageVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private AwsController _awsController;

        private ObservableCollection<AwsFile> _files;

        public ObservableCollection<AwsFile> Files
        {
            get { return _files; }
            set 
            {
                _files = value;
                OnPropertyChanged("Files");
            }
        }

        private AwsFile _selectedFile;

        public AwsFile SelectedFile
        {
            get { return _selectedFile; }
            set 
            { 
                _selectedFile = value;
                OnPropertyChanged("SelectedFile");
            }
        }


        public RelayCommand AddFileCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new RelayCommand(() =>
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Multiselect = true;
  
                    if (openFileDialog.ShowDialog() == true)
                    {
                        foreach (var file in openFileDialog.FileNames)
                        {
                            if(!_awsController.UploadFile(Path.GetFileName(file), file))
                                MessageBox.Show($"File not upload! \nFile:{Path.GetFileName(file)}", "Operation result", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    MessageBox.Show($"All files was uploaded!", "Operation result", MessageBoxButton.OK, MessageBoxImage.Information);
                    Files = new ObservableCollection<AwsFile>(_awsController.ListBucketContents());
                   
                }));
            }
        }

        private RelayCommand _addCommand;
        public RelayCommand DeleteFileCommand
        {
            get
            {
                return _deleteCommand ?? (_deleteCommand = new RelayCommand(() =>
                {
                    if (_selectedFile == null)
                    {
                        MessageBox.Show($"Selected file was null!", "Operation result", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    if (_awsController.DeleteFile(_selectedFile.FileName))
                    {
                        MessageBox.Show($"File was deleted from bucket!", "Operation result", MessageBoxButton.OK, MessageBoxImage.Information);
                        Files = new ObservableCollection<AwsFile>(_awsController.ListBucketContents());
                    }
                }));
            }
        }

        private RelayCommand _deleteCommand;

        public RelayCommand DownloadFileCommand
        {
            get
            {
                return _downloadCommand ?? (_downloadCommand = new RelayCommand(() =>
                {
                    if (_selectedFile == null)
                    {
                        MessageBox.Show($"Selected file was null!", "Operation result", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                        

                    if (_awsController.DownloadFile(_selectedFile.FileName))
                    {
                        MessageBox.Show($"File was downlanded in folder My Documents", "Operation result", MessageBoxButton.OK, MessageBoxImage.Information);
                        SelectedFile = null;
                    }
                       
                }));
            }
        }

        private RelayCommand _downloadCommand;

        public MainPageVM()
        {
            _awsController = new AwsController();
            Files = new ObservableCollection<AwsFile>(_awsController.ListBucketContents());
            _selectedFile = null;
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
