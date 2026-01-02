using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class CountryCode
{
    public static bool IsInNoTenjinCountries()
    {

        string Country = GetCountryCode();
        string timeZone = GetTimeZone();
        bool isNoTenjinCountr = IsNoTenjinCountry(Country);
        bool isNoTenjinZone = IsNotenjinTimeZone(timeZone);
        Debug.Log("Country " + Country);
        Debug.Log("Country Time Zone " + timeZone);
        if (isNoTenjinCountr || isNoTenjinZone)
        {
            Debug.Log("Country NoTenjin " + (isNoTenjinCountr || isNoTenjinZone));
            PlayerPrefs.SetInt("noTenjin", 1);
            return true;
        }
        else
        {
            Debug.Log("Country Use Tenjin ");
            PlayerPrefs.SetInt("noTenjin", 0);
            return false;
        }
    }
    private static string cachedCountryCode = null;
    private static string cachedTimeZone = null;
    // Public method to get country code
    public static string GetCountryCode()
    {
        if (cachedCountryCode == null)
        {
            cachedCountryCode = RetrieveCountryCode();
        }
        return cachedCountryCode;
    }

    // Public method to get time zone
    public static string GetTimeZone()
    {
        if (cachedTimeZone == null)
        {
            cachedTimeZone = RetrieveTimeZone();
        }
        return cachedTimeZone;
    }

    // Platform-specific country code retrieval
    private static string RetrieveCountryCode()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return GetCountryCodeAndroid();
#elif UNITY_IOS && !UNITY_EDITOR
        return IOSgetPhoneCountryCode(); // Ensure this is correctly implemented
#else
        return "US"; // Default fallback
#endif
    }

    // Platform-specific time zone retrieval
    private static string RetrieveTimeZone()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        return GetTimeZoneAndroid();
#elif UNITY_IOS && !UNITY_EDITOR
        return IOSGetTimeZone();
#else
        return TimeZoneInfo.Local.Id; // Default fallback
#endif
    }

    // Android country code retrieval
    public static string GetCountryCodeAndroid()
    {
        try
        {
            using (AndroidJavaClass cls = new AndroidJavaClass("java.util.Locale"))
            {
                using (AndroidJavaObject locale = cls.CallStatic<AndroidJavaObject>("getDefault"))
                {
                    string android_country = locale.Call<string>("getCountry"); // e.g., "BR"
                    return android_country;
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error retrieving Android country code: " + e.Message);
            return "US"; // Fallback
        }
    }

    // Android time zone retrieval
    public static string GetTimeZoneAndroid()
    {
        try
        {
            using (AndroidJavaClass timeZoneClass = new AndroidJavaClass("java.util.TimeZone"))
            {
                string timeZoneID = timeZoneClass.CallStatic<AndroidJavaObject>("getDefault").Call<string>("getID");
                return timeZoneID;
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error retrieving Android time zone: " + e.Message);
            return TimeZoneInfo.Local.Id; // e.g., "America/Sao_Paulo"
        }
    }

    // iOS country code retrieval via native plugin
#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern string IOSgetPhoneCountryCode();
#endif

#if UNITY_IOS
    [DllImport("__Internal")]
    private static extern string IOSGetTimeZone();
#endif

    public static bool IsNoTenjinCountry(string CountryCode)
    {
        string[] NoTenjinCountry = new string[]
        {
            // "BR",   // Brazil
            // "IN",   // India
            // "MX",   // Mexico
            // "RU",   // Russia
            // "ID"    // Indonesia
        };

        return NoTenjinCountry.Contains(CountryCode);
    }

    // Method to check if time zone is within Brazil's time zones
    public static bool IsNotenjinTimeZone(string timeZoneID)
    {
        // Comprehensive list of Brazil's time zones
        string[] noTenjinZones = new string[]
        {
            // // Brazil Zones
            // "America/Rio_Branco",    // Acre Time (ACT) - UTC-5
            // "America/Manaus",        // Amazon Time (AMT) - UTC-4
            // "America/Cuiaba",        // Central-West Time (AMT) - UTC-4
            // "America/Sao_Paulo",     // Brasília Time (BRT) - UTC-3
            // "America/Fortaleza",     // Fortaleza Time (BRT) - UTC-3
            // "America/Noronha",       // Fernando de Noronha Time (FNT) - UTC-2

            // // India Zone
            // "Asia/Kolkata",          // Indian Standard Time (IST) - UTC+5:30

            // // Mexico Zones
            // "America/Tijuana",       // Northwest Time (PST) - UTC-8
            // "America/Hermosillo",    // Sonora Time (UTC-7)
            // "America/Mazatlan",      // Mountain Time (MST) - UTC-7
            // "America/Mexico_City",   // Central Time (CST) - UTC-6
            // "America/Cancun",        // Southeastern Time (EST) - UTC-5

            // // Russia Zones
            // "Europe/Kaliningrad",    // Kaliningrad Time - UTC+2
            // "Europe/Moscow",         // Moscow Time - UTC+3
            // "Europe/Samara",         // Samara Time - UTC+4
            // "Asia/Yekaterinburg",    // Yekaterinburg Time - UTC+5
            // "Asia/Novosibirsk",      // Novosibirsk Time - UTC+7
            // "Asia/Krasnoyarsk",      // Krasnoyarsk Time - UTC+7
            // "Asia/Irkutsk",          // Irkutsk Time - UTC+8
            // "Asia/Yakutsk",          // Yakutsk Time - UTC+9
            // "Asia/Vladivostok",      // Vladivostok Time - UTC+10
            // "Asia/Magadan",          // Magadan Time - UTC+11
            // "Asia/Kamchatka",        // Kamchatka Time - UTC+12

            // // Indonesia Zones
            // "Asia/Jakarta",          // Western Indonesia Time (WIB) - UTC+7
            // "Asia/Pontianak",        // Western Indonesia Time (WIB) - UTC+7
            // "Asia/Makassar",         // Central Indonesia Time (WITA) - UTC+8
            // "Asia/Jayapura"          // Eastern Indonesia Time (WIT) - UTC+9
        };


        return noTenjinZones.Contains(timeZoneID);
    }
}
