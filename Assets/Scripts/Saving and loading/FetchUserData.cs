using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class FetchUserData
{
    private static          FetchUserData _loader;
    private static readonly object        _lock = new object();
    public static FetchUserData Loader
    {
        get
        {
            //double-check locking pattern for safety
            if (_loader == null)
            {
                lock (_lock)
                {
                    if (_loader == null)
                    {
                        _loader = new FetchUserData();
                    }
                }
            }
            return _loader;
        }
    }    
    
    
    public enum UserDataQuery
    {
        prologueSeen,
        playedBefore,
        storyAWon,
        storyBWon,
        storyCWon,
        storyAIntroSeen,
        storyBIntroSeen,
        storyCIntroSeen
    }


    public UserData GetUserData()
    {
        string userDataFileLocation = FilePathConstants.GetUserDataFileLocation();
        string userDataFileJsonContents = FilePathConstants.GetSafeFileContents(userDataFileLocation, "User Data", "Loading");
        if (userDataFileJsonContents is null)
        {
            Debug.LogError("No userdata was found");
            return null;
        }

        return JsonConvert.DeserializeObject<UserData>(userDataFileJsonContents);
    }
    
    /// <summary>
    /// Gets a <see cref="SaveData"/> object from the save file contents
    /// </summary>
    public bool GetUserDataValue(UserDataQuery query)
    {
        string userDataFileLocation = FilePathConstants.GetUserDataFileLocation();
        string userDataFileJsonContents = FilePathConstants.GetSafeFileContents(userDataFileLocation, "User Data", "Loading");
        if (userDataFileJsonContents is null)
        {
            Debug.LogError("No userdata was found");
            return false;
        }

        UserData userData = JsonConvert.DeserializeObject<UserData>(userDataFileJsonContents);

        switch (query)
        {
            case UserDataQuery.prologueSeen:
                return userData.prologueSeen;
            case UserDataQuery.playedBefore:
                return userData.playedBefore;
            case UserDataQuery.storyAWon:
                return userData.storyAWon;
            case UserDataQuery.storyBWon:
                return userData.storyBWon;
            case UserDataQuery.storyCWon:
                return userData.storyCWon;
            case UserDataQuery.storyAIntroSeen:
                return userData.storyAIntroSeen;
            case UserDataQuery.storyBIntroSeen:
                return userData.storyBIntroSeen;
            case UserDataQuery.storyCIntroSeen:
                return userData.storyCIntroSeen;
            default:
                Debug.LogError("Invalid UserData query");
                return false;
        }
    }
}
