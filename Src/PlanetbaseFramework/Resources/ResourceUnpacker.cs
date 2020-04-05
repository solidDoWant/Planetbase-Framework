// Could be abstract class instead due to the need fo Unpack with same implementation all accross.
public interface IResourceUnpacker
{
    public void Unpack(Stream resourceStream, string file);
}

public class ZipUnpacker : IResourceUnpacker
{
    private readonly FastZip _zipInstance = new SomethingFastZip();

    public void Unpack(string file)
    {
        using (var resourceStream = currentAssembly.GetManifestResourceStream(file))
        {
            Unpack(file, resourceStream);
        }
    }

    public void Unpack(Stream resourceStream, string file)
    {
        Debug.Log("zip " + GetResourceRelativeFilePath(file));
        ZipInstance.ExtractZip(
            resourceStream,
            ModPath,
            FastZip.Overwrite.Always,
            null,
            null,
            null,
            false,
            false
        );
    }
}

public class NoUnpacker : IResourceUnpacker
{
    public void Unpack(string file)
    {
        using (var resourceStream = currentAssembly.GetManifestResourceStream(file))
        {
            Unpack(file, resourceStream);
        }
    }

    public void Unpack(Stream resourceStream, string file)
    {
        var filePath = Path.Combine(ModPath, GetResourceRelativeFilePath(file));

        Debug.Log($"Loading \"{file}\" to \"{filePath}\"");

        Directory.CreateDirectory(Path.GetDirectoryName(filePath));

        using (var fileStream = File.Create(filePath))
        {
            resourceStream.CopyTo(fileStream);
        }

        break;
    }
}


public class ResourceUnpacker : IResourceUnpacker
{
    private readonly Dictionary<string, IResourceUnpacker> _unpackers = new Dictionary<string, IResourceUnpacker>();
    private readonly IResourceUnpacker _defaultUnpacker = new NoUnpacker();

    public void RegisterUnpacker(string fileExtension, IResourceUnpacker unpacker)
    {
        _unpackers.Add(fileExtension, unpacker);
    }

    public void RegisterDefault(IResourceUnpacker unpacker)
    {
        _defaultUnpacker = unpacker;
    }

    public void Unpack(string file)
    {
        using (var resourceStream = currentAssembly.GetManifestResourceStream(file))
        {
            Unpack(file, resourceStream);
        }
    }

    public void Unpack(Stream resourceStream, string file)
    {
        using (var resourceStream = currentAssembly.GetManifestResourceStream(file))
        {
            var extension = Path.GetExtension(file);
            if (_unpackers.ContainsKey(extension))
            {
                _unpackers[extension].Unpack(resourceStream, file);
            }
            else
            {
                _defaultUnpacker.Unpack(resourceStream, file);
            }
        }
    }
}
