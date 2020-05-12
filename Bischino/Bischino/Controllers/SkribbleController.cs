using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bischino.Model;
using Bischino.Skribble;
using Microsoft.AspNetCore.Mvc;
using Skribble.Model;

namespace Bischino.Controllers
{
    public class SkribbleController : AbstractController
    {
        private readonly ISkribbleHandler _skribbleHandler;

        public SkribbleController(ISkribbleHandler skribbleHandler)    
        {
            this._skribbleHandler = skribbleHandler;
        }


        public IActionResult NotifyDrawUpdated([FromBody] RoomQuery<IEnumerable<Line>> drawUpdate)
            => TryOk(() => _skribbleHandler.UpdateDraw(drawUpdate));


        public Task<IActionResult> GetDrawUpdate([FromBody] RoomQuery roomQuery)
            => TryValuedOkAsync(() => _skribbleHandler.GetDrawUpdate(roomQuery));
    }
}
