/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:角色信息基类
 *          
 *          description:
 *              功能描述:设计角色基类信息
 *              
 *          author:
 *              作者:
 * 
 * ===================================================================
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class RoleBase
    {
        public RoleBase(string guid, RoleEnum roleType,int profession)
        {
            this.guid = guid;
            this.roleType = roleType;
            this.profession = profession;
        }
        private string guid;  //唯一标识
        private RoleEnum roleType;  //角色类型
        private int profession;  //职业类型(模型Id)
        private string name;  //角色名称
        private int level;  //角色等级
        private int hp;  //气血
        private int maxHp;  //气血上限
        private int mp;  //元气
        private int maxMp;  //元气上限
        private int physicalAttack;  //物理攻击
        private int physicalDefense;  //物理防御
        private int magicAttack;  //元气攻击
        private int magicDefense;  //元气防御
        private int speed;  //速度
        private int experience;  //经验
        private float frozen;  //冰冻命中率
        private float frozenResist;  //冰冻抗性
        private float poisoning;  //中毒命中率
        private float poisoningResist;  //中毒抗性
        private float burn;  //烧伤命中率
        private float burnResist;  //烧伤抗性
        private float continueHit;  //连击率
        private float strikeBack;  //反击率

        public string Guid { get { return guid; } }
        public RoleEnum RoleType { get { return roleType; } }
        public int Profession { get { return profession; } }
        public string Name { get { return name; } }
        public int Level { get { return level; } }
        public int Hp { get { return hp; } }
        public int MaxHp { get { return maxHp; } }
        public int Mp { get { return mp; } }
        public int MaxMp { get { return maxMp; } }
        public int PhysicalAttack {get { return physicalAttack; } }
        public int PhysicalDefense { get { return physicalDefense; } }
        public int MagicAttack { get { return magicAttack; } }
        public int MagicDefense { get { return magicDefense; } }
        public int Speed { get { return speed; } }
        public int Experience { get { return experience; } }
        public float Frozen { get { return frozen; } }
        public float FrozenResist { get { return frozenResist; } }
        public float Poisoning { get { return poisoning; } }
        public float PoisoningResist { get { return poisoningResist; } }
        public float Burn { get { return burn; } }
        public float BurnResist { get { return burnResist; } }
        public float ContinueHit { get { return continueHit; } }
        public float StrikeBack { get { return strikeBack; } }

        /// <summary>
        /// 修改名字
        /// </summary>
        /// <param name="name"></param>
        public void ChangeName(string name)
        {
            this.name = name;
        }
        /// <summary>
        /// 提升等级
        /// </summary>
        /// <param name="offest"></param>
        public void UpLevel(int offest)
        {
            level += offest;
        }
        /// <summary>
        /// 修改等级
        /// </summary>
        /// <param name="level"></param>
        public void ChangeLevel(int level)
        {
            this.level = level;
        }
        /// <summary>
        /// 减少血量
        /// </summary>
        /// <param name="offest"></param>
        public void ReduceHp(int offest)
        {
            hp -= offest;
        }
        /// <summary>
        /// 增加血量
        /// </summary>
        /// <param name="offest"></param>
        public void AddHp(int offest)
        {
            hp += offest;
        }
        /// <summary>
        /// 修改血量
        /// </summary>
        /// <param name="hp"></param>
        public void ChangeHp(int hp)
        {
            this.hp = hp;
        }
        /// <summary>
        /// 减少生命上限
        /// </summary>
        /// <param name="offest"></param>
        public void ReduceMaxHp(int offest)
        {
            maxHp -= offest;
        }
        /// <summary>
        /// 增加生命上限
        /// </summary>
        /// <param name="offest"></param>
        public void AddMaxHp(int offest)
        {
            maxHp += offest;
        }
        /// <summary>
        /// 修改生命上限
        /// </summary>
        /// <param name="maxHp"></param>
        public void ChangeMaxHp(int maxHp)
        {
            this.maxHp = maxHp;
        }
        /// <summary>
        /// 减少元气值
        /// </summary>
        /// <param name="offest"></param>
        public void ReduceMp(int offest)
        {
            mp -= offest;
        }
        /// <summary>
        /// 增加元气值
        /// </summary>
        /// <param name="offest"></param>
        public void AddMp(int offest)
        {
            mp += offest;
        }
        /// <summary>
        /// 修改元气值
        /// </summary>
        /// <param name="mp"></param>
        public void ChangeMp(int mp)
        {
            this.mp = mp;
        }
        /// <summary>
        /// 减少元气值上限
        /// </summary>
        /// <param name="offest"></param>
        public void ReduceMaxMp(int offest)
        {
            maxMp -= offest;
        }
        /// <summary>
        /// 增加元气值上限
        /// </summary>
        /// <param name="offest"></param>
        public void AddMaxMp(int offest){
            maxMp += offest;
        }
        /// <summary>
        /// 修改元气值上限
        /// </summary>
        /// <param name="maxMp"></param>
        public void ChangeMaxMp(int maxMp)
        {
            this.maxMp = maxMp;
        }

        /// <summary>
        /// 降低物理攻击
        /// </summary>
        /// <param name="offest"></param>
        public void ReducePhysicalAttack(int offest)
        {
            physicalAttack -= offest;
        }

        /// <summary>
        /// 提升物理攻击
        /// </summary>
        /// <param name="offest"></param>
        public void AddPhysicalAttack(int offest)
        {
            physicalAttack += offest;
        }

        /// <summary>
        /// 修改物理攻击
        /// </summary>
        /// <param name="physicalAttack"></param>
        public void ChangePhysicalAttack(int physicalAttack)
        {
            this.physicalAttack = physicalAttack;
        }

        /// <summary>
        /// 降低物理防御
        /// </summary>
        /// <param name="offest"></param>
        public void ReducePhysicalDefense(int offest)
        {
            physicalDefense -= offest;
        }
        
        /// <summary>
        /// 提升物理防御
        /// </summary>
        /// <param name="offest"></param>
        public void AddPhysicalDefense(int offest)
        {
            physicalDefense += offest;
        }

        /// <summary>
        /// 修改物理防御
        /// </summary>
        /// <param name="physicalDefense"></param>
        public void ChangePhysicalDefense(int physicalDefense)
        {
            this.physicalDefense = physicalDefense;
        }

        /// <summary>
        /// 降低元气攻击
        /// </summary>
        /// <param name="offest"></param>
        public void ReduceMagicAttack(int offest)
        {
            magicAttack -= offest;
        }

        /// <summary>
        /// 提升元气攻击
        /// </summary>
        /// <param name="offest"></param>
        public void AddMagicAttack(int offest)
        {
            magicAttack += offest;
        }

        /// <summary>
        /// 修改元气攻击
        /// </summary>
        /// <param name="magicAttack"></param>
        public void ChangeMagicAttack(int magicAttack)
        {
            this.magicAttack = magicAttack;
        }

        /// <summary>
        /// 降低元气防御
        /// </summary>
        /// <param name="offest"></param>
        public void ReduceMagicDefense(int offest)
        {
            magicDefense -= offest;
        }

        /// <summary>
        /// 提升元气防御
        /// </summary>
        /// <param name="offest"></param>
        public void AddMagicDefense(int offest)
        {
            magicDefense += offest;
        }

        /// <summary>
        /// 修改元气防御
        /// </summary>
        /// <param name="magicDefense"></param>
        public void ChangeMagicDefense(int magicDefense)
        {
            this.magicDefense = magicDefense;
        }

        /// <summary>
        /// 降低速度
        /// </summary>
        /// <param name="offest"></param>
        public void ReduceSpeed(int offest)
        {
            speed -= offest;
        }

        /// <summary>
        /// 提升速度
        /// </summary>
        /// <param name="offest"></param>
        public void AddSpeed(int offest)
        {
            speed += offest;
        }

        /// <summary>
        /// 修改速度
        /// </summary>
        /// <param name="speed"></param>
        public void ChangeSpeed(int speed)
        {
            this.speed = speed;
        }

        /// <summary>
        /// 减少经验
        /// </summary>
        /// <param name="offest"></param>
        public void ReduceExperience(int offest)
        {
            experience -= offest;
        }

        /// <summary>
        /// 增加经验
        /// </summary>
        /// <param name="offest"></param>
        public void AddExperience(int offest)
        {
            experience += offest;
            int maxLevel = ConfigSys.Instance.GetConfigLength("ExperienceConfig");
            if (level >= maxLevel)
                return;
            while(experience >= ConfigSys.Instance.GetConfig<ExperienceConfig>(level + 1).experience){
                experience -= ConfigSys.Instance.GetConfig<ExperienceConfig>(level + 1).experience;
                level++;
            }
        }

        /// <summary>
        /// 修改经验
        /// </summary>
        /// <param name="experience"></param>
        public void ChangeExperience(int experience)
        {
            this.experience = experience;
        }

        /// <summary>
        /// 降低冰冻命中
        /// </summary>
        /// <param name="offest"></param>
        public void ReduceFrozen(float offest)
        {
            frozen -= offest;
        }

        /// <summary>
        /// 提升冰冻命中
        /// </summary>
        /// <param name="offest"></param>
        public void AddFrozen(float offest)
        {
            frozen += offest;
        }

        /// <summary>
        /// 修改冰冻命中
        /// </summary>
        /// <param name="frozen"></param>
        public void ChangeFrozen(float frozen)
        {
            this.frozen = frozen;
        }

        /// <summary>
        /// 降低冰冻抗性
        /// </summary>
        /// <param name="offest"></param>
        public void ReduceFrozenResist(float offest)
        {
            frozenResist -= offest;
        }

        /// <summary>
        /// 提升冰冻抗性
        /// </summary>
        /// <param name="offest"></param>
        public void AddFrozenResist(float offest)
        {
            frozenResist += offest;
        }

        /// <summary>
        /// 修改冰冻抗性
        /// </summary>
        /// <param name="frozenResist"></param>
        public void ChangeFrozenResist(float frozenResist)
        {
            this.frozenResist = frozenResist;
        }

        /// <summary>
        /// 降低中毒命中
        /// </summary>
        /// <param name="offest"></param>
        public void ReducePoisoning(float offest)
        {
            poisoning -= offest;
        }

        /// <summary>
        /// 提升中毒命中
        /// </summary>
        /// <param name="offest"></param>
        public void AddPoisoning(float offest)
        {
            poisoning += offest;
        }

        /// <summary>
        /// 修改中毒命中
        /// </summary>
        /// <param name="poisoning"></param>
        public void ChangePoisoning(float poisoning)
        {
            this.poisoning = poisoning;
        }

        /// <summary>
        /// 降低中毒抗性
        /// </summary>
        /// <param name="offest"></param>
        public void ReducePoisoningResist(float offest)
        {
            poisoningResist -= offest;
        }

        /// <summary>
        /// 提升中毒抗性
        /// </summary>
        /// <param name="offest"></param>
        public void AddPoisoningResist(float offest)
        {
            poisoningResist += offest;
        }

        /// <summary>
        /// 修改中毒抗性
        /// </summary>
        /// <param name="poisoningResist"></param>
        public void ChangePoisoningResist(float poisoningResist)
        {
            this.poisoningResist = poisoningResist;
        }

        /// <summary>
        /// 降低烧伤命中
        /// </summary>
        /// <param name="offest"></param>
        public void ReduceBurn(float offest)
        {
            burn -= offest;
        }

        /// <summary>
        /// 提升烧伤命中
        /// </summary>
        /// <param name="offest"></param>
        public void AddBurn(float offest)
        {
            burn += offest;
        }

        /// <summary>
        /// 修改烧伤命中
        /// </summary>
        /// <param name="burn"></param>
        public void ChangeBurn(float burn)
        {
            this.burn = burn;
        }

        /// <summary>
        /// 降低烧伤抗性
        /// </summary>
        /// <param name="offest"></param>
        public void ReduceBurnResist(float offest)
        {
            burnResist -= offest;
        }

        /// <summary>
        /// 提升烧伤抗性
        /// </summary>
        /// <param name="offest"></param>
        public void AddBurnResist(float offest)
        {
            burnResist += offest;
        }

        /// <summary>
        /// 修改烧伤抗性
        /// </summary>
        /// <param name="burnResist"></param>
        public void ChangeBurnResist(float burnResist)
        {
            this.burnResist = burnResist;
        }

        /// <summary>
        /// 降低连击率
        /// </summary>
        /// <param name="offest"></param>
        public void ReduceContinueHit(float offest)
        {
            continueHit -= offest;
        }

        /// <summary>
        /// 提升连击率
        /// </summary>
        /// <param name="offest"></param>
        public void AddContinueHit(float offest)
        {
            continueHit += offest;
        }

        /// <summary>
        /// 修改连击率
        /// </summary>
        /// <param name="continueHit"></param>
        public void ChangeContinueHit(float continueHit)
        {
            this.continueHit = continueHit;
        }

        /// <summary>
        /// 降低反击率
        /// </summary>
        /// <param name="offest"></param>
        public void ReduceStrikeBack(float offest)
        {
            strikeBack -= offest;
        }

        /// <summary>
        /// 提升反击率
        /// </summary>
        /// <param name="offest"></param>
        public void AddStrikeBack(float offest)
        {
            strikeBack += offest;
        }

        /// <summary>
        /// 修改反击率
        /// </summary>
        /// <param name="strikeBack"></param>
        public void ChangeStrikeBack(float strikeBack)
        {
            this.strikeBack = strikeBack;
        }

        public int Power { get { return maxHp * 10 + maxMp * 10 + physicalAttack * 10 + 
                    physicalDefense * 10 + magicAttack * 10 + magicDefense * 10 + 
                    speed * 10; } }

    }
}
