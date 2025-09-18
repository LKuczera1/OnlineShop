using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.Enums;

namespace Utility.Common
{
    public record UserData(int? clientId, PriviledgeLevel priviledgeLevel);
}
