﻿using System.Text.RegularExpressions;
using System.Windows.Documents;
using System.Windows.Media;
using PixiEditor.Helpers;
using PixiEditor.Models.DataHolders;

namespace PixiEditor.Models.Commands.Search;

internal abstract class SearchResult : NotifyableObject
{
    private bool isSelected;
    private bool isMouseSelected;

    public string SearchTerm { get; init; }

    public virtual Inline[] TextBlockContent => GetInlines().ToArray();

    public Match Match { get; init; }

    public abstract string Text { get; }

    public virtual string Description { get; }

    public abstract bool CanExecute { get; }

    public abstract ImageSource Icon { get; }

    public bool IsSelected
    {
        get => isSelected;
        set => SetProperty(ref isSelected, value);
    }

    public bool IsMouseSelected
    {
        get => isMouseSelected;
        set => SetProperty(ref isMouseSelected, value);
    }


    public abstract void Execute();

    public virtual KeyCombination Shortcut { get; }

    public RelayCommand ExecuteCommand { get; }

    public SearchResult()
    {
        ExecuteCommand = new(_ => Execute(), _ => CanExecute);
    }

    private IEnumerable<Inline> GetInlines()
    {
        if (Match == null)
        {
            yield return new Run(Text);
            yield break;
        }

        foreach (Group group in Match.Groups.Values.Skip(1))
        {
            var run = new Run(group.Value);

            if (group.Value.Equals(SearchTerm, StringComparison.OrdinalIgnoreCase))
            {
                yield return new Bold(run);
            }
            else
            {
                yield return run;
            }
        }
    }
}
