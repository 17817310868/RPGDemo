using UnityEditor;
using UnityEngine;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using LuaFramework;

public class Packager {
    public static string platform = string.Empty;
    static List<string> paths = new List<string>();
    static List<string> files = new List<string>();
    static List<AssetBundleBuild> maps = new List<AssetBundleBuild>();

    ///-----------------------------------------------------------
    static string[] exts = { ".txt", ".xml", ".lua", ".assetbundle", ".json" };
    static bool CanCopy(string ext) {   //能不能复制
        foreach (string e in exts) {
            if (ext.Equals(e)) return true;
        }
        return false;
    }

    /// <summary>
    /// 载入素材
    /// </summary>
    static UnityEngine.Object LoadAsset(string file) {
        if (file.EndsWith(".lua")) file += ".txt";
        return AssetDatabase.LoadMainAssetAtPath("Assets/LuaFramework/Examples/Builds/" + file);
    }

    [MenuItem("LuaFramework/Build iPhone Resource", false, 100)]
    public static void BuildiPhoneResource() {
        BuildTarget target = BuildTarget.iOS;
        BuildAssetResource(target);
    }

    [MenuItem("LuaFramework/Build Android Resource", false, 101)]
    public static void BuildAndroidResource() {
        BuildAssetResource(BuildTarget.Android);
    }

    [MenuItem("LuaFramework/Build Windows Resource", false, 102)]
    public static void BuildWindowsResource() {
        BuildAssetResource(BuildTarget.StandaloneWindows);
    }

    /// <summary>
    /// 生成绑定素材
    /// </summary>
    public static void BuildAssetResource(BuildTarget target) {
        if (Directory.Exists(Util.DataPath)) {
            Directory.Delete(Util.DataPath, true);
        }
        string streamPath = Application.streamingAssetsPath;
        if (Directory.Exists(streamPath)) {
            Directory.Delete(streamPath, true);
        }
        Directory.CreateDirectory(streamPath);
        AssetDatabase.Refresh();

        maps.Clear();
        if (AppConst.LuaBundleMode) {
            HandleLuaBundle();
        } else {
            HandleLuaFile();
        }
        if (AppConst.ExampleMode) {
        }
        else
        {
            HandleBundle();
        }
        string resPath = "Assets/" + AppConst.AssetDir;
        BuildPipeline.BuildAssetBundles(resPath, maps.ToArray(), BuildAssetBundleOptions.None, target);
        BuildFileIndex();

        string streamDir = Application.dataPath + "/" + AppConst.LuaTempDir;
        if (Directory.Exists(streamDir)) Directory.Delete(streamDir, true);
        AssetDatabase.Refresh();
    }

    static void AddBuildMap(string bundleName, string pattern, string path) {
        string[] files = Directory.GetFiles(path, pattern);
        if (files.Length == 0) return;

        for (int i = 0; i < files.Length; i++) {
            files[i] = files[i].Replace('\\', '/');
        }
        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = bundleName;
        build.assetNames = files;
        maps.Add(build);
    }

    /// <summary>
    /// 处理Lua代码包
    /// </summary>
    static void HandleLuaBundle() {
        string streamDir = Application.dataPath + "/" + AppConst.LuaTempDir;
        if (!Directory.Exists(streamDir)) Directory.CreateDirectory(streamDir);

        string[] srcDirs = { CustomSettings.luaDir, CustomSettings.FrameworkPath + "/ToLua/Lua" };
        for (int i = 0; i < srcDirs.Length; i++) {
            if (AppConst.LuaByteMode) {
                string sourceDir = srcDirs[i];
                string[] files = Directory.GetFiles(sourceDir, "*.lua", SearchOption.AllDirectories);
                int len = sourceDir.Length;

                if (sourceDir[len - 1] == '/' || sourceDir[len - 1] == '\\') {
                    --len;
                }
                for (int j = 0; j < files.Length; j++) {
                    string str = files[j].Remove(0, len);
                    string dest = streamDir + str + ".bytes";
                    string dir = Path.GetDirectoryName(dest);
                    Directory.CreateDirectory(dir);
                    EncodeLuaFile(files[j], dest);
                }    
            } else {
                ToLuaMenu.CopyLuaBytesFiles(srcDirs[i], streamDir);
            }
        }
        string[] dirs = Directory.GetDirectories(streamDir, "*", SearchOption.AllDirectories);
        for (int i = 0; i < dirs.Length; i++) {
            string name = dirs[i].Replace(streamDir, string.Empty);
            name = name.Replace('\\', '_').Replace('/', '_');
            name = "lua/lua_" + name.ToLower() + AppConst.ExtName;

            string path = "Assets" + dirs[i].Replace(Application.dataPath, "");
            AddBuildMap(name, "*.bytes", path);
        }
        AddBuildMap("lua/lua" + AppConst.ExtName, "*.bytes", "Assets/" + AppConst.LuaTempDir);

        //-------------------------------处理非Lua文件----------------------------------
        string luaPath = AppDataPath + "/StreamingAssets/lua/";
        for (int i = 0; i < srcDirs.Length; i++) {
            paths.Clear(); files.Clear();
            string luaDataPath = srcDirs[i].ToLower();
            Recursive(luaDataPath);
            foreach (string f in files) {
                if (f.EndsWith(".meta") || f.EndsWith(".lua")) continue;
                string newfile = f.Replace(luaDataPath, "");
                string path = Path.GetDirectoryName(luaPath + newfile);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                string destfile = path + "/" + Path.GetFileName(f);
                File.Copy(f, destfile, true);
            }
        }
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 处理框架实例包
    /// </summary>
    static void HandleExampleBundle() {
        string resPath = AppDataPath + "/" + AppConst.AssetDir + "/";
        if (!Directory.Exists(resPath)) Directory.CreateDirectory(resPath);

        AddBuildMap("prompt" + AppConst.ExtName, "*.prefab", "Assets/LuaFramework/Examples/Builds/Prompt");
        AddBuildMap("message" + AppConst.ExtName, "*.prefab", "Assets/LuaFramework/Examples/Builds/Message");

        AddBuildMap("prompt_asset" + AppConst.ExtName, "*.png", "Assets/LuaFramework/Examples/Textures/Prompt");
        AddBuildMap("shared_asset" + AppConst.ExtName, "*.png", "Assets/LuaFramework/Examples/Textures/Shared");
    }

    /// <summary>
    /// 打包非脚本资源
    /// </summary>
    static void HandleBundle()
    {
        string resPath = AppDataPath + "/" + AppConst.AssetDir + "/";
        if (!Directory.Exists(resPath)) Directory.CreateDirectory(resPath);

        //登录注册面板资源
        AddBuildMap("login" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Login");
        AddBuildMap("loginbg" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/LoginBg");
        AddBuildMap("register" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Register");
        //AddBuildMap("login_asset" + AppConst.ExtName, "*png", "Assets/LuaFramework/AssetBundles/Textures/Login");
        //AddBuildMap("loginbg_asset" + AppConst.ExtName, "*png", "Assets/LuaFramework/AssetBundles/Textures/LoginBg");

        //打包摄像机资源
        AddBuildMap("camera" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Camera");

        //创建角色面板资源
        AddBuildMap("createrole" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/CreateRole");
        AddBuildMap("professionicon" + AppConst.ExtName, "*png", "Assets/LuaFramework/AssetBundles/Textures/ProfessionIcon");
        AddBuildMap("schoolicon" + AppConst.ExtName, "*png", "Assets/LuaFramework/AssetBundles/Textures/SchoolIcon");

        //遮罩面板资源
        AddBuildMap("mask" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Mask");
        AddBuildMap("tipmask" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/TipMask");

        //人物资源
        AddBuildMap("nanqiang" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NanQiangModels/NanQiang");
        AddBuildMap("nanjian" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NanJianModels/NanJian");
        AddBuildMap("nvgong" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NvGongModels/NvGong");
        AddBuildMap("nanshan" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NanShanModels/NanShan");
        AddBuildMap("nvzhang" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NvZhangModels/NvZhang");

        //Npc模型
        AddBuildMap("zhanchi" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/ZhanChi");
        AddBuildMap("kunwu" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/KunWu");

        //怪兽模型
        AddBuildMap("ling" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Ling");

        //AddBuildMap("npcicon" + AppConst.ExtName, "*png", "Assets/LuaFramework/AssetBundles/Textures/Npc");
        AddBuildMap("taskicon" + AppConst.ExtName, "*png", "Assets/LuaFramework/AssetBundles/Textures/Task");

        //人物武器mesh
        //AddBuildMap("nanqiang_asset" + AppConst.ExtName, "*asset", "Assets/LuaFramework/AssetBundles/Meshs/NanQiang");
        //AddBuildMap("nanjian_asset" + AppConst.ExtName, "*asset", "Assets/LuaFramework/AssetBundles/Meshs/NanJian");
        //AddBuildMap("nvgong_asset" + AppConst.ExtName, "*asset", "Assets/LuaFramework/AssetBundles/Meshs/NvGong");
        //AddBuildMap("nanshan_asset" + AppConst.ExtName, "*asset", "Assets/LuaFramework/AssetBundles/Meshs/NanShan");
        //AddBuildMap("nvzhang_asset" + AppConst.ExtName, "*asset", "Assets/LuaFramework/AssetBundles/Meshs/NvZhang");


        //武器资源
        AddBuildMap("wuqi_qiang01" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NanQiangModels/wuqi_qiang01");
        AddBuildMap("wuqi_jian01" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NanJianModels/wuqi_jian01");
        AddBuildMap("wuqi_gong01" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NvGongModels/wuqi_gong01");
        AddBuildMap("wuqi_shan01" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NanShanModels/wuqi_shan01");
        AddBuildMap("wuqi_fazhang01" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NvZhangModels/wuqi_fazhang01");
        AddBuildMap("wuqi_qiang02" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NanQiangModels/wuqi_qiang02");
        AddBuildMap("wuqi_jian02" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NanJianModels/wuqi_jian02");
        AddBuildMap("wuqi_gong02" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NvGongModels/wuqi_gong02");
        AddBuildMap("wuqi_shan02" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NanShanModels/wuqi_shan02");
        AddBuildMap("wuqi_fazhang02" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NvZhangModels/wuqi_fazhang02");
        AddBuildMap("wuqi_qiang03" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NanQiangModels/wuqi_qiang03");
        AddBuildMap("wuqi_jian03" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NanJianModels/wuqi_jian03");
        AddBuildMap("wuqi_gong03" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NvGongModels/wuqi_gong03");
        AddBuildMap("wuqi_shan03" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NanShanModels/wuqi_shan03");
        AddBuildMap("wuqi_fazhang03" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NvZhangModels/wuqi_fazhang03");
        AddBuildMap("wuqi_qiang04" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NanQiangModels/wuqi_qiang04");
        AddBuildMap("wuqi_jian04" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NanJianModels/wuqi_jian04");
        AddBuildMap("wuqi_gong04" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NvGongModels/wuqi_gong04");
        AddBuildMap("wuqi_shan04" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NanShanModels/wuqi_shan04");
        AddBuildMap("wuqi_fazhang04" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NvZhangModels/wuqi_fazhang04");
        AddBuildMap("wuqi_qiang05" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NanQiangModels/wuqi_qiang05");
        AddBuildMap("wuqi_jian05" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NanJianModels/wuqi_jian05");
        AddBuildMap("wuqi_gong05" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NvGongModels/wuqi_gong05");
        AddBuildMap("wuqi_shan05" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NanShanModels/wuqi_shan05");
        AddBuildMap("wuqi_fazhang05" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/NvZhangModels/wuqi_fazhang05");

        //动画资源
        //AddBuildMap("nanqiang_ani" + AppConst.ExtName, "*fbx", "Assets/LuaFramework/AssetBundles/Animations/NanQiang");
        //AddBuildMap("nanjian_ani" + AppConst.ExtName, "*fbx", "Assets/LuaFramework/AssetBundles/Animations/NanJian");
        //AddBuildMap("nvgong_ani" + AppConst.ExtName, "*fbx", "Assets/LuaFramework/AssetBundles/Animations/NvGong");
        //AddBuildMap("nanshan_ani" + AppConst.ExtName, "*fbx", "Assets/LuaFramework/AssetBundles/Animations/NanShan");
        //AddBuildMap("nvzhang_ani" + AppConst.ExtName, "*fbx", "Assets/LuaFramework/AssetBundles/Animations/NvZhang");

        //动画控制器资源
        //AddBuildMap("animator_ctrl" + AppConst.ExtName, "*controller", "Assets/LuaFramework/AssetBundles/Animators");


        //贴图资源
        //AddBuildMap("nanqiang_tga" + AppConst.ExtName, "*tga", "Assets/LuaFramework/AssetBundles/Textures/NanQiang");
        //AddBuildMap("nanjian_tga" + AppConst.ExtName, "*tga", "Assets/LuaFramework/AssetBundles/Textures/NanJian");
        //AddBuildMap("nvgong_tga" + AppConst.ExtName, "*tga", "Assets/LuaFramework/AssetBundles/Textures/NvGong");
        //AddBuildMap("nanshan_tga" + AppConst.ExtName, "*tga", "Assets/LuaFramework/AssetBundles/Textures/NanShan");
        //AddBuildMap("nvzhang_tga" + AppConst.ExtName, "*tga", "Assets/LuaFramework/AssetBundles/Textures/NvZhang");

        //材质资源
        //AddBuildMap("nanqiang_mat" + AppConst.ExtName, "*mat", "Assets/LuaFramework/AssetBundles/Materials/NanQiang");
        //AddBuildMap("nanjian_mat" + AppConst.ExtName, "*mat", "Assets/LuaFramework/AssetBundles/Materials/NanJian");
        //AddBuildMap("nvgong_mat" + AppConst.ExtName, "*mat", "Assets/LuaFramework/AssetBundles/Materials/NvGong");
        //AddBuildMap("nanshan_mat" + AppConst.ExtName, "*mat", "Assets/LuaFramework/AssetBundles/Materials/NanShan");
        //AddBuildMap("nvzhang_mat" + AppConst.ExtName, "*mat", "Assets/LuaFramework/AssetBundles/Materials/NvZhang");

        //AddBuildMap("animator_ctrl" + AppConst.ExtName, "*fbx", "Assets/LuaFramework/AssetBundles/Animators");

        //打包主UI面板
        AddBuildMap("mainui" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/MainUI");
        AddBuildMap("roleicon" + AppConst.ExtName, "*png", "Assets/LuaFramework/AssetBundles/Textures/RoleIcon");

        //打包背包面板
        //AddBuildMap("bag" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Bag");
        AddBuildMap("othericon" + AppConst.ExtName, "*png", "Assets/LuaFramework/AssetBundles/Textures/OtherIcon");

        //打包角色面板
        AddBuildMap("role" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Role");
        //AddBuildMap("roleicon" + AppConst.ExtName, "*png", "Assets/LuaFramework/AssetBundles/Textures/Role");

        //打包物品
        AddBuildMap("item" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Item");
        AddBuildMap("itembg" + AppConst.ExtName, "*png", "Assets/LuaFramework/AssetBundles/Textures/ItemBg");
        AddBuildMap("itemicon" + AppConst.ExtName, "*png", "Assets/LuaFramework/AssetBundles/Textures/ItemIcon");

        //打包物品信息面板
        AddBuildMap("iteminfo" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/ItemInfo");
        //AddBuildMap("iteminfoicon" + AppConst.ExtName, "*png", "Assets/LuaFramework/AssetBundles/Textures/Iteminfo");

        //打包技能面板
        AddBuildMap("skill" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Skill");
        AddBuildMap("skillicon" + AppConst.ExtName, "*png", "Assets/LuaFramework/AssetBundles/Textures/SkillIcon");

        //打包交互面板
        AddBuildMap("eachotherrole" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/EachOtherRole");

        //打包消息面板
        AddBuildMap("message" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Message");

        //打包战斗面板
        AddBuildMap("battle" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Battle");

        //打包用于显示人物头顶面板
        AddBuildMap("headmgr" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/HeadMgr");

        //打包人物头顶预制体
        AddBuildMap("head" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Head");

        //打包粒子特效
        AddBuildMap("attackeffect" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/AttackEffect");
        AddBuildMap("paishandaohai" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/PaiShanDaoHai");
        AddBuildMap("hengsaoqianjun" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/HengSaoQianJun");
        AddBuildMap("fanyunfuyu" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/FanYunFuYu");
        AddBuildMap("fenghuoliantian" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/FengHuoLianTian");

        //打包伤害预制体
        AddBuildMap("hurt" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Hurt");

        //打包buff预制体
        AddBuildMap("buff" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Buff");
        AddBuildMap("bufficon" + AppConst.ExtName, "*png", "Assets/LuaFramework/AssetBundles/Textures/BuffIcon");

        //打包战斗面板
        AddBuildMap("buffinfo" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/BuffInfo");

        //打包成员预制体
        AddBuildMap("member" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Member");

        //打包商城面板
        AddBuildMap("shop" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Shop");

        //打包商品预制体
        AddBuildMap("commodity" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Commodity");

        //打包锻造面板
        AddBuildMap("forge" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Forge");

        //打包装备预制体
        AddBuildMap("equip" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Equip");

        //打包宝石预制体
        AddBuildMap("gem" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Gem");

        //打包制造书预制体
        AddBuildMap("makebook" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/MakeBook");

        //打包材料预制体
        AddBuildMap("material" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Material");

        //打包对话面板预制体
        AddBuildMap("talk" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Talk");

        //打包任务标题预制体
        AddBuildMap("tasktitle" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/TaskTitle");

        //打包任务奖励物品预制体
        AddBuildMap("goods" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Goods");

        //打包任务窗口预制体
        AddBuildMap("taskbox" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/TaskBox");

        //打包排行榜面板预制体
        AddBuildMap("rank" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Rank");
        AddBuildMap("rolelevelrank" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/RoleLevelRank");
        AddBuildMap("rolepowerrank" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/RolePowerRank");

        //打包邮件面板
        AddBuildMap("mail" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Mail");
        AddBuildMap("mailbox" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/MailBox");


        //打包拍卖面板
        AddBuildMap("auction" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Auction");
        AddBuildMap("auctionbox" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/AuctionBox");

        //打包加载面板
        AddBuildMap("loading" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Loading");

        //打包设置面板
        AddBuildMap("setting" + AppConst.ExtName, "*prefab", "Assets/LuaFramework/AssetBundles/Prefabs/Setting");

        AddBuildMap("audio" + AppConst.ExtName, "*mp3", "Assets/LuaFramework/AssetBundles/Audio");

        //AddBuildMap("lua" + AppConst.ExtName, "*lua", "Assets/LuaFramework/AssetBundles/ToLua/Lua");

    }

    /// <summary>
    /// 处理Lua文件
    /// </summary>
    static void HandleLuaFile() {
        string resPath = AppDataPath + "/StreamingAssets/";
        string luaPath = resPath + "/lua/";

        //----------复制Lua文件----------------
        if (!Directory.Exists(luaPath)) {
            Directory.CreateDirectory(luaPath); 
        }
        string[] luaPaths = { AppDataPath + "/LuaFramework/lua/", 
                              AppDataPath + "/LuaFramework/Tolua/Lua/" };

        for (int i = 0; i < luaPaths.Length; i++) {
            paths.Clear(); files.Clear();
            string luaDataPath = luaPaths[i].ToLower();
            Recursive(luaDataPath);
            int n = 0;
            foreach (string f in files) {
                if (f.EndsWith(".meta")) continue;
                string newfile = f.Replace(luaDataPath, "");
                string newpath = luaPath + newfile;
                string path = Path.GetDirectoryName(newpath);
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                if (File.Exists(newpath)) {
                    File.Delete(newpath);
                }
                if (AppConst.LuaByteMode) {
                    EncodeLuaFile(f, newpath);
                } else {
                    File.Copy(f, newpath, true);
                }
                UpdateProgress(n++, files.Count, newpath);
            } 
        }
        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();
    }

    static void BuildFileIndex() {
        string resPath = AppDataPath + "/StreamingAssets/";
        ///----------------------创建文件列表-----------------------
        string newFilePath = resPath + "/files.txt";
        if (File.Exists(newFilePath)) File.Delete(newFilePath);

        paths.Clear(); files.Clear();
        Recursive(resPath);

        FileStream fs = new FileStream(newFilePath, FileMode.CreateNew);
        StreamWriter sw = new StreamWriter(fs);
        for (int i = 0; i < files.Count; i++) {
            string file = files[i];
            string ext = Path.GetExtension(file);
            if (file.EndsWith(".meta") || file.Contains(".DS_Store")) continue;

            string md5 = Util.md5file(file);
            string value = file.Replace(resPath, string.Empty);
            sw.WriteLine(value + "|" + md5);
        }
        sw.Close(); fs.Close();
    }

    /// <summary>
    /// 数据目录
    /// </summary>
    static string AppDataPath {
        get { return Application.dataPath.ToLower(); }
    }

    /// <summary>
    /// 遍历目录及其子目录
    /// </summary>
    static void Recursive(string path) {
        string[] names = Directory.GetFiles(path);
        string[] dirs = Directory.GetDirectories(path);
        foreach (string filename in names) {
            string ext = Path.GetExtension(filename);
            if (ext.Equals(".meta")) continue;
            files.Add(filename.Replace('\\', '/'));
        }
        foreach (string dir in dirs) {
            paths.Add(dir.Replace('\\', '/'));
            Recursive(dir);
        }
    }

    static void UpdateProgress(int progress, int progressMax, string desc) {
        string title = "Processing...[" + progress + " - " + progressMax + "]";
        float value = (float)progress / (float)progressMax;
        EditorUtility.DisplayProgressBar(title, desc, value);
    }

    public static void EncodeLuaFile(string srcFile, string outFile) {
        if (!srcFile.ToLower().EndsWith(".lua")) {
            File.Copy(srcFile, outFile, true);
            return;
        }
        bool isWin = true; 
        string luaexe = string.Empty;
        string args = string.Empty;
        string exedir = string.Empty;
        string currDir = Directory.GetCurrentDirectory();
        if (Application.platform == RuntimePlatform.WindowsEditor) {
            isWin = true;
            luaexe = "luajit.exe";
            args = "-b -g " + srcFile + " " + outFile;
            exedir = AppDataPath.Replace("assets", "") + "LuaEncoder/luajit/";
        } else if (Application.platform == RuntimePlatform.OSXEditor) {
            isWin = false;
            luaexe = "./luajit";
            args = "-b -g " + srcFile + " " + outFile;
            exedir = AppDataPath.Replace("assets", "") + "LuaEncoder/luajit_mac/";
        }
        Directory.SetCurrentDirectory(exedir);
        ProcessStartInfo info = new ProcessStartInfo();
        info.FileName = luaexe;
        info.Arguments = args;
        info.WindowStyle = ProcessWindowStyle.Hidden;
        info.UseShellExecute = isWin;
        info.ErrorDialog = true;
        Util.Log(info.FileName + " " + info.Arguments);

        Process pro = Process.Start(info);
        pro.WaitForExit();
        Directory.SetCurrentDirectory(currDir);
    }

    [MenuItem("LuaFramework/Build Protobuf-lua-gen File")]
    public static void BuildProtobufFile() {
        if (!AppConst.ExampleMode) {
            UnityEngine.Debug.LogError("若使用编码Protobuf-lua-gen功能，需要自己配置外部环境！！");
            return;
        }
        string dir = AppDataPath + "/Lua/3rd/pblua";
        paths.Clear(); files.Clear(); Recursive(dir);

        string protoc = "d:/protobuf-2.4.1/src/protoc.exe";
        string protoc_gen_dir = "\"d:/protoc-gen-lua/plugin/protoc-gen-lua.bat\"";

        foreach (string f in files) {
            string name = Path.GetFileName(f);
            string ext = Path.GetExtension(f);
            if (!ext.Equals(".proto")) continue;

            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = protoc;
            info.Arguments = " --lua_out=./ --plugin=protoc-gen-lua=" + protoc_gen_dir + " " + name;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.UseShellExecute = true;
            info.WorkingDirectory = dir;
            info.ErrorDialog = true;
            Util.Log(info.FileName + " " + info.Arguments);

            Process pro = Process.Start(info);
            pro.WaitForExit();
        }
        AssetDatabase.Refresh();
    }
}