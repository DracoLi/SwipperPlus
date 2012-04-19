using System;
using System.ComponentModel;

namespace SwipperPlus.Model
{
  public class Connection : INotifyPropertyChanged
  {
    public string Name { get; set; }

    private bool _isConnected;
    public bool IsConnected { 
      get { return _isConnected; } 
      set
      {
        _isConnected = value;
        RaisePropertyChanged("IsConnected");
      }
    }

    private bool _isEnabled;
    public bool IsEnabled {
      get { return _isEnabled; }
      set
      {
        _isEnabled = value;
        RaisePropertyChanged("IsEnabled");
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    private void RaisePropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    public Connection(string name)
    {
      Name = name;
    }
  }
}
