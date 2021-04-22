/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:属性系统
 *          
 *          description:
 *              功能描述:提供修改属性的接口
 *              
 *          author:
 *              作者:照着教程敲出bug的程序员
 * 
 * ===================================================================
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.GameSys
{

    public enum AttrEnum
    {
        Hp = 90,
        Mp = 122,
        PhysicalAttack = 69,
        MagicAttack = 143,
        PhysicalDefense = 102,
        MagicDefense = 112,
        Speed = 79
    }
    class AttrSys
    {
        private static AttrSys instance;
        public static AttrSys Instance
        {
            get
            {
                if (instance == null)
                    instance = new AttrSys();
                return instance;
            }
        }

        /// <summary>
        /// 添加装备属性
        /// </summary>
        /// <param name="player"></param>
        /// <param name="equipId"></param>
        public void AddEquipAttr(Player player, int equipId)
        {

            EquipConfig equipConfig = ConfigSys.Instance.GetConfig<EquipConfig>(equipId);
            if (equipConfig.firstAttr == 0)
                return;
            AddAttr(player, equipConfig.firstAttr, equipConfig.firstAttrValue);
            if (equipConfig.secondAttr == 0)
                return;
            AddAttr(player, equipConfig.secondAttr, equipConfig.secondAttrValue);
            if (equipConfig.thirdAttr == 0)
                return;
            AddAttr(player, equipConfig.thirdAttr, equipConfig.thirdAttrValue);

        }

        /// <summary>
        /// 添加宝石属性
        /// </summary>
        /// <param name="player"></param>
        /// <param name="gemId"></param>
        public void AddGemAttr(Player player,int gemId)
        {
            GemConfig gemConfig = ConfigSys.Instance.GetConfig<GemConfig>(gemId);
            if (gemConfig.attr == 0)
                return;
            AddAttr(player, gemConfig.attr, gemConfig.attrValue);
        }

        /// <summary>
        /// 移除宝石属性
        /// </summary>
        /// <param name="player"></param>
        /// <param name="gemId"></param>
        public void ReduceGemAttr(Player player, int gemId)
        {
            GemConfig gemConfig = ConfigSys.Instance.GetConfig<GemConfig>(gemId);
            if (gemConfig.attr == 0)
                return;
            ReduceAttr(player, gemConfig.attr, gemConfig.attrValue);
        }


        /// <summary>
        /// 移除装备属性
        /// </summary>
        /// <param name="player"></param>
        /// <param name="equipId"></param>
        public void ReduceEquipAttr(Player player, int equipId)
        {
            EquipConfig equipConfig = ConfigSys.Instance.GetConfig<EquipConfig>(equipId);
            if (equipConfig.firstAttr == 0)
                return;
            ReduceAttr(player, equipConfig.firstAttr, equipConfig.firstAttrValue);
            if (equipConfig.secondAttr == 0)
                return;
            ReduceAttr(player, equipConfig.secondAttr, equipConfig.secondAttrValue);
            if (equipConfig.thirdAttr == 0)
                return;
            ReduceAttr(player, equipConfig.thirdAttr, equipConfig.thirdAttrValue);
        }

        /// <summary>
        /// 添加属性
        /// </summary>
        /// <param name="player"></param>
        /// <param name="attrId"></param>
        /// <param name="attrValue"></param>
        void AddAttr(Player player, int attrId, int attrValue)
        {
            switch (attrId)
            {
                case (int)AttrEnum.Hp:
                    player.mainRole.AddMaxHp(attrValue);
                    break;
                case (int)AttrEnum.Mp:
                    player.mainRole.AddMaxMp(attrValue);
                    break;
                case (int)AttrEnum.PhysicalAttack:
                    player.mainRole.AddPhysicalAttack(attrValue);
                    break;
                case (int)AttrEnum.PhysicalDefense:
                    player.mainRole.AddPhysicalDefense(attrValue);
                    break;
                case (int)AttrEnum.MagicAttack:
                    player.mainRole.AddMagicAttack(attrValue);
                    break;
                case (int)AttrEnum.MagicDefense:
                    player.mainRole.AddMagicDefense(attrValue);
                    break;
                case (int)AttrEnum.Speed:
                    player.mainRole.AddSpeed(attrValue);
                    break;
            }
        }

        /// <summary>
        /// 移除属性
        /// </summary>
        /// <param name="player"></param>
        /// <param name="attrId"></param>
        /// <param name="attrValue"></param>
        void ReduceAttr(Player player, int attrId, int attrValue)
        {
            switch (attrId)
            {
                case (int)AttrEnum.Hp:
                    player.mainRole.ReduceMaxHp(attrValue);
                    break;
                case (int)AttrEnum.Mp:
                    player.mainRole.ReduceMaxMp(attrValue);
                    break;
                case (int)AttrEnum.PhysicalAttack:
                    player.mainRole.ReducePhysicalAttack(attrValue);
                    break;
                case (int)AttrEnum.PhysicalDefense:
                    player.mainRole.ReducePhysicalDefense(attrValue);
                    break;
                case (int)AttrEnum.MagicAttack:
                    player.mainRole.ReduceMagicAttack(attrValue);
                    break;
                case (int)AttrEnum.MagicDefense:
                    player.mainRole.ReduceMagicDefense(attrValue);
                    break;
                case (int)AttrEnum.Speed:
                    player.mainRole.ReduceSpeed(attrValue);
                    break;
            }
        }

    }
}
