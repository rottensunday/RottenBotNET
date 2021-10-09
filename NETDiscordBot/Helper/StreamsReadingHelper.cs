namespace NETDiscordBot.Helper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    
    // TODO: Move to other file and reconsider whether it's needed
    public class AggregatedStreamData
    {
        public string UserName { get; init; }
        public TimeSpan StreamTime { get; init; }
        public TimeSpan LonelyStreamTime { get; init; }
        public TimeSpan LongestStreamTime { get; init; }
    }

    public static class StreamsReadingHelper
    {
        public static void DisplayStreamsData(IEnumerable<StreamEntry> streams)
        {
            IEnumerable<AggregatedStreamData> transformedResult = streams
                .GroupBy(entry => entry.UserId)
                .Select(g =>
                    g.Aggregate(new AggregatedStreamData(), (acc, curr) =>
                    {
                        if (curr.StreamEnd is null)
                        {
                            return acc;
                        }

                        var diff = curr.StreamEnd.Value - curr.StreamStart;

                        return new AggregatedStreamData()
                        {
                            UserName = string.IsNullOrWhiteSpace(acc.UserName) ? curr.UserName : acc.UserName,
                            StreamTime = acc.StreamTime + diff,
                            LonelyStreamTime = acc.LonelyStreamTime + TimeSpan.FromSeconds(curr.TotalLonelyStreamTime ?? 0),
                            LongestStreamTime = acc.LongestStreamTime > diff ? acc.LongestStreamTime : diff
                        };
                    }))
                .ToList();
            
            Console.WriteLine($"{"UserName",15}{"Total Stream Time",25}{"Lonely Stream Time",25}{"Longest Stream Time",25}");
            foreach (var data in transformedResult)
            {
                Console.WriteLine($"{data.UserName,15}{data.StreamTime.TotalHours,25}{data.LonelyStreamTime.TotalHours,25}{data.LongestStreamTime.TotalHours,25}");
            }
        }
    }
}