using System;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;

namespace SwipperPlus.Utils
{
  public static class StorageUtils
  {
    #region Helpers for saving primitives
    
    /// <summary>
    /// Check if something exists
    /// </summary>
    internal static bool HasKeyValue(string key)
    {
      return IsolatedStorageSettings.ApplicationSettings.Contains(key);
    }

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
    /// Save something to the local storage identified by key
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

    /// <summary>
    /// Remove something from the local storage
    /// </summary>
    internal static void RemoveKeyValue(string key)
    {
      if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
      {
        IsolatedStorageSettings.ApplicationSettings.Remove(key);
        IsolatedStorageSettings.ApplicationSettings.Save();
      }
    }

    #endregion

    #region Helpers for saving serializable classes

    // This is the folder all our serialized data will be saved into
    private const string TargetFolder = "SwipperData";

    /// <summary>
    /// Save serializable data to isolated storage
    /// </summary>
    /// <typeparam name="T">Serializable object</typeparam>
    /// <param name="data">Data to be saved</param>
    /// <param name="key">Key to identify data</param>
    internal static void SaveData<T>(T data, string key)
    {
      // Get the isolated storage
      IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
      string targetFileName = String.Format("{0}/{1}.dat", TargetFolder, key);

      // Create data dir if not exists
      if (!isoFile.DirectoryExists(TargetFolder))
        isoFile.CreateDirectory(TargetFolder);

      // Try to save our data
      try
      {
        using (var targetFile = isoFile.CreateFile(targetFileName))
        {
          DataContractSerializer serializer = new DataContractSerializer(typeof(T));
          serializer.WriteObject(targetFile, data);
        }
      }
      catch (Exception e)
      {
        isoFile.DeleteFile(targetFileName);
        System.Diagnostics.Debug.WriteLine(e.Message);
      }
    }

    /// <summary>
    /// Get saved data from isolated storage
    /// </summary>
    /// <typeparam name="T">A serializable object</typeparam>
    internal static T GetData<T>(string key)
    {
      // Return default value by default
      T retVal = default(T);

      // Get file from isolated storage
      IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
      string targetFileName = String.Format("{0}/{1}.dat", TargetFolder, key);
      if (isoFile.FileExists(targetFileName))
      {
        // Open file and read data
        using (var sourceStream = isoFile.OpenFile(targetFileName, System.IO.FileMode.Open))
        {
          DataContractSerializer serializer = new DataContractSerializer(typeof(T));
          retVal = (T)serializer.ReadObject(sourceStream);
        }
      }
      return retVal;
    }

    /// <summary>
    /// Remove a data file from isolated storage if exists
    /// </summary>
    internal static void RemoveData(string key)
    {
      // Delete the file from our isolated storage if exists
      IsolatedStorageFile isoFile = IsolatedStorageFile.GetUserStoreForApplication();
      string targetFileName = String.Format("{0}/{1}.dat", TargetFolder, key);
      if (isoFile.FileExists(targetFileName))
      {
        isoFile.DeleteFile(targetFileName);
      }
    }

    #endregion
  }
}
