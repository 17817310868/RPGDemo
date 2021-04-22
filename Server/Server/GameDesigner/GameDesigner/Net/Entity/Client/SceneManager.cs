using Net.Client;
using Net.Share;
using System.Collections.Generic;
using UnityEngine;

namespace Net.Entity.Client
{
    public class SceneManager
    {
        private NetClientBase client;
        public Dictionary<string, NetworkTransform> transforms = new Dictionary<string, NetworkTransform>();

        public SceneManager(NetClientBase client)
        {
            this.client = client;
            client.OnRevdCustomBufferHandle += OnRevdCustomBufferHandle;
        }

        ~SceneManager()
        {
            if(client != null)
                client.OnRevdCustomBufferHandle -= OnRevdCustomBufferHandle;
        }

        void OnRevdCustomBufferHandle(byte cmd, byte[] buffer, int index, int count)
        {
            switch (cmd) {
                case Command.SyncOperations:
                    NetConvert.Deserialize(buffer, index, count, (func, pars) => {
                        OnSyncOperations(pars[0] as OperationList);
                    });
                    break;
            }
        }

        void OnSyncOperations(OperationList list)
        {
            foreach (var op in list.operations) {
                if (!transforms.ContainsKey(op.name)) {
                    var t = OnCreateTransform(op);
                    if (t != null)
                        transforms.Add(op.name, t);
                } else {
                    var t = transforms[op.name];
                    t.position = op.position;
                    t.rotation = op.rotation;
                }
            }
        }

        public virtual NetworkTransform OnCreateTransform(Operation operation)
        {
            var t = new GameObject(operation.name);
            var tt = t.AddComponent<NetworkTransform>();
            tt.Name = operation.name;
            tt.position = operation.position;
            tt.rotation = operation.rotation;
            tt.transform.position = operation.position;
            tt.transform.rotation = operation.rotation;
            return tt;
        }
    }
}
