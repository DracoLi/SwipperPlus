using System;
using System.IO.IsolatedStorage;

namespace SwipperPlus.Utils
{
  public static class Storage
  {
    /// <summary>
    /// Get Something from localstorage
    /// </summary>
    internal static T GetKeyValue<T>(string key)
    {
      if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
        return (T)IsolatedStorageSettings.ApplicationSettings[key];
      else
        return default(T);
    }

    /// <summary>
    /// Save something to the local storange identified by key
    /// </summary>
    internal static void SetKeyValue<T>(string key, T value)
    {
      if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
      {
        if (value == null)
          IsolatedStorageSettings.ApplicationSettings.Remove(key);
        else
          IsolatedStorageSettings.ApplicationSettings[key] = value;
      }
      else
        if (value != null)
          IsolatedStorageSettings.ApplicationSettings.Add(key, value);
      IsolatedStorageSettings.ApplicationSettings.Save();
    }
  }
}
