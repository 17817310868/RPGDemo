/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:技能信息类
 *          
 *          description:
 *              功能描述:设计每个技能具体信息
 *              
 *          author:
 *              作者:照着教程敲出bug的程序员
 * 
 * ===================================================================
 */

using MongoDB.Driver.Linq;
using Server;
using Server.Buff;
using Server.GameSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Skill
{

    /// <summary>
    /// 技能基类
    /// </summary>
    public class SkillBase
    {
        public SkillBase(int skillId)
        {
            this.skillId = skillId;
        }

        private int skillId;
        public int SkillId { get { return skillId; } }  //配置表中的技能Id

        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="skillId"></param>
        /// <param name="battle"></param>
        /// <param name="actor"></param>
        /// <param name="victims"></param>
        /// <param name="S2C_battleMessage"></param>
        public virtual void Release(Battle battle, Battler actor, Battler[] victims, ref S2C_BattleMessage S2C_battleMessage)
        {

        }

    }

    /// <summary>
    /// 反击
    /// </summary>
    public class StrikeBack : SkillBase
    {
        public StrikeBack(int skillId) :base(skillId)
        {

        }

        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="skillId">技能id</param>
        /// <param name="battle">所属战局</param>
        /// <param name="actor">行动者站位索引</param>
        /// <param name="victims">受害者站位索引</param>
        /// <param name="S2C_battleMessage">战斗消息</param>
        public override void Release(Battle battle, Battler actor, Battler[] victims, ref S2C_BattleMessage S2C_battleMessage)
        {
            float attackerStrikeBack = actor.Role.StrikeBack;
            //当行动者反击率返回true时，进行反击
            if (attackerStrikeBack >= Tools._Random.NextDouble())
            {
                SkillSys.Instance.Release(1000, battle, actor, victims, ref S2C_battleMessage);
            }
        }
    }


    /// <summary>
    /// 普通攻击
    /// </summary>
    public class NormalAttack : SkillBase
    {
        public NormalAttack(int skillId) : base(skillId)
        {

        }

        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="skillId">技能id</param>
        /// <param name="battle">所属战局</param>
        /// <param name="actor">行动者站位索引</param>
        /// <param name="victims">受害者站位索引</param>
        /// <param name="S2C_battleMessage">战斗消息</param>
        public override void Release(Battle battle, Battler actor, Battler[] victims, ref S2C_BattleMessage S2C_battleMessage)
        {

            //更新战斗消息中的行动者站位索引和受害者站位索引
            //S2C_battleMessage.actorIndex = actor.PositionIndex;
            //S2C_battleMessage.victims = new byte[] {victims[0].PositionIndex };

            //获取受害者的参战者数据
            //Battler injure = battle.roles[victims[0]];
            
            do
            {
                //计算最终伤害结果
                int result = actor.physicalAttack - victims[0].physicalDefense;
                if (result < 0)
                {
                    result = 0;
                }
                //创建一条新的战斗消息，用于存放至战斗消息的连击反击列表中
                S2C_BattleMessage S2C_message = new S2C_BattleMessage();
                //更新新的战斗消息
                S2C_message.actorIndex = actor.PositionIndex;
                S2C_message.victims = new byte[] { victims[0].PositionIndex };
                S2C_message.actorType = (byte)ActorType.Attack;
                S2C_message.targetsHpEffect.Add(victims[0].PositionIndex, -result);
                //将新的战斗消息添加至战斗消息的其他消息列表
                S2C_battleMessage.otherMessages.Add(S2C_message);


                //更新角色hp
                if (victims[0].Role.Hp > result)
                {
                    victims[0].Role.ReduceHp(result);
                }
                else
                {
                    victims[0].Role.ChangeHp(1);
                    battle.BattlerDead(ref victims[0]);
                    break;
                }
            }
            //当连击判断返回true，则继续攻击
            while (actor.Role.ContinueHit >= Tools._Random.NextDouble());
            //如果目标未死亡，则执行反击方法
            if(!battle.IsDead(victims[0].PositionIndex))
                SkillSys.Instance.Release(1001, battle, victims[0],new Battler[] { actor },
                    ref S2C_battleMessage);
        }
    }


    /// <summary>
    /// 威震凌霄
    /// </summary>
    public class WeiZhenLingXiao:SkillBase
    {
        public WeiZhenLingXiao(int skillId) : base(skillId)
        {
            
        }

        /// <summary>
        /// 释放技能
        /// </summary>
        /// <param name="skillId">技能id</param>
        /// <param name="battle">所属战局</param>
        /// <param name="actor">行动者站位索引</param>
        /// <param name="victims">受害者站位索引</param>
        /// <param name="S2C_battleMessage">战斗消息</param>
        public override void Release(Battle battle, Battler actor, Battler[] victims, ref S2C_BattleMessage S2C_battleMessage)
        {

            //更新战斗消息中的行动者站位索引和受害者站位索引
            S2C_battleMessage.actorIndex = actor.PositionIndex;
            byte[] victimsIndex = new byte[victims.Length];
            for(int i = 0; i< victimsIndex.Length;i++)
            {
                victimsIndex[i] = victims[i].PositionIndex;
            }
            S2C_battleMessage.victims = victimsIndex;


            //更新行动者所施展的技能所消耗的mp，并更新角色mp
            //int mpConsume = ConfigSys.Instance.GetConfig<SkillConfig>(SkillId).consume;
            //actor.Role.ReduceMp(mpConsume);


            //遍历受害者站位索引数组
            for (byte j = 0; j < victims.Length; j++)
            {
                //获取受害者参战数据
                Battler injured = victims[j];
                
                //得出最终伤害
                int result = actor.physicalAttack - injured.physicalDefense;
                if (result < 0)
                {
                    result = 0;
                }

                battle.AddActionBuff(new Poisoning(2006, -5, 3),ref injured);
                S2C_battleMessage.actionBuffs.Add(injured.PositionIndex, 2006);
                S2C_battleMessage.actionBuffsEffect.Add(injured.PositionIndex, -5);
                S2C_battleMessage.targetsHpEffect.Add(injured.PositionIndex, -result);
                //更新角色hp及状态
                if (injured.Role.Hp > result)
                {
                    injured.Role.ReduceHp(result);
                }
                else
                {
                    injured.Role.ChangeHp(1);
                    battle.BattlerDead(ref injured);
                }
                
            }

            //如果目标未死亡，则执行反击方法
            if (!battle.IsDead(victims[0].PositionIndex))
                SkillSys.Instance.Release(1001, battle, victims[0], new Battler[] { actor },
                    ref S2C_battleMessage);
        }
    }

    public class PaiShanDaoHai : SkillBase
    {
        public PaiShanDaoHai(int skillId) : base(skillId)
        {
            
        }

        public override void Release(Battle battle, Battler actor, Battler[] victims, ref S2C_BattleMessage S2C_battleMessage)
        {
            S2C_battleMessage.actorIndex = actor.PositionIndex;
            byte[] victimsIndex = new byte[victims.Length];
            for (int i = 0; i < victimsIndex.Length; i++)
            {
                victimsIndex[i] = victims[i].PositionIndex;
            }
            S2C_battleMessage.victims = victimsIndex;

            SkillConfig config = ConfigSys.Instance.GetConfig<SkillConfig>(SkillId);
            //更新行动者所施展的技能所消耗的mp，并更新角色mp
            //int mpConsume = config.consume;
            //actor.Role.ReduceMp(mpConsume);
            //遍历受害者站位索引数组
            for (byte j = 0; j < victims.Length; j++)
            {
                //获取受害者参战数据
                Battler injured = victims[j];

                //得出最终伤害
                int result = (int)(actor.physicalAttack * config.skillRatio) - injured.physicalDefense;
                if (result < 0)
                {
                    result = 0;
                }

                //battle.AddActionBuff(new Poisoning(2006, -5, 3), ref injured);
                //S2C_battleMessage.actionBuffs.Add(injured.PositionIndex, 2006);
                //S2C_battleMessage.actionBuffsEffect.Add(injured.PositionIndex, -5);
                S2C_battleMessage.targetsHpEffect.Add(injured.PositionIndex, -result);
                //更新角色hp及状态
                if (injured.Role.Hp > result)
                {
                    injured.Role.ReduceHp(result);
                }
                else
                {
                    injured.Role.ChangeHp(1);
                    battle.BattlerDead(ref injured);
                }

                //如果目标未死亡，则执行反击方法
                if (!battle.IsDead(victims[0].PositionIndex))
                    SkillSys.Instance.Release(1001, battle, victims[0], new Battler[] { actor },
                        ref S2C_battleMessage);

            }
        }
    }

    public class HengSaoQianJun : SkillBase
    {
        public HengSaoQianJun(int skillId) : base(skillId)
        {

        }

        public override void Release(Battle battle, Battler actor, Battler[] victims, ref S2C_BattleMessage S2C_battleMessage)
        {
            //更新战斗消息中的行动者站位索引和受害者站位索引
            S2C_battleMessage.actorIndex = actor.PositionIndex;
            byte[] victimsIndex = new byte[victims.Length];
            for (int i = 0; i < victimsIndex.Length; i++)
            {
                victimsIndex[i] = victims[i].PositionIndex;
            }
            S2C_battleMessage.victims = victimsIndex;

            SkillConfig config = ConfigSys.Instance.GetConfig<SkillConfig>(SkillId);
            //更新行动者所施展的技能所消耗的mp，并更新角色mp
            //int mpConsume = config.consume;
            //actor.Role.ReduceMp(mpConsume);


            //遍历受害者站位索引数组
            for (byte j = 0; j < victims.Length; j++)
            {
                //获取受害者参战数据
                Battler injured = victims[j];

                //得出最终伤害
                int result = (int)(actor.physicalAttack * config.skillRatio) - injured.physicalDefense;
                if (result < 0)
                {
                    result = 0; 
                }

                battle.AddActionBuff(new Poisoning(2006, -5, 3), ref injured);
                S2C_battleMessage.actionBuffs.Add(injured.PositionIndex, 2006);
                S2C_battleMessage.actionBuffsEffect.Add(injured.PositionIndex, -5);
                S2C_battleMessage.targetsHpEffect.Add(injured.PositionIndex, -result);
                //更新角色hp及状态
                if (injured.Role.Hp > result)
                {
                    injured.Role.ReduceHp(result);
                }
                else
                {
                    injured.Role.ChangeHp(1);
                    battle.BattlerDead(ref injured);
                }

            }


            //如果目标未死亡，则执行反击方法
            if (!battle.IsDead(victims[0].PositionIndex))
                SkillSys.Instance.Release(1001, battle, victims[0], new Battler[] { actor },
                    ref S2C_battleMessage);
        }
    }

    public class FengHuoLianTian : SkillBase
    {
        public FengHuoLianTian(int skillId) : base(skillId)
        {

        }

        public override void Release(Battle battle, Battler actor, Battler[] victims, ref S2C_BattleMessage S2C_battleMessage)
        {
            //更新战斗消息中的行动者站位索引和受害者站位索引
            S2C_battleMessage.actorIndex = actor.PositionIndex;
            byte[] victimsIndex = new byte[victims.Length];
            for (int i = 0; i < victimsIndex.Length; i++)
            {
                victimsIndex[i] = victims[i].PositionIndex;
            }
            S2C_battleMessage.victims = victimsIndex;

            SkillConfig config = ConfigSys.Instance.GetConfig<SkillConfig>(SkillId);
            //更新行动者所施展的技能所消耗的mp，并更新角色mp
            //int mpConsume = config.consume;
            //actor.Role.ReduceMp(mpConsume);


            //遍历受害者站位索引数组
            for (byte j = 0; j < victims.Length; j++)
            {
                //获取受害者参战数据
                Battler injured = victims[j];

                //得出最终伤害
                int result = (int)(actor.magicAttack * config.skillRatio) - injured.magicDefense;
                if (result < 0)
                {
                    result = 0;
                }

                //battle.AddActionBuff(new Poisoning(2006, -5, 3), ref injured);
                //S2C_battleMessage.actionBuffs.Add(injured.PositionIndex, 2006);
                //S2C_battleMessage.actionBuffsEffect.Add(injured.PositionIndex, -5);
                S2C_battleMessage.targetsHpEffect.Add(injured.PositionIndex, -result);
                //更新角色hp及状态
                if (injured.Role.Hp > result)
                {
                    injured.Role.ReduceHp(result);
                }
                else
                {
                    injured.Role.ChangeHp(1);
                    battle.BattlerDead(ref injured);
                }
            }
        }
    }

    public class FanYunFuYu : SkillBase
    {
        public FanYunFuYu(int skillId) : base(skillId)
        {

        }

        public override void Release(Battle battle, Battler actor, Battler[] victims, ref S2C_BattleMessage S2C_battleMessage)
        {
            //更新战斗消息中的行动者站位索引和受害者站位索引
            S2C_battleMessage.actorIndex = actor.PositionIndex;
            byte[] victimsIndex = new byte[victims.Length];
            for (int i = 0; i < victimsIndex.Length; i++)
            {
                victimsIndex[i] = victims[i].PositionIndex;
            }
            S2C_battleMessage.victims = victimsIndex;

            SkillConfig config = ConfigSys.Instance.GetConfig<SkillConfig>(SkillId);
            //更新行动者所施展的技能所消耗的mp，并更新角色mp
            //int mpConsume = config.consume;
            //actor.Role.ReduceMp(mpConsume);


            //遍历受害者站位索引数组
            for (byte j = 0; j < victims.Length; j++)
            {
                //获取受害者参战数据
                Battler injured = victims[j];

                //得出最终伤害
                int result = (int)(actor.magicAttack * config.skillRatio) - injured.magicDefense;
                if (result < 0)
                {
                    result = 0;
                }

                //battle.AddActionBuff(new Poisoning(2006, -5, 3), ref injured);
                //S2C_battleMessage.actionBuffs.Add(injured.PositionIndex, 2006);
                //S2C_battleMessage.actionBuffsEffect.Add(injured.PositionIndex, -5);
                S2C_battleMessage.targetsHpEffect.Add(injured.PositionIndex, -result);
                //更新角色hp及状态
                if (injured.Role.Hp > result)
                {
                    injured.Role.ReduceHp(result);
                }
                else
                {
                    injured.Role.ChangeHp(1);
                    battle.BattlerDead(ref injured);
                }

            }
        }
    }
}
