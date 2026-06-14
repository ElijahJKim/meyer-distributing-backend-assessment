using System.Collections.Generic;
using System.Linq;

namespace InterviewTest.Returns
{
    public class ReturnRepository
    {
        private List<IReturn> returns;
        public ReturnRepository()
        {
            returns = new List<IReturn>();
        }

        public void Add(IReturn newReturn)
        {
            returns.Add(newReturn);
        }

        public void Remove(IReturn removedReturn)
        {
            returns = returns.Where(o => !string.Equals(removedReturn.ReturnNumber, o.ReturnNumber)).ToList();
        }

        public List<IReturn> Get()
        {
            return returns;
        }
    }
}
