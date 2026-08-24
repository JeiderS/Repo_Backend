using System.Text;
using Inventory.Application.Actions.Dto;
using Inventory.Application.Actions.Errors;
using Inventory.Domain.Actions.DomainActions;
using Inventory.Domain.Actions.Entity;
using Inventory.Domain.Common.Persistence;
using Inventory.Domain.Common.Results;
using Inventory.Domain.Common.Results.Errors;
using Inventory.Domain.Modules.DomainModules;
using MediatR;

namespace Inventory.Application.Actions.Command.CreateAction;

public class CreateActionCommandHandler(
    IModuleGetByIdService moduleGetByIdService,
    IActionCreateService actionCreateService,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateActionCommand, Result<ActionDto, Error>>
{
    public async Task<Result<ActionDto, Error>> Handle(CreateActionCommand request, CancellationToken cancellationToken)
    {
        var module = await moduleGetByIdService.GetByIdAsync(request.ModuleId);
        if (module is null)
            return ActionErrorBuilder.ModuleNotFound();

        var namePascal = ToPascalSegment(request.Name);
        if (namePascal.Length == 0)
            return ActionErrorBuilder.InvalidName();

        var modulePascal = module.Name.Replace(" ", string.Empty);
        var code = modulePascal + namePascal;

        if (await actionCreateService.CodeExistsAsync(code))
            return ActionErrorBuilder.CodeAlreadyExists();

        var action = new ActionEntity
        {
            ModuleId = module.Id,
            Code = code,
            Name = request.Name,
            IsActive = true
        };

        await actionCreateService.AddAsync(action);

        var savedRows = await unitOfWork.SaveChangesAsync(cancellationToken);
        if (savedRows <= 0)
            return ActionErrorBuilder.CreationException();

        action.Module = module;

        return ActionDto.FromEntity(action);
    }

    /// <summary>
    /// Mirrors the '{Module}{Verb}' convention from Sql/TenantBootstrap/03_Actions_Seed.sql:
    /// strips spaces/non-alphanumeric separators and capitalizes each remaining word,
    /// e.g. "Export Report" -> "ExportReport".
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
