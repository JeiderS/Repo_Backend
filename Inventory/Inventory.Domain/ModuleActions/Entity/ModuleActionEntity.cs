using Inventory.Domain.Actions.Entity;
using Inventory.Domain.Modules.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Domain.ModuleActions.Entity
{
    public class ModuleActionEntity
    {
        private ModuleActionEntity() { }

        public int ModuleId { get; private set; }
        public int ActionId { get; private set; }

        public ModuleEntity Module { get; private set; } = default!;
        public ActionEntity Action { get; private set; } = default!;

        public static ModuleActionEntity Create(int moduleId, int actionId) => new()
        {
            ModuleId = moduleId,
            ActionId = actionId
        };
    }
}
