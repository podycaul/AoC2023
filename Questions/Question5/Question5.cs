
using Microsoft.Extensions.Configuration;

// answer: 26273516
public class Question5 : QuestionBase, IQuestion
{
    public int Number => 5;

    public Question5(IConfiguration configuration) : base(configuration)
    {
        base.QuestionNumber = this.Number;
    }

    public async Task<string> Run()
    {
        var input = (await base.GetQuestionInput()).ToList();

        List<double> seeds = GetSeeds(input);

        var seedToSoilMap = GetRawMappingValues(MapType.SeedToSoil, input);
        var soilToFertilizerMap = GetRawMappingValues(MapType.SoilToFertilizer, input);
        var fertilizerToWaterMap = GetRawMappingValues(MapType.FertilizerToWater, input);
        var waterToLightMap = GetRawMappingValues(MapType.WaterToLight, input);
        var lightToTemperatureMap = GetRawMappingValues(MapType.LightToTemperature, input);
        var tempToHumidityMap = GetRawMappingValues(MapType.TemperatureToHumidity, input);
        var humidityToLocationMap = GetRawMappingValues(MapType.HumidityToLocation, input);

        List<double> locations = new();
        foreach (var seed in seeds)
        {
            double seedToSoil = GetMapValue(seedToSoilMap, seed);
            double soilToFertilizer = GetMapValue(soilToFertilizerMap, seedToSoil);
            double fertilizerToWater = GetMapValue(fertilizerToWaterMap, soilToFertilizer);
            double waterToLight = GetMapValue(waterToLightMap, fertilizerToWater);
            double lightToTemp = GetMapValue(lightToTemperatureMap, waterToLight);
            double tempToHumidity = GetMapValue(tempToHumidityMap, lightToTemp);
            double humdityToLocation = GetMapValue(humidityToLocationMap, tempToHumidity);
            locations.Add(humdityToLocation);
        }

        return locations.Min().ToString();
    }

    private double GetMapValue(MappingData data, double item)
    {
        double mappedValue = item;
        var orderedRanges = data.Ranges.OrderBy(x => x.Source).ToList();
        // if the given 'item' is less than the lowest number in the map, we know the number doesn't exist in the map
        if (orderedRanges.First().Source > item) return mappedValue;

        // iterate over the ranges to find where the 'item' falls in the map
        foreach (var range in orderedRanges)
        {
            if (range.Source <= item && range.Source + range.Amount >= item)
            {
                mappedValue = range.Destination + (item - range.Source);
            }
        }

        return mappedValue;
    }

    private List<double> GetSeeds(List<string> input)
    {
        string[] numbers = input[0].Split(":")[1].Trim().Split(" ");
        return numbers.Select(n => double.Parse(n)).ToList();
    }

    private MappingData GetRawMappingValues(string type, List<string> input)
    {
        MappingData mapping = new()
        {
            Type = type
        };
        for (int i = 0; i < input.Count; i++)
        {
            if (!input[i].Contains(type)) continue;
            for (int j = i + 1; j < input.Count; j++)
            {
                string[] numbers = input[j].Split(" ");
                mapping.Ranges.Add(new Range
                {
                    Destination = double.Parse(numbers[0]),
                    Source = double.Parse(numbers[1]),
                    Amount = double.Parse(numbers[2])
                });

                if (j + 1 > input.Count - 1 || string.IsNullOrEmpty(input[j + 1])) break;
            }

            break;
        }

        return mapping;
    }

    private record MappingData
    {
        public string Type { get; init; }
        public List<Range> Ranges { get; init; } = new();
    }

    private record Range
    {
        public double Source { get; init; }
        public double Destination { get; init; }
        public double Amount { get; init; }
    }

    private static class MapType
    {
        public static string SeedToSoil = "seed-to-soil";
        public static string SoilToFertilizer = "soil-to-fertilizer";
        public static string FertilizerToWater = "fertilizer-to-water";
        public static string WaterToLight = "water-to-light";
        public static string LightToTemperature = "light-to-temperature";
        public static string TemperatureToHumidity = "temperature-to-humidity";
        public static string HumidityToLocation = "humidity-to-location";
    }
}