using System.Collections.Generic;

namespace Net.Entity
{
    public class OperationList
    {
        public long frame;
        public List<Operation> operations = new List<Operation>();

        public void Add(Operation op)
        {
            operations.Add(op);
        }

        public void AddRange(IEnumerable<Operation> list)
        {
            operations.AddRange(list);
        }
    }
}
