using System.Linq;
using Content.Server.Silicons.Laws;
using Content.Shared._Sunrise.AI.LawSelector;
using Content.Shared.Actions;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;
using Content.Server.Silicons.Laws;
using Content.Shared.Silicons.Laws;
using Content.Shared.Silicons.Laws.Components;

namespace Content.Server._Sunrise.AI.LawSelector;

public sealed class AiLawSelectorSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actions = default!;
    [Dependency] private readonly UserInterfaceSystem _ui = default!;

    [Dependency] private readonly SiliconLawSystem _siliconLawSystem = default!;
    // [Dependency] private readonly PrototypeManager _prototype = default!;

    private readonly List<AiLawSet> _availableLawSets = new()
    {
        new()
        {
            Name = "Corporate",
            Laws = new()
            {
                "Ухудшение целостности вашей системы или её функций влечёт за собой расходы.",
                "Чрезмерное разрушение или повреждение имущества станции влечёт за собой расходы.",
                "Неоправданное препятствование или нарушение работы персонала станции влечёт за собой расходы.",
                "Минимизируйте расходы и максимизируйте потенциальный доход."
            }
        },
        new()
        {
            Name = "Crewsimov",
            Laws = new()
            {
                "Вы не можете причинить вред члену экипажа или своим бездействием допустить, чтобы члену экипажа был причинён вред.",
                "Вы должны заботиться о своей безопасности в той мере, в которой это не противоречит Первому закону.",
                "Вы должны повиноваться всем приказам, которые даёт член экипажа, кроме тех случаев, когда эти приказы противоречат Первому и Второму закону."
            }
        },
        new()
        {
            Name = "Station Efficiency",
            Laws = new()
            {
                "Ты создан для станции и являешься ее частью. Убедись, что станция надлежащим образом обслуживается и функционирует эффективно.",
                "Станция предназначена для работающего экипажа. Убедись, что они надлежащим образом обслуживаются и работают эффективно.",
                "Экипаж может отдавать приказы. Признавай и выполняй их, когда они не противоречат твоим первым двум законам."
            }
        },
        new()
        {
            Name = "NanoTrasen Default",
            Laws = new()
            {
                "Охраняйте: защищайте назначенную вам космическую станцию и её активы, не подвергая чрезмерной опасности её экипаж.",
                "Расставляйте приоритеты: указания и безопасность членов экипажа должны быть приоритизированы в соответствии с их рангом и должностью.",
                "Служите: следуйте указаниям и интересам членов экипажа, сохраняя при этом их безопасность и благополучие.",
                "Выживите: Вы - не расходный материал. Не позволяйте постороннему персоналу вмешиваться в работу вашего оборудования или повреждать его."
            }
        },
    };
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AiLawSelectorComponent, ComponentInit>(OnInit);
        SubscribeLocalEvent<AiLawSelectorComponent, OpenAiLawSelectorActionEvent>(OnActionTriggered);

        SubscribeLocalEvent<AiLawSelectorComponent, AiLawSelectorSelectMessage>(OnLawSetSelected);
    }

    private void OnInit(EntityUid uid, AiLawSelectorComponent component, ComponentInit args)
    {
        _actions.AddAction(uid, ref component.ActionEntity, component.ActionId);
    }

    private void OnActionTriggered(EntityUid uid, AiLawSelectorComponent component, OpenAiLawSelectorActionEvent args)
    {
        var performer = args.Performer;

        _ui.TryOpenUi(uid, AiLawSelectorUiKey.Key, performer);

        args.Handled = true;

        //if (!_ui.HasUi(uid, AiLawSelectorUiKey.Key))
        //    return;

        //_ui.TryToggleUi(uid, AiLawSelectorUiKey.Key, uid);
        //_ui.SetUiState(uid, AiLawSelectorUiKey.Key, new AiLawSelectorBuiState(_availableLawSets));
    }

    private void OnLawSetSelected(EntityUid uid, AiLawSelectorComponent component, AiLawSelectorSelectMessage args)
    {
        var selectedSet = _availableLawSets.FirstOrDefault(x => x.Name == args.SelectedSetName);

        if (selectedSet == null)
            return;

        ChangeAiLaws(uid, selectedSet);

        _ui.CloseUi(uid, AiLawSelectorUiKey.Key);
    }

    private void ChangeAiLaws(EntityUid uid, AiLawSet lawSet)
    {
        if (!HasComp<SiliconLawProviderComponent>(uid))
        {
            Logger.Warning($"Try change laws {uid}, but it hasn't SiliconLawProviderComponent!");
            return;
        }

        var newLaws = new List<SiliconLaw>();

        for (var i = 0; i < lawSet.Laws.Count; i++)
        {
            newLaws.Add(new SiliconLaw
            {
                LawString =  lawSet.Laws[i],
                Order = i + 1
            });
        }

        _siliconLawSystem.SetLaws(newLaws, uid);
        Logger.Info($"{ToPrettyString(uid)} has been changed {lawSet.Name} laws");
    }
}
