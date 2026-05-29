using System;
using System.Collections.Generic;
using System.Numerics;
using Content.Shared._Sunrise.AI.LawSelector;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Maths;

namespace Content.Client._Sunrise.AI.LawSelector;

public sealed partial class AiLawSelectorWindow : DefaultWindow
    {
        public Action<string>? OnLawSetSelected;

        private BoxContainer LawsContainer;

        public AiLawSelectorWindow()
        {
            RobustXamlLoader.Load(this);

            LawsContainer = this.FindControl<BoxContainer>("LawsContainer");
        }

        public void UpdateLayout(List<AiLawSet> lawSets)
        {
            LawsContainer.DisposeAllChildren();

            foreach (var set in lawSets)
            {
                var setBox = new BoxContainer
                {
                    Orientation = BoxContainer.LayoutOrientation.Vertical,
                    SeparationOverride = 4
                };

                var titleLabel = new Label
                {
                    Text = set.Name,
                    StyleClasses = { "LabelBig" }
                };
                setBox.AddChild(titleLabel);

                foreach (var law in set.Laws)
                {
                    var lawLabel = new RichTextLabel();
                    lawLabel.SetMessage(law);

                    setBox.AddChild(lawLabel);
                }

                string buttonText = $"Активировать {set.Name}";

                var selectButton = new Button
                {
                    Text = buttonText,
                    HorizontalAlignment = Control.HAlignment.Left
                };

                selectButton.OnPressed += _ => OnLawSetSelected?.Invoke(set.Name);
                setBox.AddChild(selectButton);

                LawsContainer.AddChild(setBox);

                LawsContainer.AddChild(new PanelContainer { MinSize = new Vector2(0, 2) });
            }
        }
    }
