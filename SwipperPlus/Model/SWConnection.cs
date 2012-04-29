using System;
using System.ComponentModel;

namespace SwipperPlus.Model
{
  public class SWConnection : INotifyPropertyChanged
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

    public event PropertyChangedEventHandler PropertyChanged;
    private void RaisePropertyChanged(string propertyName)
    {
      if (this.PropertyChanged != null)
      {
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    public SWConnection(string name)
    {
      Name = name;
    }
  }
}
