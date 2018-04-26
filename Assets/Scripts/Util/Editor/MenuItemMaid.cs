using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class MenuItemMaid
{
    [MenuItem("Create/Cymon/SongData")]
    public static void CreateSongData()
    {
        SongData data = ScriptableObject.CreateInstance<SongData>();
        AssetDatabase.CreateAsset(data, AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/SongData/NewSongData.asset"));
    }
}
