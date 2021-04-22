using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net.Entity
{
    public class DataBaseEntity : Server.ServerDataBase<PlayerEntity>
    {
        public static DataBaseEntity Ins = new DataBaseEntity();
    }
}
