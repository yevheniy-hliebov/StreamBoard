namespace StreamTabula.Features.Decks.Models;

public enum ImageType { System, Custom }

public record ImageLibraryResult(ImageType Type, string Path);
