
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.StringLoading;
using VRC.SDKBase;
using VRC.Udon.Common.Interfaces;

[UdonBehaviourSyncMode(BehaviourSyncMode.Continuous)]
public class WorldBan : UdonSharpBehaviour
{
    public GameObject[] walls;
    //public List<string> banList;
    public string[] banList;
    public Text textBanlist;
    public Text textMy;
    //public VRCSceneDescriptor sceneDescriptor;
    //public VRC_SceneDescriptor sceneDescriptor;
    public Transform[] spawnPoints;
    public Transform banUserSpawnPoint;


    public VRCUrl[] banListUrls;
    //public bool kickMode=false;
    void Start()
    {
        LoadURL();


        foreach (var wall in walls)
        {
            if (wall != null)
            {
                wall.SetActive(false);
            }
        }
        Ban();

    }
    public override void OnStringLoadError(IVRCStringDownload result)
    {
        Debug.LogError($"Error loading string: {result.ErrorCode} - {result.Error}");
        //Invoke("LoadURL",5f);
        //LoadURL();
    }
    void LoadURL()
    {
        foreach (var banListUrl in banListUrls)
        {
            VRCStringDownloader.LoadUrl(banListUrl, (IUdonEventReceiver)this);
        }
    }
    public override void OnStringLoadSuccess(IVRCStringDownload result)
    {
        if (result==null)
        {
            return;
        }
        var resultAsUTF8 = result.Result;
        var texts = resultAsUTF8.Split('\n', '\r');
        var banListCopy = new string[banList.Length];
        for (int i = 0; i < banListCopy.Length; i++)
        {
            banListCopy[i] = banList[i];
        }
       
        //foreach (var item in texts)
        //{
        //    if (string.IsNullOrEmpty(item))
        //    {
        //        continue;
        //    }
        //    //banList.Add(item.Trim());
        //    //banList.Add(item);
        //}
        //banList.AddRange(texts);
        banList = new string[banList.Length+ texts.Length];

        for (int i = 0; i < banListCopy.Length; i++)
        {
            banList[i] = banListCopy[i];
        }
        for (int i = banListCopy.Length; i < banList.Length; i++)
        {
            banList[i] = texts[i- banListCopy.Length];
        }
        Ban();
    }
    void Update()
    {
        if (Time.time<10+ (banListUrls.Length*5))
        {
            Ban();
        }
    }
    void Ban()
    {
        var displayName = Networking.LocalPlayer.displayName;
        //var playerId = Networking.LocalPlayer.ToString();
        if (textMy != null)
        {
            textMy.text = $"YourName: {displayName}\n";
            //textMy.text += $"YourID: {playerId}\n";
        }
        //text.text += $"\n";


        if (textBanlist != null)
        {
            textBanlist.text = string.Join("\n", banList);
        }

        if (Networking.LocalPlayer==null)
        {
            return;
        }
        if (string.IsNullOrEmpty(displayName))
        {
            return;
        }
        if (banList == null)
        {
            return;
        }
        foreach (var banid in banList)
        {
            if (displayName != banid)
            {
                continue;
            }
            /*
            if (playerId != banid)
            {
                continue;
            }
            */
            foreach (var wall in walls)
            {
                if (wall != null)
                {
                    wall.SetActive(true);
                }
            }

            if (banUserSpawnPoint != null)
            {
                /*
                if (sceneDescriptor != null)
                {
                    if (sceneDescriptor.spawns != null)
                    {
                        foreach (var spawnPoint in sceneDescriptor.spawns)
                        {
                            spawnPoint.transform.position = banUserSpawnPoint.transform.position;
                        }
                    }
                    else
                    {
                        sceneDescriptor.transform.position = banUserSpawnPoint.transform.position;
                    }
                }
                */
                foreach (var spawnPoint in spawnPoints)
                {
                    if (spawnPoint!=null)
                    {
                        spawnPoint.transform.position = banUserSpawnPoint.transform.position;
                    }
                }
                Networking.LocalPlayer.TeleportTo(banUserSpawnPoint.transform.position, banUserSpawnPoint.transform.rotation);
            }
            break;
        }

    }
}
