namespace MiniTwitch.Helix.Responses;

public class ActiveExtensions
{
    public ExtensionsData Data { get; init; }

    public record Component(
         bool Active,
         string? Id,
         string? Version,
         string? Name,
         int? X,
         int? Y
    );

    public record Panel(
         bool Active,
         string? Id,
         string? Version,
         string? Name
    );

    public record Overlay(
         bool Active,
         string? Id,
         string? Version,
         string? Name
    );

    public record ExtensionsData(
         Dictionary<int, Panel> Panel,
         Dictionary<int, Overlay> Overlay,
         Dictionary<int, Component> Component
    );
}
