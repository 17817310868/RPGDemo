/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:buff信息类
 *          
 *          description:
 *              功能描述:设计所有buff结构
 *              
 *          author:
 *              作者:照着教程敲出bug的程序员
 * 
 * ===================================================================
 */

using Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Buff
{
    /// <summary>
    /// buff基类
    /// </summary>
    public class BuffBase
    {
        public BuffBase(int buffId, int effect,byte rounds)
        {
            this.buffId = buffId;
            this.effect = effect;
            this.rounds = rounds;
        }

        public virtual void Calculate(ref Battler battler)
        {
            if (rounds > 0)
                rounds--;
        }

        public int buffId;
        public byte rounds;
        public int effect;
    }

    /// <summary>
    /// 物理攻击提升
    /// </summary>
    public class PhysicalAttackPromote : BuffBase
    {
        public PhysicalAttackPromote(int buffId, int effect,byte rounds) : base(buffId,effect,rounds)
        {

        }

        public override void Calculate(ref Battler battler)
        {
            base.Calculate(ref battler);
            battler.physicalAttack += effect;
        }
    }

    /// <summary>
    /// 物理防御提升
    /// </summary>
    public class PhysicalDefensePromote : BuffBase
    {
        public PhysicalDefensePromote(int buffId, int effect,byte rounds) : base(buffId,effect,rounds)
        {

        }

        public override void Calculate(ref Battler battler)
        {
            base.Calculate(ref battler);
        }
    }
    
    /// <summary>
    /// 元气攻击提升
    /// </summary>
    public class MagicAttackPromote : BuffBase
    {
        public MagicAttackPromote(int buffId, int effect,byte rounds) : base(buffId,effect,rounds)
        {

        }

        public override void Calculate(ref Battler battler)
        {
            base.Calculate(ref battler);
            battler.magicAttack += effect;
        }
    }

    /// <summary>
    /// 元气防御提升
    /// </summary>
    public class MagicDefensePromote: BuffBase
    {
        public MagicDefensePromote(int buffId, int effect,byte rounds) : base(buffId,effect,rounds)
        {

        }

        public override void Calculate(ref Battler battler)
        {
            base.Calculate(ref battler);
        }
    }

    /// <summary>
    /// 气血持续回复
    /// </summary>
    public class HpReply: BuffBase
    {
        public HpReply(int buffId, int effect,byte rounds) : base(buffId,effect,rounds)
        {

        }

        public override void Calculate(ref Battler battler)
        {
            base.Calculate(ref battler);
        }
    }

    /// <summary>
    /// 烧伤
    /// </summary>
    public class Burn: BuffBase
    {
        public Burn(int buffId, int effect,byte rounds) : base(buffId, effect, rounds)
        {

        }

        public override void Calculate(ref Battler battler)
        {
            base.Calculate(ref battler);
        }
    }

    /// <summary>
    /// 中毒
    /// </summary>
    public class Poisoning : BuffBase
    {
        public Poisoning(int buffId,int effect ,byte rounds) : base(buffId, effect, rounds)
        {

        }

        public override void Calculate(ref Battler battler)
        {
            base.Calculate(ref battler);
            if (battler.Role.Hp > Mathf.Abs(effect))
            {
                battler.Role.AddHp(effect);
            }
            else
            {
                battler.Role.ChangeHp(1);
            }
        }
    }

    /// <summary>
    /// 晕眩
    /// </summary>
    public class Dizzy: BuffBase
    {
        public Dizzy(int buffId, int effect,byte rounds) : base(buffId, effect, rounds)
        {

        }

        public override void Calculate(ref Battler battler)
        {
            base.Calculate(ref battler);
        }
    }

    /// <summary>
    /// 冰冻
    /// </summary>
    public class Frozen : BuffBase
    {
        public Frozen(int buffId, int effect,byte rounds) : base(buffId, effect, rounds)
        {

        }

        public override void Calculate(ref Battler battler)
        {
            base.Calculate(ref battler);
        }
    }
}
