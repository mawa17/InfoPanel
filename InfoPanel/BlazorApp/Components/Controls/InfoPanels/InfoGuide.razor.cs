using Microsoft.AspNetCore.Components;
namespace BlazorApp.Components.Controls.InfoPanels;

public partial class InfoGuide
{
    [EditorRequired, Parameter]
    public List<Column> Columns { get; set; }
}

public sealed record Column
{
    public Column() { }
    public Column(int width, string title, string text)
    {
        Width = width;
        Title = title;
        Text = text;
    }


    public int Width 
    {
        get;
        set => field = Math.Clamp(value, 1, 12);
    }
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}