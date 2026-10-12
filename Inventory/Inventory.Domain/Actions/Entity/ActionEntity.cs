using System.Text;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Modules.Entity;

namespace Inventory.Domain.Actions.Entity
{
    public class ActionEntity
    {
        // Para EF Core.
        private ActionEntity() { }

        public int Id { get; private set; }
        public int ModuleId { get; private set; }
        public string Code { get; private set; } = default!;
        public string Name { get; private set; } = default!;
        public bool IsActive { get; private set; }

        public ModuleEntity? Module { get; private set; }

        /// <summary>
        /// Crea una acción para un módulo. Regla de negocio: el código es
        /// "{Módulo}{Nombre}" en PascalCase, la misma convención de
        /// Sql/TenantBootstrap/03_Actions_Seed.sql. Ej.: módulo "Users" +
        /// nombre "export report" => "UsersExportReport".
        /// Como la regla vive aquí, es imposible crear una acción con un código
        /// mal formado desde cualquier parte del sistema.
        /// </summary>
        public static Result<ActionEntity, Error> Create(ModuleEntity module, string name)
        {
            var namePascal = ToPascalSegment(name);
            if (namePascal.Length == 0)
                return ActionErrors.InvalidName();

            return new ActionEntity
            {
                ModuleId = module.Id,
                Module = module,
                Code = module.Name.Replace(" ", string.Empty) + namePascal,
                Name = name,
                IsActive = true
            };
        }

        /// <summary>
        /// Quita espacios y separadores, y pone en mayúscula la primera letra de
        /// cada palabra: "Export Report" -> "ExportReport".
        /// </summary>
        private static string ToPascalSegment(string value)
        {
            var words = value.Split(
                new[] { ' ', '-', '_', '.', ',' },
                StringSplitOptions.RemoveEmptyEntries);

            var builder = new StringBuilder();
            foreach (var word in words)
            {
                var cleaned = new string(word.Where(char.IsLetterOrDigit).ToArray());
                if (cleaned.Length == 0)
                    continue;

                builder.Append(char.ToUpperInvariant(cleaned[0]));
                if (cleaned.Length > 1)
                    builder.Append(cleaned[1..]);
            }

            return builder.ToString();
        }
    }
}
