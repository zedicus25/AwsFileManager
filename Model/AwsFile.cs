using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Aws.Model
{
    public class AwsFile : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DateTime _modificationDate;

        public DateTime ModificationDate
        {
            get => _modificationDate;
            set 
            { 
                _modificationDate = value;
                OnPropertyChanged("ModificationDate");
            }
        }

        private float _fileSize;

        public float FileSize
        {
            get => _fileSize;
            set
            {
                _fileSize = value;
                OnPropertyChanged("FileSize");
            }
        }

        private string _fileName;

        public string FileName
        {
            get => _fileName;
            set
            {
                _fileName = value;
                OnPropertyChanged("FileName");
            }
        }


        public AwsFile(DateTime modificationDate, float size, string fileName)
        {
            FileName = fileName;
            ModificationDate = modificationDate;
            FileSize = size;
        }

        public void OnPropertyChanged([CallerMemberName] string prop = "") =>
           PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
    }
}
