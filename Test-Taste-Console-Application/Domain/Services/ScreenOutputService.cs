using System;
using System.Linq;
using Test_Taste_Console_Application.Constants;
using Test_Taste_Console_Application.Domain.Objects;
using Test_Taste_Console_Application.Domain.Services.Interfaces;
using Test_Taste_Console_Application.Utilities;

namespace Test_Taste_Console_Application.Domain.Services
{
    /// <inheritdoc />
    public class ScreenOutputService : IOutputService
    {
        private readonly IPlanetService _planetService;

        private readonly IMoonService _moonService;

        public ScreenOutputService(IPlanetService planetService, IMoonService moonService)
        {
            _planetService = planetService;
            _moonService = moonService;
        }

        public void OutputAllPlanetsAndTheirMoonsToConsole()
        {
            //The service gets all the planets from the API.
            var planets = _planetService.GetAllPlanets().ToArray();

            //If the planets aren't found, then the function stops and tells that to the user via the console.
            if (!planets.Any())
            {
                Console.WriteLine(OutputString.NoPlanetsFound);
                return;
            }

            //The column sizes and labels for the planets are configured here. 
            var columnSizesForPlanets = new[] { 20, 20, 30, 20 };
            var columnLabelsForPlanets = new[]
            {
                OutputString.PlanetNumber, OutputString.PlanetId, OutputString.PlanetSemiMajorAxis,
                OutputString.TotalMoons
            };

            //The column sizes and labels for the moons are configured here.
            //The second moon's column needs the 2 extra '-' characters so that it's aligned with the planet's column.
            var columnSizesForMoons = new[] { 20, 70 + 2 };
            var columnLabelsForMoons = new[]
            {
                OutputString.MoonNumber, OutputString.MoonId
            };

            //The for loop creates the correct output.
            for (int i = 0, j = 1; i < planets.Length; i++, j++)
            {
                //First the line is created.
                ConsoleWriter.CreateLine(columnSizesForPlanets);

                //Under the line the header is created.
                ConsoleWriter.CreateText(columnLabelsForPlanets, columnSizesForPlanets);

                //Under the header the planet data is shown.
                ConsoleWriter.CreateText(
                    new[]
                    {
                        j.ToString(), CultureInfoUtility.TextInfo.ToTitleCase(planets[i].Id),
                        planets[i].SemiMajorAxis.ToString(),
                        planets[i].Moons.Count.ToString()
                    },
                    columnSizesForPlanets);

                //Under the planet data the header for the moons is created.
                ConsoleWriter.CreateLine(columnSizesForPlanets);
                ConsoleWriter.CreateText(columnLabelsForMoons, columnSizesForMoons);

                //The for loop creates the correct output.
                for (int k = 0, l = 1; k < planets[i].Moons.Count; k++, l++)
                {
                    ConsoleWriter.CreateText(
                        new[]
                        {
                            l.ToString(), CultureInfoUtility.TextInfo.ToTitleCase(planets[i].Moons.ElementAt(k).Id)
                        },
                        columnSizesForMoons);
                }

                //Under the data the footer is created.
                ConsoleWriter.CreateLine(columnSizesForMoons);
                ConsoleWriter.CreateEmptyLines(2);

                /*
                    This is an example of the output for the planet Earth:
                    --------------------+--------------------+------------------------------+--------------------
                    Planet's Number     |Planet's Id         |Planet's Semi-Major Axis      |Total Moons
                    10                  |Terre               |0                             |1
                    --------------------+--------------------+------------------------------+--------------------
                    Moon's Number       |Moon's Id
                    1                   |La Lune
                    --------------------+------------------------------------------------------------------------
                */
            }
        }

        public void OutputAllMoonsAndTheirMassToConsole()
        {
            //The function works the same way as the PrintAllPlanetsAndTheirMoonsToConsole function. You can find more comments there.
            var moons = _moonService.GetAllMoons().ToArray();

            if (!moons.Any())
            {
                Console.WriteLine(OutputString.NoMoonsFound);
                return;
            }

            var columnSizesForMoons = new[] { 20, 20, 30, 20 };
            var columnLabelsForMoons = new[]
            {
                OutputString.MoonNumber, OutputString.MoonId, OutputString.MoonMassExponent, OutputString.MoonMassValue
            };

            ConsoleWriter.CreateHeader(columnLabelsForMoons, columnSizesForMoons);

            for (int i = 0, j = 1; i < moons.Length; i++, j++)
            {
                ConsoleWriter.CreateText(
                    new[]
                    {
                        j.ToString(), CultureInfoUtility.TextInfo.ToTitleCase(moons[i].Id),
                        moons[i].MassExponent.ToString(), moons[i].MassValue.ToString()
                    },
                    columnSizesForMoons);
            }

            ConsoleWriter.CreateLine(columnSizesForMoons);
            ConsoleWriter.CreateEmptyLines(2);

            /*
                This is an example of the output for the moon around the earth:
                --------------------+--------------------+------------------------------+--------------------
                Moon's Number       |Moon's Id           |Moon's Mass Exponent          |Moon's Mass Value
                --------------------+--------------------+------------------------------+--------------------
                1                   |Lune                |22                            |7,346             
                ...more data...
                --------------------+--------------------+------------------------------+--------------------
             */
        }

        public void OutputAllPlanetsAndTheirAverageMoonGravityToConsole()
        {
            //The function works the same way as the PrintAllPlanetsAndTheirMoonsToConsole function. You can find more comments there.
            var planets = _planetService.GetAllPlanets().ToArray();
            if (!planets.Any())
            {
                Console.WriteLine(OutputString.NoMoonsFound);
                return;
            }

            var columnSizes = new[] { 20, 30 };
            var columnLabels = new[]
            {
                OutputString.PlanetId, OutputString.PlanetMoonAverageGravity
            };


            ConsoleWriter.CreateHeader(columnLabels, columnSizes);

            foreach (Planet planet in planets)
            {
                if (planet.HasMoons())
                {
                    ConsoleWriter.CreateText(new string[] { $"{planet.Id}", $"{planet.AverageMoonGravity}" }, columnSizes);
                }
                else
                {
                    ConsoleWriter.CreateText(new string[] { $"{planet.Id}", $"-" }, columnSizes);
                }
            }

            ConsoleWriter.CreateLine(columnSizes);
            ConsoleWriter.CreateEmptyLines(2);

            /*
                --------------------+--------------------------------------------------
                Planet's Number     |Planet's Average Moon Gravity
                --------------------+--------------------------------------------------
                1                   |0.0f
                --------------------+--------------------------------------------------
            */
        }

        // Outputs the average temperature of moons for each planet to the console.
        public void OutputPlanetsWithMoonAvgTemperatureToConsole()
        {
            Console.WriteLine("Loading planets and their moons' average temperatures...");

            // Retrieve all planets using the planet service and convert the result to a list for easier manipulation.
            var planets = _planetService.GetAllPlanets().ToList();

            // If no planets are found, display a message and exit the method.
            if (!planets.Any())
            {
                Console.WriteLine(OutputString.NoPlanetsFound);
                return;
            }

            // Define the sizes for the columns to be displayed in the console output.
            var columnSizes = new[] { 20, 30 };
            // Define the labels for the table columns: Planet Id and Moon Average Temperature.
            var columnLabels = new[]
            {
                OutputString.PlanetId, OutputString.MoonAvgTemp
            };

            // Remove planets that do not have any moons, since we're only interested in planets with moons.
            planets.RemoveAll(p => p.Moons == null || p.Moons.Count == 0);

            // If no planets with moons are left after filtering, display a message and exit the method.
            if (!planets.Any())
            {
                Console.WriteLine(OutputString.NoPlanetsFoundWithMoon);
                return;
            }

            // Create and print the table header based on column labels and sizes.
            ConsoleWriter.CreateHeader(columnLabels, columnSizes);

            // Iterate through each planet that has moons.
            foreach (var planet in planets)
            {
                if (planet.Moons != null && planet.Moons.Any())
                {
                    // Collect all average temperatures of the moons that have a temperature value.
                    var moonTemperatures = planet.Moons
                                                 .Where(m => m.AvgTemp.HasValue)
                                                 .Select(m => m.AvgTemp.Value)
                                                 .ToList();

                    if (moonTemperatures.Any())
                    {
                        // If there are valid temperatures, calculate the average temperature.
                        var averageTemp = moonTemperatures.Average();

                        // Output the planet ID and the calculated average moon temperature.
                        ConsoleWriter.CreateText(new string[] { $"{planet.Id}", $"{averageTemp}" }, columnSizes);
                    }
                    else
                    {
                        // If no moon temperatures are available, output the planet ID and a placeholder (e.g., "-").
                        ConsoleWriter.CreateText(new string[] { $"{planet.Id}", $"-" }, columnSizes);
                    }
                }
            }

            // Draw a line at the end of the table for visual separation.
            ConsoleWriter.CreateLine(columnSizes);

            // Add two empty lines for spacing after the output.
            ConsoleWriter.CreateEmptyLines(2);

            /*
               Example Output:
               --------------------+--------------------------------------------------
               Planet's Id          | Planet's Average Moon Gravity
               --------------------+--------------------------------------------------
               1                   | 0.0
               --------------------+--------------------------------------------------
            */
        }

    }
}
