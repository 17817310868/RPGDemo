/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:角色系统
 *          
 *          description:
 *              功能描述:管理所有在线客户端的所有角色（包括主角，伙伴，宠物)
 *              
 *          author:
 *              作者:
 * 
 * ===================================================================
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Net.Share;
using Net.Server;
using UnityEditor;
using UnityEngine;
using Server.GameSys;

namespace Server
{
    public enum RoleEnum
    {
        None = 0,
        MainRole,
        Pet,
        Partner
    }
    class RoleSys
    {
        private static RoleSys instance;
        public static RoleSys Instance
        {
            get
            {
                if (instance == null)
                    instance = new RoleSys();
                return instance;
            }
        }

        public RoleSys()
        {
            EventSys.Instance.AddListener("RoleDataSave", () =>
            {
                foreach(Player player in ServerSys.Instance.Players.Values)
                {
                    SaveRoleData(player);
                }
            });
        }

        //该字典存储各个客户端得角色<客户端<roleEnum<Guid，Role>>>
        Dictionary<Player, Dictionary<int, Dictionary<string,RoleBase>>> rolesDic = new Dictionary<Player, Dictionary<int, Dictionary<string,RoleBase>>>();

        /// <summary>
        /// 初始化角色
        /// </summary>
        /// <param name="player"></param>
        public void InitRole(Player player)
        {
            Console.WriteLine("初始化角色");
            InitMainRole(player);
            //InitPetRole(player);
            //InitPartnerRole(player);
        }

        public void BeginGame(Player player)
        {
            if (DBSys.Instance.GetAllDatas<MainRoleData>("account", player.account).Count < 1)
            {
                //向客户端发送创建角色消息
                ServerSys.Instance.Send(player, "CallLuaFunction", "LoginCtrl.CreateRole");
                return;
            }
            Console.WriteLine("进入场景");

            //向客户端发送进入游戏场景的消息
            PlayerData playerData = DBSys.Instance.GetData<PlayerData>("account", player.account);
            Console.WriteLine(playerData.sceneId == "MainScene");
            if(playerData.sceneId == "MainScene")
            {
                ServerSys.Instance.Send(player, "CallLuaFunction", "LoginCtrl.ChangeScene", new S2C_SceneInfo("taoyuanzhen"));
                if (ServerSys.Instance.Scenes.ContainsKey("taoyuanzhen"))
                    ServerSys.Instance.SwitchScene(player, "taoyuanzhen");
                else
                {
                    ServerSys.Instance.CreateScene(player, "taoyuanzhen");
                    Console.WriteLine("创建场景");
                }
                Console.WriteLine("玩家sceneid : " + player.sceneID);
                Console.WriteLine("桃源镇人数:" + ServerSys.Instance.Scenes["taoyuanzhen"].Players.Count);
                return;
            }
            if(ServerSys.Instance.Scenes.ContainsKey(playerData.sceneId))
                ServerSys.Instance.SwitchScene(player, playerData.sceneId);
            else
                ServerSys.Instance.CreateScene(player, playerData.sceneId);
            Console.WriteLine("桃源镇人数:" + ServerSys.Instance.Scenes["taoyuanzhen"].Players.Count);
            ServerSys.Instance.Send(player,"CallLuaFunction", "LoginCtrl.ChangeScene",new S2C_SceneInfo (playerData.sceneId));
        }

        /// <summary>
        /// 创建主角
        /// </summary>
        /// <param name="player"></param>
        /// <param name="role"></param>
        public void CreateMainRole(Player player,C2S_CreateMainRole role)
        {
            //判断该客户是否已存在主角
            if (DBSys.Instance.GetAllDatas<MainRoleData>("account", player.account).Count > 0)
            {
                //创建失败，向客户端发送操作不合法
                return;
            }
            ProfessionConfig config = ConfigSys.Instance.GetConfig<ProfessionConfig>(role.professionId);

            DBSys.Instance.InsertData<ItemInfoData>(new ItemInfoData(Guid.NewGuid().ToString(),
                player.account, (int)ItemEunm.Item,12020010, (int)InventoryEnum.Bag, 2));
            //默认属性从数据表读取
            DBSys.Instance.InsertData(new MainRoleData(player.account,player.playerID, role.name,config.id,
                config.level, config.hp,config.hp, config.mp, config.mp, config.physicalAttack,
                config.physicalDefense, config.magicAttack,config.magicDefense, config.speed,
                config.experience,config.poisoning,config.poisoningResist,config.burn,
                config.burnResist,config.continueHit,config.strikeBack, role.professionId,
                role.schoolId,config.moveSpeed, config.positionX, config.positionY,
                config.positionZ,config.silver,config.gold,config.yuanBao,1000));
            BeginGame(player);
        }

        //初始化主角
        void InitMainRole(Player player)
        {
            Console.WriteLine("初始化主角");
            //从数据库取出该玩家的主角信息
            MainRoleData mainRoleData = DBSys.Instance.GetData<MainRoleData>("account", player.account);
            //给主角赋值
            MainRole mainRole = new MainRole(mainRoleData.Guid, RoleEnum.MainRole,mainRoleData.professionId);
            mainRole.ChangeName(mainRoleData.name);
            mainRole.ChangeSchoolId(mainRoleData.schoolId);
            mainRole.ChangeLevel(mainRoleData.level);
            mainRole.ChangeMoveSpeed(mainRoleData.moveSpeed);
            mainRole.ChangeHp(mainRoleData.hp);
            mainRole.ChangeMaxHp(mainRoleData.maxHp);
            mainRole.ChangeMp(mainRoleData.mp);
            mainRole.ChangeMaxMp(mainRoleData.maxMp);
            mainRole.ChangePhysicalAttack(mainRoleData.physicalAttack);
            mainRole.ChangePhysicalDefense(mainRoleData.physicalDefense);
            mainRole.ChangeMagicAttack(mainRoleData.magicAttack);
            mainRole.ChangeMagicDefense(mainRoleData.magicDefense);
            mainRole.ChangeSpeed(mainRoleData.speed);
            mainRole.ChangeExperience(mainRoleData.experience);
            mainRole.ChangePosition(new Vector3(mainRoleData.positionX, mainRoleData.positionY, mainRoleData.positionZ));
            mainRole.ChangeSilver(mainRoleData.silver);
            mainRole.ChangeGold(mainRoleData.gold);
            mainRole.ChangeYuanBao(mainRoleData.yuanBao);
            //将主角加入字典
            //AddMainRole(player, mainRole);
            player.mainRole = mainRole;
            InventorySys.Instance.InitItem(player);
            int weaponId = -1;
            if(player.mainRole.Inventory.GetEquipbarEquip((byte)EquipDress.Weapon) != null)
            {
                weaponId = player.mainRole.Inventory.GetEquipbarEquip((byte)EquipDress.Weapon).ItemId;
            }
            
            //if(player.mainRole.Inventory.GetInventoryItems((byte)InventoryEnum.Equip) != null)
            //{
            //    ItemBase[] equips = player.mainRole.Inventory.GetInventoryItems((byte)InventoryEnum.Equip);
            //    for (int i = 0; i < equips.Length; i++)
            //    {
            //        if (ConfigSys.Instance.GetConfig<EquipConfig>(equips[i].ItemId)._type == (byte)EquipDress.Weapon)
            //            weaponId = equips[i].ItemId;
            //    }
            //}

            //自身主角需要在自身客户端显示的信息
            S2C_MainRoleInfo S2C_mainRoleInfo = new S2C_MainRoleInfo(mainRole.Guid,(int)mainRole.RoleType,
                mainRole.Profession,mainRole.SchoolId, mainRole.Name, false, false,
                mainRole.Level,mainRole.MoveSpeed,mainRole.Power,weaponId, mainRole.Position,mainRole.Hp,
                mainRole.MaxHp,mainRole.Mp, mainRole.MaxMp,mainRole.PhysicalAttack,
                mainRole.PhysicalDefense, mainRole.MagicAttack,mainRole.MagicDefense,
                mainRole.Speed,mainRole.Experience,mainRole.Silver,mainRole.Gold,mainRole.YuanBao);
            //向客户端发送主角信息，并初始化主角信息
            ServerSys.Instance.Send(player, "CreateMainRole", S2C_mainRoleInfo);
            Console.WriteLine("向客户端发送初始化主角消息");

            //自身主角在其他客户端需要显示的信息
            S2C_OtherRoleInfo S2C_otherRoleInfo = new S2C_OtherRoleInfo(mainRole.Guid,
                (int)mainRole.RoleType, mainRole.Profession, mainRole.SchoolId, mainRole.Name,
                false, false, mainRole.Level,mainRole.MoveSpeed,mainRole.Power,weaponId, mainRole.Position);

            //用于存储同一场景的在线客户端主角信息（除自身）
            List<S2C_OtherRoleInfo> otherRoles = new List<S2C_OtherRoleInfo>();

            //在其他客户端创建本客户端主角角色，向场景中的所有在线客户端（除自身）发送创建其他角色消息
            //并获取其他客户端需要在本客户端显示的角色信息，并存进列表
            Console.WriteLine(player.sceneID + "该场景所有玩家数：" + (player.Scene as Scene).Players.Count);
            //Console.WriteLine("桃源镇人数:" + ServerSys.Instance.Scenes["taoyuanzhen"].Players.Count);
            foreach (Player client in (player.Scene as Scene).Players)
            {
                if(client.playerID != player.playerID)
                {
                    
                    ServerSys.Instance.Send(client, "CreateOtherRole", S2C_otherRoleInfo);
                    bool isBattle = false;
                    MainRole role = client.mainRole;
                    if (role.State == RoleState.Battle)
                        isBattle = true;
                    int otherWeaponId = -1;
                    if (client.mainRole.Inventory.GetInventoryItems((byte)InventoryEnum.Equip) != null)
                    {
                        ItemBase[] equips = client.mainRole.Inventory.GetInventoryItems((byte)InventoryEnum.Equip);
                        for (int i = 0; i < equips.Length; i++)
                        {
                            if (ConfigSys.Instance.GetConfig<EquipConfig>(equips[i].ItemId)._type == (byte)EquipDress.Weapon)
                                otherWeaponId = equips[i].ItemId;
                        }
                    }
                    S2C_OtherRoleInfo S2C_roleInfo = new S2C_OtherRoleInfo(role.Guid,
                        (int)mainRole.RoleType,role.Profession,role.SchoolId,role.Name,
                        role.Leader,isBattle,role.Level,role.MoveSpeed,role.Power,otherWeaponId,role.Position);
                    otherRoles.Add(S2C_roleInfo);
                }
            }
            //Console.WriteLine("其他玩家数:" + otherRoles.Count);
            //向本客户端发送创建所有其他主角消息
            ServerSys.Instance.Send(player, "CreateAllOtherRoles", otherRoles);
            
        }

        //初始化宠物
        void InitPetRole(Player player)
        {
            if (DBSys.Instance.GetAllDatas<PetRoleData>("account",player.playerID).Count < 1)
            {
                Console.WriteLine($"{player.account}该账号下没有宠物");
                return;
            }
            List<PetRoleData> petsData = DBSys.Instance.GetAllDatas<PetRoleData>("account", player.account);
            foreach(PetRoleData petData in petsData)
            {
                //给每只宠物赋值，并向客户端发送宠物信息，并初始化宠物信息
                PetRole pet = new PetRole(Guid.NewGuid().ToString(),RoleEnum.Pet,petData.profession);
                AddPetRole(player, pet);
                
            }
        }

        //初始化伙伴
        void InitPartnerRole(Player player)
        {
            if (DBSys.Instance.GetAllDatas<PartnerRoleData>("account", player.account).Count < 1)
            {
                Console.WriteLine($"{player.account}该账号下没有伙伴");
                return;
            }
            List<PartnerRoleData> petsData = DBSys.Instance.GetAllDatas<PartnerRoleData>("account", player.account);
            foreach (PartnerRoleData partnerData in petsData)
            {
                //给每个伙伴赋值，并向客户端发送伙伴信息，并初始化伙伴信息
                PartnerRole partner = new PartnerRole(Guid.NewGuid().ToString(),RoleEnum.Partner,partnerData.profession);
                AddPartnerRole(player, partner);
                
            }
        }

        /// <summary>
        /// 增加主角
        /// </summary>
        /// <param name="player"></param>
        /// <param name="mainRole"></param>
        void AddMainRole(Player player,MainRole mainRole)
        {
            //将角色加入字典
            if (!rolesDic.TryGetValue(player, out Dictionary<int, Dictionary<string, RoleBase>> Roles))
            {
                rolesDic.Add(player, new Dictionary<int, Dictionary<string, RoleBase>>());
            }
            if (!rolesDic[player].TryGetValue((int)RoleEnum.MainRole, out Dictionary<string, RoleBase> mainRoles))
            {
                rolesDic[player].Add((int)RoleEnum.MainRole, new Dictionary<string, RoleBase>());
            }
            //if (!rolesDic[player][(int)RoleEnum.MainRole].TryGetValue(mainRole.Guid, out RoleBase role))
            //{
            //    Console.WriteLine($"{mainRole.Guid}该id对应得主角已存在，操作不合法");
            //    return;
            //}
            if (rolesDic[player][(int)RoleEnum.MainRole].Count > 0)
            {
                Console.WriteLine($"{player.account}该账号已存在主角，操作不合法");
                return;
            }
            rolesDic[player][(int)RoleEnum.MainRole].Add(mainRole.Guid, mainRole);
        }

        /// <summary>
        /// 增加宠物
        /// </summary>
        /// <param name="player"></param>
        /// <param name="pet"></param>
        void AddPetRole(Player player,PetRole pet)
        {
            //将宠物加入字典
            if (!rolesDic.TryGetValue(player, out Dictionary<int, Dictionary<string, RoleBase>> roles))
            {
                rolesDic.Add(player, new Dictionary<int, Dictionary<string, RoleBase>>());
            }
            if (!rolesDic[player].TryGetValue((int)RoleEnum.Pet, out Dictionary<string, RoleBase> petRoles))
            {
                rolesDic[player].Add((int)RoleEnum.Pet, new Dictionary<string, RoleBase>());
            }
            if (!rolesDic[player][(int)RoleEnum.Pet].TryGetValue(pet.Guid, out RoleBase petRole))
            {
                Console.WriteLine($"{pet.Guid}该id对应得宠物已存在，操作不合法");
                return;
            }
            rolesDic[player][(int)RoleEnum.Pet].Add(pet.Guid, pet);
        }

        /// <summary>
        /// 增加伙伴
        /// </summary>
        /// <param name="player"></param>
        /// <param name="partner"></param>
        void AddPartnerRole(Player player,PartnerRole partner)
        {
            //将伙伴加入字典
            if (!rolesDic.TryGetValue(player, out Dictionary<int, Dictionary<string, RoleBase>> roles))
            {
                rolesDic.Add(player, new Dictionary<int, Dictionary<string, RoleBase>>());
            }
            if (!rolesDic[player].TryGetValue((int)RoleEnum.Partner, out Dictionary<string, RoleBase> rolesList))
            {
                rolesDic[player].Add((int)RoleEnum.Partner, new Dictionary<string, RoleBase>());
            }
            if (!rolesDic[player][(int)RoleEnum.Partner].TryGetValue(partner.Guid, out RoleBase partnerRole))
            {
                Console.WriteLine($"{partner.Guid}该id对应得伙伴已存在，操作不合法");
                return;
            }
            rolesDic[player][(int)RoleEnum.Partner].Add(partner.Guid, partner);
        }

        /// <summary>
        /// 删除宠物
        /// </summary>
        /// <param name="player"></param>
        /// <param name="Guid"></param>
        void RemovePetRole(Player player,string Guid)
        {
            if(!rolesDic.TryGetValue(player,out Dictionary<int,Dictionary<string ,RoleBase>> roles))
            {
                Console.WriteLine($"{player.account}该账号下没有角色");
                return;
            }
            if(!rolesDic[player].TryGetValue((int)RoleEnum.Pet,out Dictionary<string,RoleBase> petRoles))
            {
                Console.WriteLine($"{player.account}该账号下不存在宠物");
                return;
            }
            if(!rolesDic[player][(int)RoleEnum.Pet].TryGetValue(Guid,out RoleBase pet))
            {
                Console.WriteLine($"{player.account}该账号下不存在{Guid}该id宠物");
                return;
            }
            rolesDic[player][(int)RoleEnum.Pet].Remove(Guid);
        }

        /// <summary>
        /// 删除伙伴
        /// </summary>
        /// <param name="player"></param>
        /// <param name="Guid"></param>
        void RemovePartnerRole(Player player, string Guid)
        {
            if (!rolesDic.TryGetValue(player, out Dictionary<int, Dictionary<string, RoleBase>> roles))
            {
                Console.WriteLine($"{player.account}该账号下没有角色");
                return;
            }
            if (!rolesDic[player].TryGetValue((int)RoleEnum.Partner, out Dictionary<string, RoleBase> partnerRoles))
            {
                Console.WriteLine($"{player.account}该账号下不存在伙伴");
                return;
            }
            if (!rolesDic[player][(int)RoleEnum.Partner].TryGetValue(Guid, out RoleBase partner))
            {
                Console.WriteLine($"{player.account}该账号下不存在{Guid}该id伙伴");
                return;
            }
            rolesDic[player][(int)RoleEnum.Partner].Remove(Guid);
        }

        /// <summary>
        /// 获取主角
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public MainRole GetMainRole(Player player)
        {
            if (!rolesDic.TryGetValue(player, out Dictionary<int, Dictionary<string, RoleBase>> roles))
            {
                Console.WriteLine($"{player.account}该账号下没有角色");
                return null;
            }
            if (!rolesDic[player].TryGetValue((int)RoleEnum.MainRole, out Dictionary<string, RoleBase> mainRoles))
            {
                Console.WriteLine($"{player.account}该账号下不存在主角");
                return null;
            }
            foreach(RoleBase role in rolesDic[player][(int)RoleEnum.MainRole].Values)
            {
                return role as MainRole;
            }
            Console.WriteLine($"{player.account}该账号下不存在主角");
            return null;
        }

        /// <summary>
        /// 向客户端更新主角信息
        /// </summary>
        /// <param name="player"></param>
        public void UpdatePlayerMainRole(Player player)
        {
            MainRole mainRole = player.mainRole;

            int weaponId = -1;
            if (player.mainRole.Inventory.GetInventoryItems((byte)InventoryEnum.Equip) != null)
            {
                ItemBase[] equips = player.mainRole.Inventory.GetInventoryItems((byte)InventoryEnum.Equip);
                for (int i = 0; i < equips.Length; i++)
                {
                    if (ConfigSys.Instance.GetConfig<EquipConfig>(equips[i].ItemId)._type == (byte)EquipDress.Weapon)
                        weaponId = equips[i].ItemId;
                }
            }
            S2C_MainRoleInfo S2C_mainRoleInfo = new S2C_MainRoleInfo(mainRole.Guid, (int)mainRole.RoleType,
                mainRole.Profession, mainRole.SchoolId, mainRole.Name, false, false,
                mainRole.Level, mainRole.MoveSpeed,mainRole.Power,weaponId, mainRole.Position, mainRole.Hp,
                mainRole.MaxHp, mainRole.Mp, mainRole.MaxMp, mainRole.PhysicalAttack,
                mainRole.PhysicalDefense, mainRole.MagicAttack, mainRole.MagicDefense,
                mainRole.Speed, mainRole.Experience, mainRole.Silver, mainRole.Gold, mainRole.YuanBao);

            ServerSys.Instance.Send(player, "CallLuaFunction", "MainUICtrl.UpdateMainRole",
                S2C_mainRoleInfo);
        }

        /// <summary>
        /// 获取宠物
        /// </summary>
        /// <param name="player"></param>
        /// <param name="Guid"></param>
        /// <returns></returns>
        public PetRole GetPetRole(Player player,string Guid)
        {
            if (!rolesDic.TryGetValue(player, out Dictionary<int, Dictionary<string, RoleBase>> roles))
            {
                Console.WriteLine($"{player.account}该账号下没有角色");
                return null;
            }
            if (!rolesDic[player].TryGetValue((int)RoleEnum.Pet, out Dictionary<string, RoleBase> petRoles))
            {
                Console.WriteLine($"{player.account}该账号下不存在宠物");
                return null;
            }
            if (!rolesDic[player][(int)RoleEnum.Pet].TryGetValue(Guid, out RoleBase pet))
            {
                Console.WriteLine($"{player.account}该账号下不存在{Guid}该id宠物");
                return null;
            }
            return pet as PetRole;
        }

        /// <summary>
        /// 获取伙伴
        /// </summary>
        /// <param name="player"></param>
        /// <param name="Guid"></param>
        /// <returns></returns>
        public PartnerRole GetPartnerRole(Player player,string Guid)
        {
            if (!rolesDic.TryGetValue(player, out Dictionary<int, Dictionary<string, RoleBase>> roles))
            {
                Console.WriteLine($"{player.account}该账号下没有角色");
                return null;
            }
            if (!rolesDic[player].TryGetValue((int)RoleEnum.Partner, out Dictionary<string, RoleBase> partnerRoles))
            {
                Console.WriteLine($"{player.account}该账号下不存在伙伴");
                return null;
            }
            if (!rolesDic[player][(int)RoleEnum.Partner].TryGetValue(Guid, out RoleBase partner))
            {
                Console.WriteLine($"{player.account}该账号下不存在{Guid}该id伙伴");
                return null;
            }
            return partner as PartnerRole;
        }

        public void CheckInfo(Player player,C2S_CheckInfo C2S_checkInfo)
        {
            S2C_CheckInfo info = new S2C_CheckInfo();
            info.Guid = C2S_checkInfo.Guid;
            if (!ServerSys.Instance.Players.ContainsKey(info.Guid))
                return;
            info.equips = new List<S2C_AddEquipInfo>();
            Player target = ServerSys.Instance.Players[info.Guid];
            if(target.mainRole.Inventory.GetInventoryItems((byte)ItemEunm.Equip) != null)
            {
                ItemBase[] equips = target.mainRole.Inventory.GetInventoryItems((byte)ItemEunm.Equip);
                for (int i = 0;i < equips.Length; i++)
                {
                    EquipInfo equip = equips[i] as EquipInfo;
                    info.equips.Add(new S2C_AddEquipInfo(equip.Guid, equip.ItemId, (int)equip.ItemType,
                        equip.Inventory, equip.gems));
                }
            }
            ServerSys.Instance.Send(player, "CallLuaFunction", "RoleCtrl.ShowInfo", info);
        }

        /// <summary>
        /// 保存主角数据
        /// </summary>
        /// <param name="player"></param>
        public void SaveRoleData(Player player)
        {
            DBSys.Instance.DeleteAllDatas<MainRoleData>("account", player.account);
            DBSys.Instance.InsertData(new MainRoleData(
                player.mainRole.Guid, player.account, player.mainRole.Name,player.mainRole.Profession, player.mainRole.Level,
                player.mainRole.Hp, player.mainRole.MaxHp, player.mainRole.Mp, player.mainRole.MaxMp,
                player.mainRole.PhysicalAttack, player.mainRole.PhysicalDefense, player.mainRole.MagicAttack,
                player.mainRole.MagicDefense, player.mainRole.Speed, player.mainRole.Experience,
                player.mainRole.Poisoning, player.mainRole.PoisoningResist, player.mainRole.Burn,
                player.mainRole.BurnResist, player.mainRole.ContinueHit, player.mainRole.StrikeBack,
                player.mainRole.Profession, player.mainRole.SchoolId, player.mainRole.MoveSpeed,
                player.mainRole.Position.x, player.mainRole.Position.y, player.mainRole.Position.z,
                player.mainRole.Silver, player.mainRole.Gold, player.mainRole.YuanBao, player.mainRole.Power));
        }
    }
}
