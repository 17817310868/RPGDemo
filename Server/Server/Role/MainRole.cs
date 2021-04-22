/*
 * ===================================================================
 * 
 *          projectName:
 *              项目名称:拱猪
 *                  
 *          title:
 *              标题:主角信息模型
 *          
 *          description:
 *              功能描述:设计主角信息
 *              
 *          author:
 *              作者:照着教程敲出bug的程序员
 * 
 * ===================================================================
 */

using Net;
using Server.GameSys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public enum RoleState
    {
        None = 0,
        Idle,
        Battle,
        Follow
    }
    public class MainRole:RoleBase
    {
        public MainRole(string guid,RoleEnum roleType,int profession) : base(guid,roleType, profession)
        {
            inventory = new Inventory();
            skills = new SkillInfo();
            tasks = new TaskInfo();
            mails = new List<Mail>();
        }
        private int schoolId;
        private int moveSpeed;
        private string teamId = "-1";
        private bool leader = false;
        private RoleState state = RoleState.Idle;
        private UnityEngine.Vector3 position;
        private Inventory inventory;
        private SkillInfo skills;
        private TaskInfo tasks;
        private List<Mail> mails;
        private int silver;
        private int gold;
        private int yuanBao;
        public Inventory Inventory { get { return inventory; } }
        public SkillInfo Skills { get { return skills; } }
        public TaskInfo Tasks { get { return tasks; } }
        public List<Mail> Mails { get { return mails; } }
        public int SchoolId { get { return schoolId; } }
        public int MoveSpeed { get { return moveSpeed; } }
        public string TeamId { get { return teamId; } }
        public bool Leader { get { return leader; } }
        public RoleState State { get { return state; } }
        public UnityEngine.Vector3 Position { get { return position; } }

        public int Silver { get { return silver; } }
        public int Gold { get { return gold; } }
        public int YuanBao { get { return yuanBao; } }


        public void ChangeSchoolId(int schoolId)
        {
            this.schoolId = schoolId;
        }

        public void ChangeLeader(bool IsTrue)
        {
            leader = IsTrue;
        }

        public void ChangeTeam(string teamId)
        {
            this.teamId = teamId;
        }

        public void ChangeMoveSpeed(int moveSpeed)
        {
            this.moveSpeed = moveSpeed;
        }

        public void ChangeState(RoleState state)
        {
            this.state = state;
        }

        public void ChangePosition(UnityEngine.Vector3 position)
        {
            this.position = position;
        }

        public void ReduceSilver(int effect)
        {
            silver -= effect;
        }

        public void AddSilver(int effect)
        {
            silver += effect;
        }

        public void ChangeSilver(int silver)
        {
            this.silver = silver;
        }

        public void ReduceGold(int effect)
        {
            gold -= effect;
        }

        public void AddGold(int effect)
        {
            gold += effect;
        }

        public void ChangeGold(int gold)
        {
            this.gold = gold;
        }

        public void ReduceYuanBao(int effect)
        {
            yuanBao -= effect;
        }

        public void AddYuanBao(int effect)
        {
            yuanBao += effect;
        }

        public void ChangeYuanBao(int yuanBao)
        {
            this.yuanBao = yuanBao;
        }

    }
}
