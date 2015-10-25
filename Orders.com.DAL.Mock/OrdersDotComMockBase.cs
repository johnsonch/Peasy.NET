using Peasy.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orders.com.DAL.Mock
{
    public abstract class OrdersDotComMockBase<DTO> : MockRepositoryBase<DTO, long> where DTO : IDomainObject<long>
    {
        protected override long GetNextID()
        {
            if (Data.Values.Any())
                return Data.Values.Max(c => c.ID) + 1;

            return 1;
        }
    }
}
