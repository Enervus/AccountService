using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountService.Domain.Results
{
    public class CollectionResult<TEntity>: BaseResult<IEnumerable<TEntity>>
    {
        public int Count { get; set; }
    }
}
