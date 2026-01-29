using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using Unity.Serialization.Json;


public static class PlayerPrefZ
{
    public static bool HasKey(string key)
    {
        return PlayerPrefs.HasKey(key);
    }
    public static void SetBool(string key, bool val)
    {
        PlayerPrefs.SetInt(key, val ? 1 : 0);
    }
    public static bool GetBool(string key)
    {
        int intValue = PlayerPrefs.GetInt(key, -1);
        if (intValue == 1)
        {
            return true;
        }
        else if (intValue == 0)
        {
            return false;
        }
        else
        {
            // Handle the case where the key doesn't exist or has an unexpected value
            // You can return a default value or take appropriate action based on your needs
            return false;
        }
    }
    public static void SetVector3(string key, Vector3 value)
    {
        PlayerPrefs.SetFloat(key + "_X", value.x);
        PlayerPrefs.SetFloat(key + "_Y", value.y);
        PlayerPrefs.SetFloat(key + "_Z", value.z);
        PlayerPrefs.Save();
    }

    // Retrieve a Vector3
    public static Vector3 GetVector3(string key)
    {
        float x = PlayerPrefs.GetFloat(key + "_X", 0);
        float y = PlayerPrefs.GetFloat(key + "_Y", 0);
        float z = PlayerPrefs.GetFloat(key + "_Z", 0);
        return new Vector3(x, y, z);
    }
    public static void SetVector2(string key, Vector2 value)
    {
        PlayerPrefs.SetFloat(key + "_X", value.x);
        PlayerPrefs.SetFloat(key + "_Y", value.y);
        PlayerPrefs.Save();
    }

    public static Vector2 GetVector2(string key)
    {
        float x = PlayerPrefs.GetFloat(key + "_X", 0);
        float y = PlayerPrefs.GetFloat(key + "_Y", 0);
        return new Vector2(x, y);
    }
    public static void SetVector4(string key, Vector4 value)
    {
        PlayerPrefs.SetFloat(key + "_X", value.x);
        PlayerPrefs.SetFloat(key + "_Y", value.y);
        PlayerPrefs.SetFloat(key + "_Z", value.z);
        PlayerPrefs.SetFloat(key + "_W", value.w);
        PlayerPrefs.Save();
    }

    public static Vector4 GetVector4(string key)
    {
        float x = PlayerPrefs.GetFloat(key + "_X", 0);
        float y = PlayerPrefs.GetFloat(key + "_Y", 0);
        float z = PlayerPrefs.GetFloat(key + "_Z", 0);
        float w = PlayerPrefs.GetFloat(key + "_W", 0);
        return new Vector4(x, y, z, w);
    }
    public static void SetQuaternion(string key, Quaternion value)
    {
        PlayerPrefs.SetFloat(key + "_X", value.x);
        PlayerPrefs.SetFloat(key + "_Y", value.y);
        PlayerPrefs.SetFloat(key + "_Z", value.z);
        PlayerPrefs.SetFloat(key + "_W", value.w);
        PlayerPrefs.Save();
    }

    public static Quaternion GetQuaternion(string key)
    {
        float x = PlayerPrefs.GetFloat(key + "_X", 0);
        float y = PlayerPrefs.GetFloat(key + "_Y", 0);
        float z = PlayerPrefs.GetFloat(key + "_Z", 0);
        float w = PlayerPrefs.GetFloat(key + "_W", 0);
        return new Quaternion(x, y, z, w);
    }
    public static void SetData<T>(string key, T data)
    {
        string str = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, str);
    }
    public static T GetData<T>(string key, T defaultData)
    {
        if (HasKey(key))
        {
            string str = PlayerPrefs.GetString(key);
            T data = JsonUtility.FromJson<T>(str);
            return data;
        }
        else
        {
            return defaultData;
        }
    }
    public static T GetData<T>(string key)
    {
        return GetData(key, default(T));
        // if (HasKey(key))
        // {
        //     string str = PlayerPrefs.GetString(key);
        //     T data = JsonSerialization.FromJson<T>(str);
        //     return data;
        // }
        // else
        // {
        //     return default;
        // }
    }
    public static void SaveDictionary<Tkey, TValue>(string key, Dictionary<Tkey, TValue> dictionary)
    {
        string dictionaryJson = JsonUtility.ToJson(dictionary);
        PlayerPrefs.SetString(key, dictionaryJson);
    }
    // Load a dictionary from PlayerPrefs
    public static Dictionary<Tkey, TValue> LoadDictionary<Tkey, TValue>(string key, Dictionary<Tkey, TValue> defVal)
    {
        if (PlayerPrefs.HasKey(key))
        {
            string dictionaryJson = PlayerPrefs.GetString(key);
            return JsonUtility.FromJson<Dictionary<Tkey, TValue>>(dictionaryJson);
            // return serializableDictionary;
        }
        return defVal;
    }
}
