using Microsoft.AspNetCore.Components;

namespace BlazorApp.Components.Controls;

public sealed class DiashowItem : ComponentBase
{
    [Parameter] public RenderFragment ChildContent { get; set; } = null!;
    [CascadingParameter] public Diashow? Parent { get; set; }
    protected override void OnInitialized()
    {
        Parent?.Register(this);
    }
}
