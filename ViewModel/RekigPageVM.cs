using Aws.Model;
using GalaSoft.MvvmLight.Command;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Aws.ViewModel
{
    public class RekigPageVM : BaseVM
    {


        private AwsController _awsController;
        private string _lastFile;
        private BitmapImage _lastImage;
        private string _lines;
        private string _words;

        public BitmapImage LastImage
        {
            get { return _lastImage; }
            set
            {
                _lastImage = value;
                OnPropertyChanged("LastImage");
            }
        }

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

                if (SelectedFile != null)
                {
                    _awsController.DownloadFile(_selectedFile.FileName);
                    CreateBitMap($"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\{_selectedFile.FileName}");
                    if (_lastFile != "")
                    {
                        string filePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\{_lastFile}";
                        try
                        {
                            if (File.Exists(filePath))
                            {
                                File.SetAttributes(filePath, FileAttributes.Normal);
                                File.Delete(filePath);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                    _lastFile = SelectedFile.FileName;
                    Lines = _awsController.GetLinesFromImage(_lastFile);
                    Words = _awsController.GetWordsFromImage(_lastFile);
                }


            }
        }

        public string Lines
        {
            get { return _lines; }
            set
            {
                _lines = value;
                OnPropertyChanged("Lines");
            }
        }
        public string Words
        {
            get { return _words; }
            set
            {
                _words = value;
                OnPropertyChanged("Words");
            }
        }

        public RelayCommand AddFileCommand
        {
            get
            {
                return _addCommand ?? (_addCommand = new RelayCommand(() =>
                {

                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
                    string sep = string.Empty;
                    foreach (var c in codecs)
                    {
                        string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                        openFileDialog.Filter = String.Format("{0}{1}{2} ({3})|{3}", openFileDialog.Filter, sep, codecName, c.FilenameExtension);
                        sep = "|";
                    }

                    openFileDialog.DefaultExt = ".png";

                    if (openFileDialog.ShowDialog() == true)
                    {
                        if (openFileDialog.FileName == "")
                            return;
                        if (!_awsController.UploadFile(Path.GetFileName(openFileDialog.FileName), openFileDialog.FileName))
                            MessageBox.Show($"File not upload! \nFile:{Path.GetFileName(openFileDialog.FileName)}", "Operation result", MessageBoxButton.OK, MessageBoxImage.Warning);
                        Words = "";
                        Lines = "";
                        CreateBitMap(openFileDialog.FileName);
                        string fileName = Path.GetFileName(openFileDialog.FileName);
                        MessageBox.Show($"All file was uploaded!", "Operation result", MessageBoxButton.OK, MessageBoxImage.Information);
                        Lines = _awsController.GetLinesFromImage(fileName);
                        Words = _awsController.GetWordsFromImage(fileName);
                        Files = new ObservableCollection<AwsFile>(_awsController.ListBucketContents());
                    }
                    

                }));
            }
        }

        private RelayCommand _addCommand;

        public RekigPageVM()
        {
            _awsController = new AwsController();
            Files = new ObservableCollection<AwsFile>(_awsController.ListBucketContents());
            _lastFile = "";
        }

        private void CreateBitMap(string fileName)
        {
            LastImage = new BitmapImage();
            LastImage.BeginInit();
            LastImage.UriSource = new Uri(fileName);
            LastImage.CacheOption = BitmapCacheOption.OnLoad;
            LastImage.EndInit();
        }

        ~RekigPageVM()
        {
            if (_lastFile != "")
            {
                string filePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\{_lastFile}";
                try
                {
                    if (File.Exists(filePath))
                    {
                        File.SetAttributes(filePath, FileAttributes.Normal);
                        File.Delete(filePath);
                    }
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
