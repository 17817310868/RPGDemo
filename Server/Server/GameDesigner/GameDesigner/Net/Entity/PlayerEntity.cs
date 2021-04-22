using System.Collections.Generic;
using Net.EventSystem;
using Net.Server;
using Net.Share;

namespace Net.Entity
{
    public class PlayerEntity : NetPlayer, IEntityComponent
    {
        public NTransform transform = new NTransform();
        public Vector3 direction;
        public string account;
        public string password;
        public float moveSpeed = 0.2f;
        public List<Operation> operations = new List<Operation>();
        public int[] skills = new int[] { 20, 50, 75 };
        public int health = 1000;
        public bool attack;

        public new SceneManager<PlayerEntity> Scene {
            get {
                return base.Scene as SceneManager<PlayerEntity>;
            }
        }

        public bool IsDead { get { return health <= 0; } }

        public override List<Operation> Update()
        {
            if (direction != Vector3.zero & !IsDead & !attack) {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction, Vector3.up), 0.5f);
                transform.Translate(Vector3.forward * moveSpeed);
            }
            operations.Add(new Operation() {
                cmd = Command.Movement,
                name = account,
                position = transform.position,
                rotation = transform.rotation,
                direction = direction
            });
            //解决多线程填充列表
            int count = operations.Count;
            var ops = operations.GetRange(0, count);
            operations.RemoveRange(0, count);
            return ops;
        }

        public override void OnRevdCustomBuffer(byte cmd, byte[] buffer, int index, int count)
        {
            switch (cmd) {
                case Command.Operation:
                    NetConvert.Deserialize(buffer, index, count, (func, pars) => {
                        OnInputOperation((UnityEngine.Vector3)pars[0]);
                    });
                    break;
                case Command.Attack:
                    Attack(buffer[index]);
                    break;
            }
        }

        void Attack(int index)
        {
            operations.Add(new Operation() {
                cmd = Command.Attack,
                name = account,
                index = index
            });//玩家操作同步攻击
            TimeManager.Delay(1000, () => {
                foreach (var p in Scene.Players) {
                    if (Vector3.Distance(transform.position, p.transform.position) < 5f & p != this) {
                        p.health -= skills[index];
                    }
                    operations.Add(new Operation() {
                        cmd = Command.SyncHealth,
                        name = p.account,
                        index = p.health
                    });
                }
            });
            attack = true;
            TimeManager.Delay(1800, () => {
                attack = false;
            });
        }

        void OnInputOperation(UnityEngine.Vector3 dir)
        {
            direction = dir;
        }

        [Rpc]
        void CreatePlayer()
        {
            foreach (var p in Scene.Players) {
                operations.Add(new Operation() { cmd = Command.CreatePlayer, name = p.account, position = p.transform.position, rotation = p.transform.rotation });
            }
        }

        public override void OnRemoveClient()
        {
            Scene.operations.Add(new Operation() { cmd = Command.QuitGame, name = account });
        }
    }
}
