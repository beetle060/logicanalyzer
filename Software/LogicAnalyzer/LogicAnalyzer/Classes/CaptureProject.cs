using SharedDriver;
using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LogicAnalyzer.Classes
{
    internal sealed record ProjectChannel(
        [property: JsonRequired] int Number, string Name, [property: JsonRequired] bool Enabled, uint? Color);

    internal sealed record ProjectAcquisition(
        AnalyzerDriverType DriverType, int Frequency, int PreSamples, int PostSamples,
        bool PatternTrigger, int EdgeChannel, bool NegativeEdge, bool Blast,
        bool Burst, int BurstCount, bool MeasureDelays, int PatternChannel, string Pattern, bool Fast);

    internal sealed record CaptureProject(ProjectChannel[] Channels, ProjectAcquisition? Acquisition)
    {
        private const string ProjectFormat = "logicanalyzer-project";
        private const string FalconFormat = "falcon030-logic-analyzer-connections";
        private const int ProjectVersion = 1;
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        private sealed record ProjectFile(string Schema, int Version, CaptureProject Project);

        public async Task SaveAsync(string path)
        {
            string temporaryPath = Path.Combine(Path.GetDirectoryName(path)!, Path.GetRandomFileName());

            try
            {
                await using (var output = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write))
                    await JsonSerializer.SerializeAsync(output, new ProjectFile(ProjectFormat, ProjectVersion, this), JsonOptions);

                File.Move(temporaryPath, path, overwrite: true);
            }
            finally
            {
                File.Delete(temporaryPath);
            }
        }

        public static async Task<CaptureProject> LoadAsync(Stream input, int? channelCount = null)
        {
            using var document = await JsonDocument.ParseAsync(input);
            var root = document.RootElement;
            var project = root.GetProperty("schema").GetString() switch
            {
                ProjectFormat when root.GetProperty("version").GetInt32() == ProjectVersion =>
                    ReadNativeProject(root.GetProperty("project")),
                FalconFormat when root.GetProperty("schema_version").GetInt32() == 2 =>
                    ImportFalconChannelList(root),
                _ => throw new InvalidDataException("Unsupported project or channel list format.")
            };

            if (project?.Channels == null || project.Channels.Length == 0 ||
                project.Channels.Any(c => c == null || c.Name == null || c.Number < 0) ||
                project.Channels.Select(c => c.Number).Distinct().Count() != project.Channels.Length)
                throw new InvalidDataException("The file contains an invalid channel list.");

            if (channelCount is int count)
                project.ValidateChannelCount(count);

            return project;
        }

        public void ValidateChannelCount(int channelCount)
        {
            if (Channels.Any(c => c.Number >= channelCount))
                throw new InvalidDataException($"The project needs {Channels.Max(c => c.Number) + 1L} channels. The current device provides {channelCount}.");
        }

        private static CaptureProject ReadNativeProject(JsonElement element)
        {
            var project = element.Deserialize<CaptureProject>(JsonOptions);
            if (project?.Acquisition == null)
                throw new InvalidDataException("The project is missing acquisition settings.");

            return project;
        }

        private static CaptureProject ImportFalconChannelList(JsonElement root)
        {
            var channels = root.GetProperty("channels").EnumerateArray().Select(channel =>
            {
                var number = channel.GetProperty("channel_index").GetInt32();
                if (channel.GetProperty("global_channel").GetInt32() != number + 1L)
                    throw new InvalidDataException("The channel list contains conflicting channel numbers.");

                return new ProjectChannel(number, channel.GetProperty("name").GetString()!, true, null);
            }).ToArray();

            return new CaptureProject(channels, null);
        }
    }
}
