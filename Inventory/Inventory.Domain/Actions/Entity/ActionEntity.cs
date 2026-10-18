using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.ModuleActions.Entity;
using Inventory.Domain.Modules.Entity;
using System.Text;

namespace Inventory.Domain.Actions.Entity
{
    public class ActionEntity
    {
        private ActionEntity() { }

        public int Id { get; private set; }
        public string Code { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public bool IsActive { get; private set; }

        public ICollection<ModuleActionEntity> ModuleActions { get; private set; }
            = new List<ModuleActionEntity>();

        public static ActionEntity Create(string code, string name)
        {
            return new ActionEntity
            {
                Code = code,
                Name = name,
                IsActive = true
            };
        }

        public void Update(string code, string name)
        {
            Code = code;
            Name = name;
        }
    }
}
