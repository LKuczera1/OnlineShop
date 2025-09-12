using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility.DtoEntity
{
    public interface IEntity <TDto>
    {
        public TDto ToDto();
    }
}
