using Microsoft.AspNetCore.Components;
using Timer = System.Timers.Timer;
namespace BlazorApp.Components.Controls;

public partial class Diashow : IDisposable
{
    [Parameter, EditorRequired] public RenderFragment ChildContent { get; set; } = null!;
    [Parameter] public bool FreezeOnHover { get; set; }
    [Parameter] public bool DisplayProgressBar { get; set; }
    [Parameter] public bool DisplayNavButtons { get; set; }
    [Parameter] public uint IntervalMs { get; set; } = 3000;
    [Parameter] public byte TickIntervalMs { get; set; } = 50;

    private readonly List<DiashowItem> Items = new();
    private int ItemIndex = 0;
    private Timer? SlideTimer;
    private Timer? ProgressTimer;
    private double ElapsedMs = 0;
    private bool IsHovered = false;

    private byte ProgressPercentage =>
         (byte)(DisplayProgressBar ? Math.Clamp(ElapsedMs / (IntervalMs-TickIntervalMs) * 100, 0, 100) : 0);

    internal void Register(DiashowItem item)
    {
        Items.Add(item);
        StateHasChanged();
    }
    
    private void Next()
    {
        if (Items.Count == 0) return;
        ItemIndex = (ItemIndex + 1) % Items.Count;
        ResetTimers();
    }

    private void Prev()
    {
        if (Items.Count == 0) return;
        ItemIndex = (ItemIndex - 1 + Items.Count) % Items.Count;
        ResetTimers();
    }

    private void ResetTimers()
    {
        ElapsedMs = 0;

        SlideTimer?.Stop();
        ProgressTimer?.Stop();

        if (!(IsHovered && FreezeOnHover))
        {
            SlideTimer?.Start();
            if (DisplayProgressBar) ProgressTimer?.Start();
        }

        StateHasChanged();
    }

    private void OnMouseEnter()
    {
        IsHovered = true;
        if (FreezeOnHover) PauseTimers();
    }

    private void OnMouseLeave()
    {
        IsHovered = false;
        if (FreezeOnHover) ResumeTimers();
    }

    private void PauseTimers()
    {
        SlideTimer?.Stop();
        ProgressTimer?.Stop();
        ElapsedMs = 0;
        StateHasChanged();
    }

    private void ResumeTimers()
    {
        if (!FreezeOnHover) return;
        SlideTimer?.Start();
        if (DisplayProgressBar) ProgressTimer?.Start();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (!firstRender || Items.Count <= 1) return;

        SlideTimer = new(IntervalMs);
        SlideTimer.Elapsed += (_, _) =>
        {
            InvokeAsync(() =>
            {
                Next();
                ElapsedMs = 0;
            });
        };
        SlideTimer.AutoReset = true;
        SlideTimer.Start();

        if (DisplayProgressBar)
        {
            ProgressTimer = new Timer(TickIntervalMs);
            ProgressTimer.Elapsed += (_, _) =>
            {
                InvokeAsync(() =>
                {
                    ElapsedMs = Math.Clamp(ElapsedMs + TickIntervalMs, 0, IntervalMs);
                    StateHasChanged();
                });
            };
            ProgressTimer.AutoReset = true;
            ProgressTimer.Start();
        }
    }

    public void Dispose()
    {
        SlideTimer?.Stop();
        SlideTimer?.Dispose();
        ProgressTimer?.Stop();
        ProgressTimer?.Dispose();
    }
}