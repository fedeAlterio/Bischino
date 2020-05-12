using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bischino.Model;
using Skribble.Model;

namespace Bischino.Skribble
{
    public interface ISkribbleHandler
    {
        void UpdateDraw(RoomQuery<IEnumerable<Line>> drawUpdatedQuery);
        Task<IEnumerable<Line>> GetDrawUpdate(RoomQuery roomQuery);
    }
}
