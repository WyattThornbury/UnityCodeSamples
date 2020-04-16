#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class AssetDatabaseUtil
{
    public static string TrimmedDataPath => Regex.Replace(Application.dataPath, "(/Assets$)", "").Replace("/", "\\");

    /// <summary>
    /// Returns all fitlered assets of type T in or below folder
    /// </summary>
    /// <typeparam Type="T"> The Type to return </typeparam>
    /// <param Filter="filter"> The filter to use. t: n: l: are all viable </param>
    /// <param name="folder">   folder to start search from </param>
    /// <returns> List of all assets found </returns>
    public static List<T> GetAllAssetsOfType<T>(string filter = "", string[] folder = null) where T : UnityEngine.Object
    {
        List<T> objects = new List<T>();
        string[] assetGUIDs = AssetDatabase.FindAssets(filter, folder);
        if (assetGUIDs == null) { return objects; }

        foreach (var guid in assetGUIDs)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (assetPath != null)
            {
                T temp = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (temp != null)
                {
                    objects.Add(temp);
                }
            }
        }
        return objects;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="asset"></param>
    /// <param name="path">Wants  "Assets/..."</param>
    /// <param name="assetName"></param>
    /// <param name="fileEnding"></param>
    public static void CreateAsset(UnityEngine.Object asset, string path, string assetName, string fileEnding = ".asset")
    {
        if (!(path.EndsWith("/") || path.EndsWith(@"\")))
        {
            path += "/";
        }
        path = CreateAndReturnUniqueFilePath(path + assetName + fileEnding);
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void CreateAsset(UnityEngine.Object asset, string path, string assetName, out string uniquePath, string fileEnding = ".asset")
    {
        if (!(path.EndsWith("/") || path.EndsWith(@"\")))
        {
            path += "/";
        }
        path = CreateAndReturnUniqueFilePath(path + assetName + fileEnding);
        uniquePath = path;
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static string CreateAndReturnUniqueFilePath(string path)
    {
        var fullPath = Path.Combine(TrimmedDataPath, path);
        Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
        path = AssetDatabase.GenerateUniqueAssetPath(path);
        return path;
    }

    public static void CreateAssetNoRefresh(UnityEngine.Object asset, string path, string assetName, string fileEnding = ".asset")
    {
        if (!(path.EndsWith("/") || path.EndsWith(@"\")))
        {
            path += "/";
        }
        path = AssetDatabase.GenerateUniqueAssetPath(path + assetName + fileEnding);
        AssetDatabase.CreateAsset(asset, path);
        AssetDatabase.SaveAssets();
    }

    public static void RemoveAssetsAtPath<T>(string path) where T : UnityEngine.Object
    {
        var preloadedAssets = AssetDatabaseUtil.GetAllAssetsOfType<T>("t:" + typeof(T).Name, new string[] { path });
        if (preloadedAssets.Count > 0)
        {
            foreach (var item in preloadedAssets)
            {
                AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(item));
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Removing all assets in Directory " + path);
        }
    }

    public static List<T> GetAllAssetRepresentationsAtObjectPath<T>(UnityEngine.Object asset) where T : UnityEngine.Object
    {
        List<T> returnList = new List<T>();
        foreach (var file in GetAllFilesAtPath(GetFolderPathFromAsset(asset)))
        {
            var unityFile = ConvertPathToUnity(file);
            var assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(unityFile);
            returnList.AddRange(assets.Where((x) => typeof(T).IsAssignableFrom(x.GetType())).Cast<T>().ToList());
        }
        return returnList;
    }

    public static List<T> GetAllAssetsAtPath<T>(string path) where T : UnityEngine.Object
    {
        List<T> returnList = new List<T>();
        foreach (var file in GetAllFilesAtPath(path))
        {
            var unityFile = ConvertPathToUnity(file);
            var assets = AssetDatabase.LoadAllAssetsAtPath(unityFile);
            returnList.AddRange(assets.Where((x) => typeof(T).IsAssignableFrom(x.GetType())).Cast<T>().ToList());
        }
        return returnList;
    }

    public static List<T> GetAllAssetsAtPathFromObject<T>(UnityEngine.Object asset) where T : UnityEngine.Object
    {
        List<T> returnList = new List<T>();
        foreach (var file in GetAllFilesAtPath(GetFolderPathFromAsset(asset)))
        {
            var unityFile = ConvertPathToUnity(file);
            var assets = AssetDatabase.LoadAllAssetsAtPath(unityFile);
            returnList.AddRange(assets.Where((x) => typeof(T).IsAssignableFrom(x.GetType())).Cast<T>().ToList());
        }
        return returnList;
    }

    public static List<T> GetAllMainAssetsAtObjectPath<T>(UnityEngine.Object asset) where T : UnityEngine.Object
    {
        List<T> returnList = new List<T>();
        foreach (var file in GetAllFilesAtPath(GetFolderPathFromAsset(asset)))
        {
            var mainAsset = AssetDatabase.LoadMainAssetAtPath(file);
            if (typeof(T).IsAssignableFrom(mainAsset.GetType()))
            {
                returnList.Add(mainAsset as T);
            }
        }
        return returnList;
    }

    public static string GetPathFromAsset(UnityEngine.Object asset) => AssetDatabase.GetAssetPath(asset);

    public static string GetFolderPathFromAsset(UnityEngine.Object asset) => Regex.Replace(GetPathFromAsset(asset), "(?!.*/).+(.*)$", "").Replace("/", "\\");

    public static string GetFolderFromPath(string path) => Regex.Replace(path, "(?!.*/).+(.*)$", "").Replace("/", "\\");

    public static string[] GetAllFilesAtPath(string localUnityPath, bool excludeMeta = true)
    {
        string[] files = null;
        try
        {
            files = System.IO.Directory.GetFiles(Path.Combine(TrimmedDataPath, localUnityPath));
            if (excludeMeta)
            {
                files = files.Where(x => !Regex.IsMatch(x, "(.meta)$")).ToArray();
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
        return files;
    }

    public static string ConvertPathToUnity(string path)
    {
        return path.Replace(TrimmedDataPath, "").TrimStart('\\', '/').Replace("\\", "/");
    }
}

#endif