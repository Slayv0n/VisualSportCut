using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Text;
using System.Text.Json;
using VisualSportCut.Domain.Interfaces;
using VisualSportCut.Domain.Models;

namespace VisualSportCut.Infrastructure.Services
{
    public class JsonLoader : IJsonLoader
    {
        public async Task<List<Stamp>> LoadAsync(string filePath)
        {
            var json = await File.ReadAllTextAsync(filePath);
            var doc = JsonDocument.Parse(json);
            var dataElement = doc.RootElement.GetProperty("data");

            var stamps = new List<Stamp>();
            foreach (var timelineElement in dataElement.EnumerateArray())
            {
                stamps.AddRange(ParseStamps(timelineElement.GetProperty("stamps"),
                    timelineElement.GetProperty("name").GetString()!));
            }

            return stamps;
        }

        private List<Stamp> ParseStamps(JsonElement stampsElement, string name)
        {
            var stamps = new List<Stamp>();
            foreach (var stampElement in stampsElement.EnumerateArray())
            {
                stamps.Add(Stamp.Create(name,
                    stampElement.GetProperty("timeStart").GetString()!,
                    stampElement.GetProperty("timeFinish").GetString()!,
                    ParseTimeEvents(stampElement.GetProperty("timeEvents")!),
                    ParseTag(stampElement.GetProperty("tag")!),
                    ParseLabels(stampElement.GetProperty("labels")!)
                    ));
            }
            return stamps;
        }

        private List<TimeEvent> ParseTimeEvents(JsonElement timeEventsElement)
        {
            var timeEvents = new List<TimeEvent>();
            foreach (var timeEventElement in timeEventsElement.EnumerateArray())
            {
                timeEvents.Add(TimeEvent.Create(timeEventElement.GetProperty("name")!.GetString()));
            }
            return timeEvents;
        }

        private Tag ParseTag(JsonElement tagElement)
        {
            var tag = Tag.Create(tagElement.GetProperty("name").GetString(),
                tagElement.GetProperty("group").GetProperty("name").GetString(),
                tagElement.GetProperty("color").GetString());
            return tag;
        }

        private List<LabelEvent> ParseLabels(JsonElement labelEventsElement)
        {
            var labelEvents = new List<LabelEvent>();
            foreach (var labelEventElement in labelEventsElement.EnumerateArray())
            {
                labelEvents.Add(LabelEvent.Create(labelEventElement.GetProperty("name").GetString(),
                    labelEventElement.GetProperty("group").GetProperty("name").GetString()));
            }
            return labelEvents;
        }

    }
}
