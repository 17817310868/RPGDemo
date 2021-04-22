using Net.Share;

namespace Net.Entity
{
    public sealed class ServerEntity : Server.UdpServer<PlayerEntity, SceneManager<PlayerEntity>>
    {
        protected override void OnStartupCompleted()
        {
            GameManager.Run(this);
        }

        protected override PlayerEntity OnUnClientRequest(PlayerEntity unClient, byte cmd, byte[] buffer, int index, int count)
        {
            var func = NetConvert.Deserialize(buffer, index, count);
            switch (func.name) {
                case "Register":
                Register(unClient, func.pars[0].ToString(), func.pars[1].ToString());
                break;
                case "Login":
                return Login(unClient, func.pars[0].ToString(), func.pars[1].ToString());
            }
            return null;
        }

        void Register(PlayerEntity player, string account, string password)
        {
            if (DataBaseEntity.Ins.Contains(account)) {
                Send(player, "RegisterCallback", "注册失败！");
                return;
            }
            var player1 = new PlayerEntity();
            player1.playerID = account;
            player1.account = account;
            player1.password = password;
            DataBaseEntity.Ins.AddPlayer(player1);
            DataBaseEntity.Ins.Save(player1);
            Send(player, "RegisterCallback", "注册成功！");
        }

        PlayerEntity Login(PlayerEntity player, string account, string password)
        {
            if (!DataBaseEntity.Ins.Contains(account)) {
                Send(player, "LoginCallback", false, "输入帐号错误！");
                return null;
            }
            var player1 = DataBaseEntity.Ins[account];
            if (player1.password != password) {
                Send(player, "LoginCallback", false, "输入密码错误！");
                return null;
            }
            Send(player, "LoginCallback", true, "登录成功！");
            return player1;
        }
    }
}
