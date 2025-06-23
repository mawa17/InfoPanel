using Microsoft.AspNetCore.Components;
namespace BlazorApp.Components.Controls.InfoPanels;

public partial class InfoGuide
{
    [EditorRequired, Parameter]
    public InfoGuideModel[] Items { get; set; }
}

public sealed record InfoGuideModel(byte ColoumWidth, string Title, string Content);