
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PathUtilities
{
    /// <summary>
    /// Recursively walks all directories in the specified path. Returns the asset path for any folders that do not contain any sub-folders (a "leaf" on the tree.)
    /// </summary>
    public static IEnumerable<string> GetLeafDirectories(string path)
    {
        var subfolders = AssetDatabase.GetSubFolders(path);
        if (subfolders.Length > 0)
        {
            foreach (var subfolder in subfolders)            {
                foreach (var result in GetLeafDirectories(subfolder))
                {
                    yield return result;
                }
            }
        }
        else
        {
            //This is a leaf node, return it here.
            yield return path;
        }
    }
}#endif